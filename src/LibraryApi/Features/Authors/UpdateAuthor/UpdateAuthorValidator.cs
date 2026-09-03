using FluentValidation;

namespace LibraryApi.Features.Authors.UpdateAuthor;

public class UpdateAuthorValidator : AbstractValidator<UpdateAuthorRequest>
{
    public UpdateAuthorValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Author name is required.")
            .MaximumLength(200).WithMessage("Author name must not exceed 200 characters.");
    }
}
