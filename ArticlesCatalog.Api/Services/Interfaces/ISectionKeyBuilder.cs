namespace ArticlesCatalog.Api.Services.Interfaces;

/// <summary>
/// Интерфейс построителя ключей секций.
/// </summary>
public interface ISectionKeyBuilder
{
    /// <summary>
    /// Вычисляет детерминированный ключ секции для указанных идентификаторов тегов.
    /// </summary>
    /// <param name="tagIds">Идентификаторы тегов.</param>
    /// <returns>Сформированный ключ секции.</returns>
    string ComputeSectionKey(IEnumerable<int> tagIds);
}