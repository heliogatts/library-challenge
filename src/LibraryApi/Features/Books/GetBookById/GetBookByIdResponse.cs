namespace LibraryApi.Features.Books.GetBookById;

public record GetBookByIdResponse(
    Guid Id,
    string Title,
    string ISBN,
    int PublishedYear,
    string? Description,
    Guid AuthorId,
    string AuthorName,
    Guid GenreId,
    string GenreName);
