using ArticlesCatalog.Api.Entities;

namespace ArticlesCatalog.Api.Services.Interfaces;

/// <summary>
/// Предоставляет операции для управления тегами, связанными со статьями.
/// </summary>
public interface ITagService
{
    /// <summary>
    /// Присоединяет указанные теги к статье, сохраняя их порядок.
    /// </summary>
    /// <param name="article">Статья, которую нужно обновить.</param>
    /// <param name="normalizedTagNames">Коллекция нормализованных имён тегов.</param>
    /// <param name="ct">Токен отмены.</param>
    Task AttachTagsAsync(Article article, IEnumerable<string> normalizedTagNames, CancellationToken ct);
}