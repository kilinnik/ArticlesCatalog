using ArticlesCatalog.Api.Models;
using ArticlesCatalog.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ArticlesCatalog.Api.Controllers;

/// <summary>
/// Предоставляет конечные точки для просмотра секций и статей.
/// </summary>
[ApiController]
[Route("api/catalog")]
public class CatalogController : ControllerBase
{
    private readonly ICatalogService _service;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="CatalogController"/>.
    /// </summary>
    public CatalogController(ICatalogService service)
    {
        _service = service;
    }

    /// <summary>
    /// Возвращает постраничный список секций каталога.
    /// </summary>
    [HttpGet("sections")]
    public async Task<IActionResult> GetSections(
        [FromQuery] PaginationParameters pagination,
        CancellationToken ct
    )
    {
        if (!pagination.TryValidate(out string? error))
        {
            return BadRequest(error);
        }

        List<SectionResponse> sections = await _service.GetSectionsAsync(
            pagination.Skip,
            pagination.Take,
            ct
        );
        
        return Ok(sections);
    }

    /// <summary>
    /// Возвращает статьи, принадлежащие указанному ключу секции.
    /// </summary>
    [HttpGet("sections/{sectionKey}/articles")]
    public async Task<IActionResult> GetArticles(
        string sectionKey,
        [FromQuery] PaginationParameters pagination,
        CancellationToken ct
    )
    {
        if (!pagination.TryValidate(out string? error))
        {
            return BadRequest(error);
        }

        try
        {
            List<ArticleResponse> articles = await _service.GetArticlesBySectionAsync(
                sectionKey,
                pagination.Skip,
                pagination.Take,
                ct
            );
            
            return Ok(articles);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}