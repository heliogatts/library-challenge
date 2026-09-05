using FluentValidation;
using LibraryApi.Domain.ValueObjects;

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
            .MaximumLength(17).WithMessage("ISBN must not exceed 17 characters.")
            .Must(Isbn.IsValid).WithMessage("ISBN must be a valid 10 or 13 digit ISBN format (hyphens and spaces allowed).");

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
