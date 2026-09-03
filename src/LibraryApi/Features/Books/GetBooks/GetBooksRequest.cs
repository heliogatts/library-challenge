using LibraryApi.Shared.Models;

namespace LibraryApi.Features.Books.GetBooks;

public record GetBooksRequest : PagedRequest
{
    public Guid? GenreId { get; init; }
    public Guid? AuthorId { get; init; }
}
