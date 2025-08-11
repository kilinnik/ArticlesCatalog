using ArticlesCatalog.Api.Persistence;
using ArticlesCatalog.Api.Services;
using ArticlesCatalog.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.PostgreSql;

namespace ArticlesCatalog.Tests;

public class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container;
    public AppDbContext DbContext { get; private set; } = null!;
    public IArticleService ArticleService { get; private set; } = null!;
    public ICatalogService CatalogService { get; private set; } = null!;

    public PostgresFixture()
    {
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("postgres")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        string connectionString = _container.GetConnectionString();
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        DbContext = new AppDbContext(options);
        await DbContext.Database.MigrateAsync();

        TagService tagService = new TagService(DbContext);
        SectionKeyBuilder sectionBuilder = new SectionKeyBuilder();
        ArticleService = new ArticleService(DbContext, sectionBuilder, tagService, NullLogger<ArticleService>.Instance);
        CatalogService = new CatalogService(DbContext, sectionBuilder, NullLogger<CatalogService>.Instance);
    }

    public async Task DisposeAsync()
    {
        if (DbContext != null)
        {
            await DbContext.DisposeAsync();
        }

        await _container.DisposeAsync();
    }
}