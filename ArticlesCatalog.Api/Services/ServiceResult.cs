namespace ArticlesCatalog.Api.Services;

/// <summary>
/// Представляет результат операции сервиса с возможным значением или ошибками валидации.
/// </summary>
public class ServiceResult<T>
{
    /// <summary>
    /// Возвращаемое значение операции при успешном выполнении.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Ошибки валидации, сгруппированные по имени поля, если операция не прошла проверку.
    /// </summary>
    public Dictionary<string, string[]>? Errors { get; }

    /// <summary>
    /// Показывает, что запрашиваемая сущность не найдена.
    /// </summary>
    public bool IsNotFound { get; }

    /// <summary>
    /// Флаг для проверки успешности операции.
    /// </summary>
    public bool IsSuccess => Errors == null && !IsNotFound;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="ServiceResult{T}"/>.
    /// </summary>
    private ServiceResult(T? value, Dictionary<string, string[]>? errors, bool isNotFound)
    {
        Value = value;
        Errors = errors;
        IsNotFound = isNotFound;
    }

    /// <summary>
    /// Создаёт успешный результат с указанным значением.
    /// </summary>
    public static ServiceResult<T> Success(T value) => new(value, null, false);

    /// <summary>
    /// Создаёт результат валидации с переданными ошибками.
    /// </summary>
    private static ServiceResult<T> Validation(Dictionary<string, string[]> errors) => new(default, errors, false);

    /// <summary>
    /// Создаёт результат валидации для одного поля и сообщения.
    /// </summary>
    public static ServiceResult<T> Validation(string field, string error) =>
        Validation(new Dictionary<string, string[]> { [field] = [error] });

    /// <summary>
    /// Создаёт результат, указывающий на отсутствие ресурса.
    /// </summary>
    public static ServiceResult<T> NotFound() => new(default, null, true);
}