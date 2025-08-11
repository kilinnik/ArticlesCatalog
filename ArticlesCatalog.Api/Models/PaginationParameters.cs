using ArticlesCatalog.Api.Resources;

namespace ArticlesCatalog.Api.Models;

/// <summary>
/// Параметры постраничного вывода.
/// </summary>
public class PaginationParameters
{
    public const int MinSkip = 0;
    public const int MinTake = 1;
    public const int MaxTake = 100;
    public const int DefaultTake = 20;

    /// <summary>
    /// Количество элементов, которое нужно пропустить.
    /// </summary>
    public int Skip { get; init; } = MinSkip;

    /// <summary>
    /// Количество элементов, которое нужно взять.
    /// </summary>
    public int Take { get; init; } = DefaultTake;

    /// <summary>
    /// Проверяет корректность параметров постраничного вывода.
    /// </summary>
    public bool TryValidate(out string? error)
    {
        if (Skip < MinSkip)
        {
            error = ErrorMessages.SkipMustBeNonNegative;
            return false;
        }

        if (Take is < MinTake or > MaxTake)
        {
            error = ErrorMessages.TakeMustBeBetween1And100;
            return false;
        }

        error = null;
        return true;
    }
}
