using ArticlesCatalog.Api.Models;
using ArticlesCatalog.Api.Services;

namespace ArticlesCatalog.Api.Services.Interfaces;

/// <summary>
/// Определяет операции для управления статьями.
/// </summary>
public interface IArticleService
{
    /// <summary>
    /// Создаёт новую статью.
    /// </summary>
    /// <param name="request">Данные статьи.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Созданная статья, обёрнутая в результат сервиса.</returns>
    Task<ServiceResult<ArticleResponse>> CreateAsync(CreateArticleRequest request, CancellationToken ct);

    /// <summary>
    /// Возвращает статью по её идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор статьи.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Статья, если найдена; иначе <c>null</c>.</returns>
    Task<ArticleResponse?> GetAsync(int id, CancellationToken ct);

    /// <summary>
    /// Обновляет существующую статью.
    /// </summary>
    /// <param name="id">Идентификатор статьи.</param>
    /// <param name="request">Новые данные статьи.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Обновлённая статья, обёрнутая в результат сервиса.</returns>
    Task<ServiceResult<ArticleResponse>> UpdateAsync(int id, CreateArticleRequest request, CancellationToken ct);

    /// <summary>
    /// Удаляет статью по её идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор статьи.</param>
    /// <param name="ct">Токен отмены.</param>
    Task DeleteAsync(int id, CancellationToken ct);
}