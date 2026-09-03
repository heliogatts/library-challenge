namespace LibraryApi.Features.Books.UpdateBook;

public record UpdateBookRequest(
    string Title,
    string ISBN,
    int PublishedYear,
    string? Description,
    Guid AuthorId,
    Guid GenreId);
