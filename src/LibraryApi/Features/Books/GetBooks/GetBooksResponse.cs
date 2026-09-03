using LibraryApi.Shared.Models;

namespace LibraryApi.Features.Books.GetBooks;

public record GetBooksResponseItem(
    Guid Id,
    string Title,
    string ISBN,
    int PublishedYear,
    string AuthorName,
    string GenreName);
