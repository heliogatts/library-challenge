using FluentValidation;

namespace LibraryApi.Features.Genres.UpdateGenre;

public class UpdateGenreValidator : AbstractValidator<UpdateGenreRequest>
{
    public UpdateGenreValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Genre name is required.")
            .MaximumLength(100).WithMessage("Genre name must not exceed 100 characters.");
    }
}
