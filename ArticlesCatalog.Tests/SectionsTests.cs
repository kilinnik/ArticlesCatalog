using ArticlesCatalog.Api.Models;
using ArticlesCatalog.Api.Services;
using ArticlesCatalog.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ArticlesCatalog.Tests;

public class SectionsTests(PostgresFixture fixture) : IClassFixture<PostgresFixture>
{
    [Fact]
    public async Task Section_is_removed_after_deleting_last_article()
    {
        await fixture.DbContext.Database.ExecuteSqlRawAsync(
            "TRUNCATE \"ArticleTags\", \"Articles\", \"Tags\" RESTART IDENTITY CASCADE;");
        fixture.DbContext.ChangeTracker.Clear();

        IArticleService articleService = fixture.ArticleService;
        ICatalogService catalogService = fixture.CatalogService;

        ServiceResult<ArticleResponse> createResult = await articleService.CreateAsync(new CreateArticleRequest
        {
            Title = "Title",
            Tags = ["tag"]
        }, CancellationToken.None);
        Assert.True(createResult.IsSuccess);
        int articleId = createResult.Value!.Id;

        List<SectionResponse> sections = await catalogService.GetSectionsAsync(0, 10, CancellationToken.None);
        SectionResponse section = Assert.Single(sections);

        await articleService.DeleteAsync(articleId, CancellationToken.None);

        sections = await catalogService.GetSectionsAsync(0, 10, CancellationToken.None);
        Assert.Empty(sections);
    }

    [Fact]
    public async Task Sections_are_sorted_by_articles_count_desc()
    {
        await fixture.DbContext.Database.ExecuteSqlRawAsync(
            "TRUNCATE \"ArticleTags\", \"Articles\", \"Tags\" RESTART IDENTITY CASCADE;");
        fixture.DbContext.ChangeTracker.Clear();

        IArticleService articleService = fixture.ArticleService;
        ICatalogService catalogService = fixture.CatalogService;

        await articleService.CreateAsync(new CreateArticleRequest
        {
            Title = "First",
            Tags = ["one"]
        }, CancellationToken.None);

        await articleService.CreateAsync(new CreateArticleRequest
        {
            Title = "Second",
            Tags = ["two"]
        }, CancellationToken.None);

        await articleService.CreateAsync(new CreateArticleRequest
        {
            Title = "Third",
            Tags = ["two"]
        }, CancellationToken.None);

        List<SectionResponse> sections = await catalogService.GetSectionsAsync(0, 10, CancellationToken.None);
        Assert.Equal([2, 1], sections.Select(s => s.ArticlesCount).ToList());
    }
}