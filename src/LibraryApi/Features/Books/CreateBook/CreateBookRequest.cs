namespace LibraryApi.Features.Books.CreateBook;

public record CreateBookRequest(
    string Title,
    string ISBN,
    int PublishedYear,
    string? Description,
    Guid AuthorId,
    Guid GenreId);
