using ArticlesCatalog.Api.Models;
using ArticlesCatalog.Api.Services;
using ArticlesCatalog.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ArticlesCatalog.Tests;

public class IntegrationTests(PostgresFixture fixture) : IClassFixture<PostgresFixture>
{
    [Fact]
    public async Task Crud_and_catalog_flow()
    {
        await fixture.DbContext.Database.ExecuteSqlRawAsync(
            "TRUNCATE \"ArticleTags\", \"Articles\", \"Tags\" RESTART IDENTITY CASCADE;");
        fixture.DbContext.ChangeTracker.Clear();

        IArticleService articleService = fixture.ArticleService;
        ICatalogService catalogService = fixture.CatalogService;

        CreateArticleRequest createRequest = new CreateArticleRequest
        {
            Title = "Test",
            Tags = ["tag1", "tag2"]
        };

        ServiceResult<ArticleResponse> createResult =
            await articleService.CreateAsync(createRequest, CancellationToken.None);
        Assert.True(createResult.IsSuccess);
        int articleId = createResult.Value!.Id;

        CreateArticleRequest updateRequest = new CreateArticleRequest
        {
            Title = "Updated",
            Tags = ["tag2"]
        };
        ServiceResult<ArticleResponse> updateResult =
            await articleService.UpdateAsync(articleId, updateRequest, CancellationToken.None);
        Assert.True(updateResult.IsSuccess);
        List<SectionResponse> sections = await catalogService.GetSectionsAsync(0, 10, CancellationToken.None);
        Assert.Single(sections);
        SectionResponse section = sections[0];
        List<ArticleResponse> articles =
            await catalogService.GetArticlesBySectionAsync(section.SectionKey, 0, 10, CancellationToken.None);
        Assert.Single(articles);
        Assert.Equal("Updated", articles[0].Title);

        await articleService.DeleteAsync(articleId, CancellationToken.None);
        ArticleResponse? deleted = await articleService.GetAsync(articleId, CancellationToken.None);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task Article_without_tags_appears_in_without_tags_section()
    {
        await fixture.DbContext.Database.ExecuteSqlRawAsync(
            "TRUNCATE \"ArticleTags\", \"Articles\", \"Tags\" RESTART IDENTITY CASCADE;");
        fixture.DbContext.ChangeTracker.Clear();

        IArticleService articleService = fixture.ArticleService;
        ICatalogService catalogService = fixture.CatalogService;

        ServiceResult<ArticleResponse> createResult = await articleService.CreateAsync(new CreateArticleRequest
        {
            Title = "No tags",
            Tags = []
        }, CancellationToken.None);
        Assert.True(createResult.IsSuccess);
        int articleId = createResult.Value!.Id;

        List<SectionResponse> sections = await catalogService.GetSectionsAsync(0, 10, CancellationToken.None);
        SectionResponse section = Assert.Single(sections);
        Assert.Equal("Без тегов", section.Name);

        List<ArticleResponse> articles =
            await catalogService.GetArticlesBySectionAsync(section.SectionKey, 0, 10, CancellationToken.None);
        ArticleResponse article = Assert.Single(articles);
        Assert.Equal("No tags", article.Title);

        await articleService.DeleteAsync(articleId, CancellationToken.None);
    }

    [Fact]
    public async Task Articles_are_sorted_by_updated_and_created_dates()
    {
        await fixture.DbContext.Database.ExecuteSqlRawAsync(
            "TRUNCATE \"ArticleTags\", \"Articles\", \"Tags\" RESTART IDENTITY CASCADE;");
        fixture.DbContext.ChangeTracker.Clear();

        IArticleService articleService = fixture.ArticleService;
        ICatalogService catalogService = fixture.CatalogService;

        ServiceResult<ArticleResponse> first = await articleService.CreateAsync(new CreateArticleRequest
        {
            Title = "First",
            Tags = ["tag1"]
        }, CancellationToken.None);
        Assert.True(first.IsSuccess);

        await Task.Delay(10);

        ServiceResult<ArticleResponse> second = await articleService.CreateAsync(new CreateArticleRequest
        {
            Title = "Second",
            Tags = ["tag1"]
        }, CancellationToken.None);
        Assert.True(second.IsSuccess);
        List<SectionResponse> sections = await catalogService.GetSectionsAsync(0, 10, CancellationToken.None);
        string sectionKey = sections[0].SectionKey;

        List<ArticleResponse> articles =
            await catalogService.GetArticlesBySectionAsync(sectionKey, 0, 10, CancellationToken.None);
        Assert.Equal([second.Value!.Id, first.Value!.Id], articles.Select(a => a.Id));

        await Task.Delay(10);

        await articleService.UpdateAsync(first.Value!.Id, new CreateArticleRequest
        {
            Title = "First Updated",
            Tags = ["tag1"]
        }, CancellationToken.None);

        articles = await catalogService.GetArticlesBySectionAsync(sectionKey, 0, 10, CancellationToken.None);
        Assert.Equal([first.Value!.Id, second.Value!.Id], articles.Select(a => a.Id));
    }

    [Fact]
    public async Task Articles_with_same_new_tags_share_section()
    {
        await fixture.DbContext.Database.ExecuteSqlRawAsync(
            "TRUNCATE \"ArticleTags\", \"Articles\", \"Tags\" RESTART IDENTITY CASCADE;");
        fixture.DbContext.ChangeTracker.Clear();

        IArticleService articleService = fixture.ArticleService;
        ICatalogService catalogService = fixture.CatalogService;

        List<string> tags = ["new1", "new2"];

        ServiceResult<ArticleResponse> first = await articleService.CreateAsync(new CreateArticleRequest
        {
            Title = "First",
            Tags = tags
        }, CancellationToken.None);
        Assert.True(first.IsSuccess);

        ServiceResult<ArticleResponse> second = await articleService.CreateAsync(new CreateArticleRequest
        {
            Title = "Second",
            Tags = tags
        }, CancellationToken.None);
        Assert.True(second.IsSuccess);

        List<SectionResponse> sections = await catalogService.GetSectionsAsync(0, 10, CancellationToken.None);
        SectionResponse section = Assert.Single(sections);
        Assert.Equal(2, section.ArticlesCount);

        List<ArticleResponse> articles =
            await catalogService.GetArticlesBySectionAsync(section.SectionKey, 0, 10, CancellationToken.None);
        Assert.Equal(2, articles.Count);
    }
}