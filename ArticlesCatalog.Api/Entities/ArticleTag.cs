namespace ArticlesCatalog.Api.Entities;

/// <summary>
/// Сущность-связка, соединяющая <see cref="Article"/> и <see cref="Tag"/> с информацией о порядке.
/// </summary>
public class ArticleTag
{
    /// <summary>
    /// Идентификатор связанной статьи.
    /// </summary>
    public int ArticleId { get; set; }

    /// <summary>
    /// Навигационное свойство связанной статьи.
    /// </summary>
    public Article Article { get; set; } = null!;

    /// <summary>
    /// Идентификатор связанного тега.
    /// </summary>
    public int TagId { get; set; }

    /// <summary>
    /// Навигационное свойство связанного тега.
    /// </summary>
    public Tag Tag { get; set; } = null!;

    /// <summary>
    /// Позиция тега в статье, начиная с нуля.
    /// </summary>
    public int Position { get; set; }
}