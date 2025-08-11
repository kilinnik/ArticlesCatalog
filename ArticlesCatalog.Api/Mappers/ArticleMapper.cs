using ArticlesCatalog.Api.Entities;
using ArticlesCatalog.Api.Models;

namespace ArticlesCatalog.Api.Mappers;

/// <summary>
/// Предоставляет методы расширения для преобразования сущностей в модели API.
/// </summary>
public static class ArticleMapper
{
    /// <summary>
    /// Преобразует сущность <see cref="Article"/> в модель <see cref="ArticleResponse"/>.
    /// </summary>
    /// <param name="article">Преобразуемая сущность статьи.</param>
    /// <returns>Соответствующая модель <see cref="ArticleResponse"/>.</returns>
    public static ArticleResponse ToResponse(this Article article)
    {
        List<string> tagNames = article
            .ArticleTags.OrderBy(a => a.Position)
            .Select(a => a.Tag.Name)
            .ToList();

        return new ArticleResponse
        {
            Id = article.Id,
            Title = article.Title,
            CreatedAt = article.CreatedAt,
            UpdatedAt = article.UpdatedAt,
            Tags = tagNames
        };
    }
}