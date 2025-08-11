using ArticlesCatalog.Api.Models;
using ArticlesCatalog.Api.Persistence;
using ArticlesCatalog.Api.Resources;
using ArticlesCatalog.Api.Constants;
using ArticlesCatalog.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ArticlesCatalog.Api.Services;

/// <summary>
/// Предоставляет операции для запросов к каталогу, таким как секции и статьи.
/// </summary>
public sealed class CatalogService : ICatalogService
{
    private readonly AppDbContext _db;
    private readonly ISectionKeyBuilder _sectionKeyBuilder;
    private readonly ILogger<CatalogService> _logger;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="CatalogService"/>.
    /// </summary>
    public CatalogService(AppDbContext db, ISectionKeyBuilder sectionKeyBuilder, ILogger<CatalogService> logger)
    {
        _db = db;
        _sectionKeyBuilder = sectionKeyBuilder;
        _logger = logger;
    }

    /// <summary>
    /// Возвращает постраничный список секций, упорядоченных по количеству статей.
    /// </summary>
    public async Task<List<SectionResponse>> GetSectionsAsync(int skip, int take, CancellationToken ct)
    {
        _logger.LogInformation("Получение секций {Skip} {Take}", skip, take);

        List<SectionResponse> sections = await _db.Articles
            .AsNoTracking()
            .GroupBy(a => a.SectionKey)
            .Select(g => new SectionResponse
            {
                SectionKey = g.Key,
                ArticlesCount = g.Count(),
                Tags = g
                    .SelectMany(a => a.ArticleTags)
                    .Select(at => at.Tag.Name)
                    .Distinct()
                    .OrderBy(n => n)
                    .ToList(),
            })
            .OrderByDescending(s => s.ArticlesCount)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);

        foreach (SectionResponse section in sections)
        {
            string name = section.Tags.Count == 0 ? "Без тегов" : string.Join(", ", section.Tags);
            if (name.Length > Limits.MaxSectionNameLength)
            {
                name = name[..Limits.MaxSectionNameLength];
            }

            section.Name = name;
        }

        return sections;
    }

    /// <summary>
    /// Возвращает статьи, принадлежащие секции по её ключу.
    /// </summary>
    public async Task<List<ArticleResponse>> GetArticlesBySectionAsync(string sectionKey, int skip, int take,
        CancellationToken ct)
    {
        List<int> tagIds = ParseSectionKey(sectionKey);
        string normalizedKey = _sectionKeyBuilder.ComputeSectionKey(tagIds);

        _logger.LogInformation("Получение статей для секции {SectionKey}", normalizedKey);
        return await _db.Articles
            .AsNoTracking()
            .Where(a => a.SectionKey == normalizedKey)
            .OrderByDescending(a => a.UpdatedAt ?? a.CreatedAt)
            .Skip(skip)
            .Take(take)
            .Select(a => new ArticleResponse
            {
                Id = a.Id,
                Title = a.Title,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt,
                Tags = a.ArticleTags
                    .OrderBy(at => at.Position)
                    .Select(at => at.Tag.Name)
                    .ToList()
            })
            .ToListAsync(ct);
    }

    private List<int> ParseSectionKey(string sectionKey)
    {
        string[] parts = sectionKey.Split(',', StringSplitOptions.RemoveEmptyEntries);
        List<int> tagIds = [];
        foreach (string part in parts)
        {
            if (!int.TryParse(part, out int tagId))
            {
                _logger.LogError("Некорректный формат ключа секции: {SectionKey}", sectionKey);
                throw new ArgumentException(ErrorMessages.InvalidSectionKeyFormat);
            }

            tagIds.Add(tagId);
        }

        return tagIds
            .Distinct()
            .ToList();
    }
}