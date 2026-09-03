using FluentValidation;

namespace LibraryApi.Features.Genres.CreateGenre;

public class CreateGenreValidator : AbstractValidator<CreateGenreRequest>
{
    public CreateGenreValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Genre name is required.")
            .MaximumLength(100).WithMessage("Genre name must not exceed 100 characters.");
    }
}
