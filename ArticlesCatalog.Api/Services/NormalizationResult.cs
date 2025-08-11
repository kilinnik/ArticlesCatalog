namespace ArticlesCatalog.Api.Services;

/// <summary>
/// Результат нормализации набора тегов.
/// </summary>
public class NormalizationResult
{
    /// <summary>
    /// Успешна ли нормализация.
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// Нормализованные уникальные теги.
    /// </summary>
    public List<string> NormalizedTags { get; init; } = new();

    /// <summary>
    /// Сообщение об ошибке, если нормализация не удалась.
    /// </summary>
    public string? Error { get; init; }
}