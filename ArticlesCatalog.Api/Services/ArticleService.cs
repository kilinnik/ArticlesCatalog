using ArticlesCatalog.Api.Entities;
using ArticlesCatalog.Api.Models;
using ArticlesCatalog.Api.Persistence;
using ArticlesCatalog.Api.Constants;
using ArticlesCatalog.Api.Mappers;
using ArticlesCatalog.Api.Resources;
using ArticlesCatalog.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;

namespace ArticlesCatalog.Api.Services;

/// <summary>
/// Обрабатывает создание, получение, обновление и удаление статей.
/// </summary>
public sealed class ArticleService : IArticleService
{
    private readonly AppDbContext _db;
    private readonly ISectionKeyBuilder _sectionKeyBuilder;
    private readonly ITagService _tagService;
    private readonly ILogger<ArticleService> _logger;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="ArticleService"/>.
    /// </summary>
    public ArticleService(AppDbContext db,
        ISectionKeyBuilder sectionKeyBuilder,
        ITagService tagService,
        ILogger<ArticleService> logger)
    {
        _db = db;
        _sectionKeyBuilder = sectionKeyBuilder;
        _tagService = tagService;
        _logger = logger;
    }

    /// <summary>
    /// Создаёт новую статью и её теги.
    /// </summary>
    public async Task<ServiceResult<ArticleResponse>> CreateAsync(CreateArticleRequest request, CancellationToken ct)
    {
        _logger.LogInformation("Создание статьи {Title}", request.Title);

        Article article = new Article
        {
            Title = request.Title,
            CreatedAt = DateTimeOffset.UtcNow
        };

        try
        {
            await using IDbContextTransaction transaction = await _db.Database.BeginTransactionAsync(ct);
            _db.Articles.Add(article);
            await _tagService.AttachTagsAsync(article, request.Tags, ct);
            await _db.SaveChangesAsync(ct);
            article.SectionKey = _sectionKeyBuilder.ComputeSectionKey(
                article.ArticleTags.Select(at => at.TagId));
            await _db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            return ServiceResult<ArticleResponse>.Validation(FieldNames.Tags,
                ErrorMessages.TagsMustNotContainDuplicates);
        }

        return ServiceResult<ArticleResponse>.Success(article.ToResponse());
    }

    /// <summary>
    /// Возвращает статью по идентификатору или <c>null</c>, если она не найдена.
    /// </summary>
    public async Task<ArticleResponse?> GetAsync(int id, CancellationToken ct)
    {
        Article? article = await _db.Articles
            .AsNoTracking()
            .Include(a => a.ArticleTags)
            .ThenInclude(at => at.Tag)
            .FirstOrDefaultAsync(a => a.Id == id, ct);

        return article?.ToResponse();
    }

    /// <summary>
    /// Обновляет существующую статью новым заголовком или тегами.
    /// </summary>
    public async Task<ServiceResult<ArticleResponse>> UpdateAsync(int id, CreateArticleRequest request,
        CancellationToken ct)
    {
        Article? article = await _db.Articles
            .Include(a => a.ArticleTags)
            .ThenInclude(at => at.Tag)
            .FirstOrDefaultAsync(a => a.Id == id, ct);
        if (article == null)
        {
            return ServiceResult<ArticleResponse>.NotFound();
        }

        _logger.LogInformation("Обновление статьи {Id}", id);

        List<string> existingTags = article.ArticleTags
            .OrderBy(a => a.Position)
            .Select(a => a.Tag.Name)
            .ToList();
        bool titleChanged = article.Title != request.Title;
        bool tagsChanged = !existingTags.SequenceEqual(request.Tags, StringComparer.OrdinalIgnoreCase);

        if (!titleChanged && !tagsChanged)
        {
            return ServiceResult<ArticleResponse>.Success(article.ToResponse());
        }

        try
        {
            await using IDbContextTransaction transaction = await _db.Database.BeginTransactionAsync(ct);
            article.Title = request.Title;
            article.UpdatedAt = DateTimeOffset.UtcNow;
            article.ArticleTags.Clear();

            await _tagService.AttachTagsAsync(article, request.Tags, ct);
            await _db.SaveChangesAsync(ct);
            article.SectionKey = _sectionKeyBuilder.ComputeSectionKey(
                article.ArticleTags.Select(at => at.TagId));
            await _db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            return ServiceResult<ArticleResponse>.Validation(FieldNames.Tags,
                ErrorMessages.TagsMustNotContainDuplicates);
        }

        return ServiceResult<ArticleResponse>.Success(article.ToResponse());
    }

    /// <summary>
    /// Удаляет статью по идентификатору, если она существует.
    /// </summary>
    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        Article? article = await _db.Articles
            .Include(a => a.ArticleTags)
            .FirstOrDefaultAsync(a => a.Id == id, ct);
        if (article == null)
        {
            return;
        }

        _logger.LogInformation("Удаление статьи {Id}", id);
        _db.ArticleTags.RemoveRange(article.ArticleTags);
        _db.Articles.Remove(article);
        await _db.SaveChangesAsync(ct);
    }

    private static bool IsUniqueViolation(DbUpdateException ex)
    {
        return ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };
    }
}