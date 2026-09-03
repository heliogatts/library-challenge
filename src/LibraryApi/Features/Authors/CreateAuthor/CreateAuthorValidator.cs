using FluentValidation;

namespace LibraryApi.Features.Authors.CreateAuthor;

public class CreateAuthorValidator : AbstractValidator<CreateAuthorRequest>
{
    public CreateAuthorValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Author name is required.")
            .MaximumLength(200).WithMessage("Author name must not exceed 200 characters.");
    }
}
