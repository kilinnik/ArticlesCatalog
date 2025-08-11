using ArticlesCatalog.Api.Services;

namespace ArticlesCatalog.Api.Services.Interfaces;

/// <summary>
/// Интерфейс нормализатора тегов.
/// </summary>
public interface ITagNormalizer
{
    /// <summary>
    /// Нормализует указанные теги.
    /// </summary>
    /// <param name="tags">Теги для нормализации.</param>
    /// <param name="failOnDuplicate">Указывает, должны ли дубликаты приводить к ошибке.</param>
    /// <returns>Результат нормализации.</returns>
    NormalizationResult Normalize(IEnumerable<string>? tags, bool failOnDuplicate);
}