using System.Resources;

namespace ArticlesCatalog.Api.Resources;

public static class ErrorMessages
{
    private static readonly ResourceManager ResourceManager = new("ArticlesCatalog.Api.Resources.ErrorMessages", typeof(ErrorMessages).Assembly);

    public static string ArticleTooManyTags => ResourceManager.GetString(nameof(ArticleTooManyTags))!;
    public static string SkipMustBeNonNegative => ResourceManager.GetString(nameof(SkipMustBeNonNegative))!;
    public static string TakeMustBeBetween1And100 => ResourceManager.GetString(nameof(TakeMustBeBetween1And100))!;
    public static string TagsCollectionCannotBeNull => ResourceManager.GetString(nameof(TagsCollectionCannotBeNull))!;
    public static string TagCannotBeNull => ResourceManager.GetString(nameof(TagCannotBeNull))!;
    public static string TagCannotBeEmpty => ResourceManager.GetString(nameof(TagCannotBeEmpty))!;
    public static string TagsMustNotContainDuplicates => ResourceManager.GetString(nameof(TagsMustNotContainDuplicates))!;
    public static string TagLengthCannotExceed => ResourceManager.GetString(nameof(TagLengthCannotExceed))!;
    public static string TagsCountCannotExceed => ResourceManager.GetString(nameof(TagsCountCannotExceed))!;
    public static string TitleIsRequired => ResourceManager.GetString(nameof(TitleIsRequired))!;
    public static string TitleLengthCannotExceed => ResourceManager.GetString(nameof(TitleLengthCannotExceed))!;
    public static string InvalidSectionKeyFormat => ResourceManager.GetString(nameof(InvalidSectionKeyFormat))!;
}