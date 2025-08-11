using ArticlesCatalog.Api.Services.Interfaces;

namespace ArticlesCatalog.Api.Services;

/// <summary>
/// Реализация построителя ключей секций.
/// </summary>
public sealed class SectionKeyBuilder : ISectionKeyBuilder
{
    public string ComputeSectionKey(IEnumerable<int>? tagIds)
    {
        if (tagIds == null)
        {
            return string.Empty;
        }

        List<int> idsList = tagIds
            .Distinct()
            .ToList();
        idsList.Sort();
        
        return string.Join(",", idsList);
    }
}