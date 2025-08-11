using System.Text.RegularExpressions;
using ArticlesCatalog.Api.Constants;
using ArticlesCatalog.Api.Resources;
using ArticlesCatalog.Api.Services.Interfaces;

namespace ArticlesCatalog.Api.Services;

/// <summary>
/// Реализация нормализатора тегов.
/// </summary>
public sealed class TagNormalizer : ITagNormalizer
{
    private static readonly Regex SpacesRegex = new("\\s+", RegexOptions.Compiled);

    public NormalizationResult Normalize(IEnumerable<string>? tags, bool failOnDuplicate)
    {
        if (tags == null)
        {
            return new NormalizationResult
            {
                IsSuccess = false,
                Error = ErrorMessages.TagsCollectionCannotBeNull,
            };
        }

        List<string> normalizedTags = [];
        HashSet<string> seenTags = new(StringComparer.OrdinalIgnoreCase);

        foreach (string tag in tags)
        {
            if (tag == null)
            {
                return new NormalizationResult
                {
                    IsSuccess = false,
                    Error = ErrorMessages.TagCannotBeNull,
                };
            }

            string trimmedTag = tag.Trim();
            string collapsedTag = SpacesRegex.Replace(trimmedTag, " ");
            if (collapsedTag.Length == 0)
            {
                return new NormalizationResult
                {
                    IsSuccess = false,
                    Error = ErrorMessages.TagCannotBeEmpty,
                };
            }
            if (collapsedTag.Length > Limits.MaxTagLength)
            {
                return new NormalizationResult
                {
                    IsSuccess = false,
                    Error = string.Format(ErrorMessages.TagLengthCannotExceed, Limits.MaxTagLength),
                };
            }

            if (!seenTags.Add(collapsedTag))
            {
                if (failOnDuplicate)
                {
                    return new NormalizationResult
                    {
                        IsSuccess = false,
                        Error = ErrorMessages.TagsMustNotContainDuplicates,
                    };
                }
                continue;
            }

            normalizedTags.Add(collapsedTag);
        }

        if (normalizedTags.Count > Limits.MaxTagsPerArticle)
        {
            return new NormalizationResult
            {
                IsSuccess = false,
                Error = string.Format(ErrorMessages.TagsCountCannotExceed, Limits.MaxTagsPerArticle),
            };
        }

        return new NormalizationResult
        {
            IsSuccess = true,
            NormalizedTags = normalizedTags
        };
    }
}