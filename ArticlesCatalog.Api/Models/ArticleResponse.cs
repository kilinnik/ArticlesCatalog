namespace ArticlesCatalog.Api.Models;

/// <summary>
/// Представление статьи, возвращаемое клиентам API.
/// </summary>
public class ArticleResponse
{
    /// <summary>
    /// Уникальный идентификатор статьи.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Заголовок статьи.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Дата и время создания статьи.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Дата и время последнего обновления статьи, если оно было.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Список тегов, связанных со статьёй.
    /// </summary>
    public List<string> Tags { get; set; } = [];
}