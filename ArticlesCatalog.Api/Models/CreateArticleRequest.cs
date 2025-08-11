namespace ArticlesCatalog.Api.Models;

/// <summary>
/// Тело запроса для создания или обновления статьи.
/// </summary>
public class CreateArticleRequest
{
    /// <summary>
    /// Заголовок статьи.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Коллекция тегов статьи.
    /// </summary>
    public List<string> Tags { get; set; } = [];
}