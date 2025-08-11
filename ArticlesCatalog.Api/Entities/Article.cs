using ArticlesCatalog.Api.Constants;
using ArticlesCatalog.Api.Resources;

namespace ArticlesCatalog.Api.Entities;

/// <summary>
/// Представляет статью, сохранённую в каталоге.
/// </summary>
public class Article
{
    /// <summary>
    /// Идентификатор статьи в базе данных.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Заголовок статьи.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Время создания в UTC.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Время последнего изменения в UTC.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Вычисляемый ключ секции (упорядоченные идентификаторы тегов).
    /// </summary>
    public string SectionKey { get; set; } = string.Empty;

    /// <summary>
    /// Теги статьи с указанием порядка.
    /// </summary>
    public ICollection<ArticleTag> ArticleTags { get; set; } = new List<ArticleTag>();

    /// <summary>
    /// Связывает тег со статьёй на указанной позиции, контролируя лимит и уникальность.
    /// </summary>
    /// <param name="tag">Сущность тега, которую нужно присоединить.</param>
    /// <param name="position">Позиция тега начиная с нуля.</param>
    public void AddTag(Tag tag, int position)
    {
        if (ArticleTags.Count >= Limits.MaxTagsPerArticle)
        {
            throw new InvalidOperationException(string.Format(ErrorMessages.ArticleTooManyTags,
                Limits.MaxTagsPerArticle));
        }

        bool exists = ArticleTags.Any(articleTag => articleTag.Tag == tag);
        if (exists)
        {
            return;
        }

        ArticleTag articleTagToAdd = new ArticleTag
        {
            Article = this,
            Tag = tag,
            Position = position
        };

        ArticleTags.Add(articleTagToAdd);
    }
}