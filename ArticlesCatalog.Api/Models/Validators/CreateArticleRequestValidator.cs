using FluentValidation;
using ArticlesCatalog.Api.Resources;
using ArticlesCatalog.Api.Constants;
using ArticlesCatalog.Api.Services;
using ArticlesCatalog.Api.Services.Interfaces;

namespace ArticlesCatalog.Api.Models.Validators;

/// <summary>
/// Валидатор для <see cref="CreateArticleRequest"/>.
/// </summary>
public class CreateArticleRequestValidator : AbstractValidator<CreateArticleRequest>
{
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="CreateArticleRequestValidator"/>.
    /// </summary>
    /// <param name="tagNormalizer">Сервис для нормализации и проверки тегов.</param>
    public CreateArticleRequestValidator(ITagNormalizer tagNormalizer)
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(ErrorMessages.TitleIsRequired)
            .MaximumLength(Limits.MaxArticleTitleLength)
            .WithMessage(string.Format(ErrorMessages.TitleLengthCannotExceed, Limits.MaxArticleTitleLength))
            .WithName(FieldNames.Title);

        RuleFor(x => x.Tags)
            .NotNull().WithMessage(ErrorMessages.TagsCollectionCannotBeNull)
            .OverridePropertyName(FieldNames.Tags)
            .Custom((tags, context) =>
            {
                NormalizationResult norm = tagNormalizer.Normalize(tags!, failOnDuplicate: true);
                if (!norm.IsSuccess)
                {
                    context.AddFailure(norm.Error!);
                    return;
                }

                context.InstanceToValidate.Tags = norm.NormalizedTags;
            });

        RuleForEach(x => x.Tags)
            .MaximumLength(Limits.MaxTagLength)
            .WithMessage(string.Format(ErrorMessages.TagLengthCannotExceed, Limits.MaxTagLength))
            .OverridePropertyName(FieldNames.Tags);
    }
}
