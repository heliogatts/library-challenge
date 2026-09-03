using FluentValidation;

namespace LibraryApi.Features.Books.CreateBook;

public class CreateBookValidator : AbstractValidator<CreateBookRequest>
{
    public CreateBookValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.ISBN)
            .NotEmpty().WithMessage("ISBN is required.")
            .MaximumLength(13).WithMessage("ISBN must not exceed 13 characters.");

        RuleFor(x => x.PublishedYear)
            .InclusiveBetween(1450, DateTime.UtcNow.Year)
            .WithMessage($"Published year must be between 1450 and {DateTime.UtcNow.Year}.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

        RuleFor(x => x.AuthorId)
            .NotEmpty().WithMessage("Author is required.");

        RuleFor(x => x.GenreId)
            .NotEmpty().WithMessage("Genre is required.");
    }
}
