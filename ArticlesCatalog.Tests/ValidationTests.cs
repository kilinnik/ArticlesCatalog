using ArticlesCatalog.Api.Constants;
using ArticlesCatalog.Api.Models;
using ArticlesCatalog.Api.Models.Validators;
using ArticlesCatalog.Api.Resources;
using ArticlesCatalog.Api.Services;
using FluentValidation.Results;

namespace ArticlesCatalog.Tests;

public class ValidationTests
{
    private readonly CreateArticleRequestValidator _validator = new(new TagNormalizer());

    [Fact]
    public void Title_length_exceeds_limit()
    {
        CreateArticleRequest request = new()
        {
            Title = new string('a', Limits.MaxArticleTitleLength + 1),
            Tags = []
        };

        ValidationResult result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == string.Format(
            ErrorMessages.TitleLengthCannotExceed, Limits.MaxArticleTitleLength));
    }

    [Fact]
    public void Tag_length_exceeds_limit()
    {
        CreateArticleRequest request = new()
        {
            Title = "Test",
            Tags = [new string('a', Limits.MaxTagLength + 1)]
        };

        ValidationResult result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == string.Format(
            ErrorMessages.TagLengthCannotExceed, Limits.MaxTagLength));
    }

    [Fact]
    public void Too_many_tags()
    {
        List<string> tags = Enumerable.Range(0, Limits.MaxTagsPerArticle + 1)
            .Select(i => $"tag{i}")
            .ToList();
        CreateArticleRequest request = new()
        {
            Title = "Test",
            Tags = tags
        };

        ValidationResult result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == string.Format(
            ErrorMessages.TagsCountCannotExceed, Limits.MaxTagsPerArticle));
    }

    [Fact]
    public void Duplicate_tags_with_different_casing()
    {
        CreateArticleRequest request = new()
        {
            Title = "Test",
            Tags = ["Go", "go"]
        };

        ValidationResult result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ErrorMessages.TagsMustNotContainDuplicates);
    }

    [Fact]
    public void Tags_collection_is_null()
    {
        CreateArticleRequest request = new()
        {
            Title = "Test",
            Tags = null!
        };

        ValidationResult result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ErrorMessages.TagsCollectionCannotBeNull);
    }

    [Fact]
    public void Tag_is_null()
    {
        CreateArticleRequest request = new()
        {
            Title = "Test",
            Tags = [null!]
        };

        ValidationResult result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ErrorMessages.TagCannotBeNull);
    }

    [Fact]
    public void Tag_is_empty()
    {
        CreateArticleRequest request = new()
        {
            Title = "Test",
            Tags = [" "]
        };

        ValidationResult result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ErrorMessages.TagCannotBeEmpty);
    }
}