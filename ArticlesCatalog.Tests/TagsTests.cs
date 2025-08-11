using ArticlesCatalog.Api.Models;
using ArticlesCatalog.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ArticlesCatalog.Tests;

public class TagsTests(PostgresFixture fixture) : IClassFixture<PostgresFixture>
{
    [Fact]
    public async Task Tags_are_case_insensitive_unique()
    {
        await fixture.DbContext.Database.ExecuteSqlRawAsync(
            "TRUNCATE \"ArticleTags\", \"Articles\", \"Tags\" RESTART IDENTITY CASCADE;");
        fixture.DbContext.ChangeTracker.Clear();

        IArticleService articleService = fixture.ArticleService;

        await articleService.CreateAsync(new CreateArticleRequest
        {
            Title = "First",
            Tags = ["Go"]
        }, CancellationToken.None);

        await articleService.CreateAsync(new CreateArticleRequest
        {
            Title = "Second",
            Tags = ["go"]
        }, CancellationToken.None);

        int tagsCount = await fixture.DbContext.Tags.CountAsync();
        Assert.Equal(1, tagsCount);
    }
}