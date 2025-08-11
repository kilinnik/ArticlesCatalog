namespace ArticlesCatalog.Api.Constants;

/// <summary>
/// Определяет числовые ограничения, используемые в приложении.
/// </summary>
public static class Limits
{
    /// <summary>
    /// Максимальное количество тегов в одной статье.
    /// </summary>
    public const int MaxTagsPerArticle = 256;

    /// <summary>
    /// Максимальная длина имени тега.
    /// </summary>
    public const int MaxTagLength = 256;

    /// <summary>
    /// Максимальная длина заголовка статьи.
    /// </summary>
    public const int MaxArticleTitleLength = 256;

    /// <summary>
    /// Максимальная длина сгенерированного имени секции.
    /// </summary>
    public const int MaxSectionNameLength = 1024;
}
