namespace ArticlesCatalog.Api.Models;

/// <summary>
/// Представляет секцию каталога, возвращаемую API.
/// </summary>
public class SectionResponse
{
    public string SectionKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = [];
    public int ArticlesCount { get; set; }
}