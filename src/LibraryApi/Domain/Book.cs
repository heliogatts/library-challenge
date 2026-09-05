using LibraryApi.Domain.Exceptions;
using LibraryApi.Domain.ValueObjects;

namespace LibraryApi.Domain;

public class Book
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public Isbn ISBN { get; private set; } = null!;
    public int PublishedYear { get; private set; }
    public string? Description { get; private set; }
    public Guid AuthorId { get; private set; }
    public Guid GenreId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public Author Author { get; private set; } = null!;
    public Genre Genre { get; private set; } = null!;

    // EF Core parameterless constructor
    private Book() { }

    public static Book Create(
        string title,
        Isbn isbn,
        int publishedYear,
        string? description,
        Guid authorId,
        Guid genreId,
        Guid? id = null)
    {
        ValidateTitle(title);
        ValidatePublishedYear(publishedYear);
        ValidateDescription(description);
        ValidateReferences(authorId, genreId);
        ArgumentNullException.ThrowIfNull(isbn, nameof(isbn));

        return new Book
        {
            Id = id ?? Guid.NewGuid(),
            Title = title.Trim(),
            ISBN = isbn,
            PublishedYear = publishedYear,
            Description = description?.Trim(),
            AuthorId = authorId,
            GenreId = genreId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateDetails(
        string title,
        Isbn isbn,
        int publishedYear,
        string? description,
        Guid authorId,
        Guid genreId)
    {
        ValidateTitle(title);
        ValidatePublishedYear(publishedYear);
        ValidateDescription(description);
        ValidateReferences(authorId, genreId);
        ArgumentNullException.ThrowIfNull(isbn, nameof(isbn));

        Title = title.Trim();
        ISBN = isbn;
        PublishedYear = publishedYear;
        Description = description?.Trim();
        AuthorId = authorId;
        GenreId = genreId;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Book title cannot be empty.");

        if (title.Trim().Length > 200)
            throw new DomainException("Book title cannot exceed 200 characters.");
    }

    private static void ValidatePublishedYear(int publishedYear)
    {
        if (publishedYear < 1450 || publishedYear > DateTime.UtcNow.Year)
            throw new DomainException($"Published year must be between 1450 and {DateTime.UtcNow.Year}.");
    }

    private static void ValidateDescription(string? description)
    {
        if (description != null && description.Trim().Length > 2000)
            throw new DomainException("Description cannot exceed 2000 characters.");
    }

    private static void ValidateReferences(Guid authorId, Guid genreId)
    {
        if (authorId == Guid.Empty)
            throw new DomainException("Author ID cannot be empty.");

        if (genreId == Guid.Empty)
            throw new DomainException("Genre ID cannot be empty.");
    }
}
