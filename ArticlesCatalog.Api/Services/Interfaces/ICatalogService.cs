using ArticlesCatalog.Api.Models;

namespace ArticlesCatalog.Api.Services.Interfaces;

/// <summary>
/// Определяет операции для чтения каталога.
/// </summary>
public interface ICatalogService
{
    /// <summary>
    /// Возвращает постраничный список секций каталога.
    /// </summary>
    /// <param name="skip">Количество секций, которые нужно пропустить.</param>
    /// <param name="take">Количество секций, которые нужно взять.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Запрошенные секции.</returns>
    Task<List<SectionResponse>> GetSectionsAsync(int skip, int take, CancellationToken ct);

    /// <summary>
    /// Возвращает статьи, принадлежащие указанному ключу секции.
    /// </summary>
    /// <param name="sectionKey">Ключ секции.</param>
    /// <param name="skip">Количество статей, которые нужно пропустить.</param>
    /// <param name="take">Количество статей, которые нужно взять.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Запрошенные статьи.</returns>
    Task<List<ArticleResponse>> GetArticlesBySectionAsync(string sectionKey, int skip, int take, CancellationToken ct);
}