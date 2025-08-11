using ArticlesCatalog.Api.Entities;
using ArticlesCatalog.Api.Persistence;
using ArticlesCatalog.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ArticlesCatalog.Api.Services;

/// <summary>
/// Предоставляет операции для работы с тегами в базе данных.
/// </summary>
public sealed class TagService : ITagService
{
    private readonly AppDbContext _db;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="TagService"/>.
    /// </summary>
    public TagService(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Присоединяет указанные теги к статье, создавая новые при необходимости.
    /// </summary>
    /// <param name="article">Статья, к которой прикрепляются теги.</param>
    /// <param name="normalizedTagNames">Нормализованные имена тегов для присоединения.</param>
    /// <param name="ct">Токен отмены.</param>
    public async Task AttachTagsAsync(Article article, IEnumerable<string> normalizedTagNames, CancellationToken ct)
    {
        List<(string Original, string Lower)> tags = normalizedTagNames
            .Select(t => (Original: t, Lower: t.ToLowerInvariant()))
            .ToList();

        HashSet<string> normalizedSet = tags
            .Select(t => t.Lower)
            .ToHashSet(StringComparer.Ordinal);

        List<Tag> existing = await _db.Tags
            .Where(t => normalizedSet.Contains(t.NormalizedName))
            .ToListAsync(ct);
        Dictionary<string, Tag> existingMap = existing
            .ToDictionary(t => t.NormalizedName, StringComparer.Ordinal);

        int position = 0;
        foreach ((string Original, string Lower) tag in tags)
        {
            if (!existingMap.TryGetValue(tag.Lower, out Tag? tagEntity))
            {
                tagEntity = new Tag { Name = tag.Original };
                _db.Tags.Add(tagEntity);
                existingMap[tag.Lower] = tagEntity;
            }

            article.AddTag(tagEntity, position++);
        }
    }
}