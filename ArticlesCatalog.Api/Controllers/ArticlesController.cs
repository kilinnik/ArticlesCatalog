using ArticlesCatalog.Api.Models;
using ArticlesCatalog.Api.Services;
using ArticlesCatalog.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ArticlesCatalog.Api.Controllers;

/// <summary>
/// Предоставляет конечные точки CRUD для статей.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly IArticleService _service;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="ArticlesController"/>.
    /// </summary>
    public ArticlesController(IArticleService service)
    {
        _service = service;
    }

    /// <summary>
    /// Создаёт новую статью.
    /// </summary>
    /// <returns>Созданная статья со ссылкой на ресурс.</returns>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateArticleRequest request,
        CancellationToken ct
    )
    {
        ServiceResult<ArticleResponse> result = await _service.CreateAsync(request, ct);
        if (!result.IsSuccess)
        {
            return ValidationProblem(new ValidationProblemDetails(result.Errors!));
        }

        return CreatedAtAction(nameof(Get), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>
    /// Получает статью по идентификатору.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct)
    {
        ArticleResponse? article = await _service.GetAsync(id, ct);
        if (article == null)
        {
            return NotFound();
        }

        return Ok(article);
    }

    /// <summary>
    /// Обновляет существующую статью.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] CreateArticleRequest request,
        CancellationToken ct
    )
    {
        ServiceResult<ArticleResponse> result = await _service.UpdateAsync(id, request, ct);
        if (result.IsNotFound)
        {
            return NotFound();
        }

        if (!result.IsSuccess)
        {
            return ValidationProblem(new ValidationProblemDetails(result.Errors!));
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Удаляет статью по идентификатору.
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return NoContent();
    }
}