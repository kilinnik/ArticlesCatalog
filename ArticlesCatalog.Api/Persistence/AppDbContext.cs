using ArticlesCatalog.Api.Constants;
using ArticlesCatalog.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArticlesCatalog.Api.Persistence;

/// <summary>
/// Контекст базы данных Entity Framework для приложения.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Создаёт новый <see cref="AppDbContext"/>, настроенный с указанными параметрами.
    /// </summary>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Набор тегов.
    /// </summary>
    public DbSet<Tag> Tags { get; set; } = null!;

    /// <summary>
    /// Набор статей.
    /// </summary>
    public DbSet<Article> Articles { get; set; } = null!;

    /// <summary>
    /// Набор связей статей и тегов с позициями.
    /// </summary>
    public DbSet<ArticleTag> ArticleTags { get; set; } = null!;


    /// <summary>
    /// Настраивает отображение сущностей и их связи.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("Tags");

            entity.HasKey(tag => tag.Id);

            entity.Property(tag => tag.Id)
                .ValueGeneratedOnAdd();

            entity.Property(tag => tag.Name)
                .IsRequired()
                .HasMaxLength(Limits.MaxTagLength);

            entity.Property(tag => tag.NormalizedName)
                .HasColumnName("NormalizedName")
                .HasComputedColumnSql("lower(\"Name\")", stored: true);

            entity.HasIndex(tag => tag.NormalizedName)
                .IsUnique();
        });

        modelBuilder.Entity<Article>(entity =>
        {
            entity.ToTable("Articles");

            entity.HasKey(article => article.Id);

            entity.Property(article => article.Title)
                .IsRequired()
                .HasMaxLength(Limits.MaxArticleTitleLength);

            entity.Property(article => article.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamptz");

            entity.Property(article => article.UpdatedAt)
                .HasColumnType("timestamptz");

            entity.Property(article => article.SectionKey)
                .IsRequired()
                .HasColumnType("text");

            entity.HasIndex(article => article.SectionKey);

            entity.HasMany(article => article.ArticleTags)
                .WithOne(articleTag => articleTag.Article)
                .HasForeignKey(articleTag => articleTag.ArticleId);
        });

        modelBuilder.Entity<ArticleTag>(entity =>
        {
            entity.ToTable("ArticleTags",
                table => { table.HasCheckConstraint("CK_ArticleTag_Position", "\"Position\" >= 0"); });

            entity.HasKey(articleTag => new { articleTag.ArticleId, articleTag.TagId });

            entity.Property(articleTag => articleTag.Position)
                .IsRequired();

            entity.HasOne(articleTag => articleTag.Article)
                .WithMany(article => article.ArticleTags)
                .HasForeignKey(articleTag => articleTag.ArticleId);

            entity.HasOne(articleTag => articleTag.Tag)
                .WithMany(tag => tag.ArticleTags)
                .HasForeignKey(articleTag => articleTag.TagId);

            entity.HasIndex(articleTag => new { articleTag.ArticleId, articleTag.Position })
                .IsUnique();

            entity.HasIndex(articleTag => new { articleTag.ArticleId, articleTag.TagId })
                .IsUnique();
        });
    }
}