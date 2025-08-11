namespace ArticlesCatalog.Api.Entities;

/// <summary>
/// Представляет метку, которую можно связать со <see cref="Article"/>.
/// </summary>
public class Tag
{
    /// <summary>
    /// Уникальный идентификатор тега.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Отображаемое имя тега, указанное пользователем.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Нормализованное имя в нижнем регистре для сравнений.
    /// Значение заполняется базой данных и недоступно для изменения извне.
    /// </summary>
    public string NormalizedName { get; internal set; } = string.Empty;

    /// <summary>
    /// Коллекция связей этого тега со статьями.
    /// </summary>
    public ICollection<ArticleTag> ArticleTags { get; set; } = new List<ArticleTag>();
}