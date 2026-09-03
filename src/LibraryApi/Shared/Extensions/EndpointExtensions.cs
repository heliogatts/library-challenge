using LibraryApi.Features.Genres.CreateGenre;
using LibraryApi.Features.Genres.GetGenres;
using LibraryApi.Features.Genres.GetGenreById;
using LibraryApi.Features.Genres.UpdateGenre;
using LibraryApi.Features.Genres.DeleteGenre;
using LibraryApi.Features.Authors.CreateAuthor;
using LibraryApi.Features.Authors.GetAuthors;
using LibraryApi.Features.Authors.GetAuthorById;
using LibraryApi.Features.Authors.UpdateAuthor;
using LibraryApi.Features.Authors.DeleteAuthor;
using LibraryApi.Features.Books.CreateBook;
using LibraryApi.Features.Books.GetBooks;
using LibraryApi.Features.Books.GetBookById;
using LibraryApi.Features.Books.UpdateBook;
using LibraryApi.Features.Books.DeleteBook;

namespace LibraryApi.Shared.Extensions;

public static class EndpointExtensions
{
    public static WebApplication MapFeatureEndpoints(this WebApplication app)
    {
        var genres = app.MapGroup("/api/genres").WithTags("Genres");
        genres.MapCreateGenre();
        genres.MapGetGenres();
        genres.MapGetGenreById();
        genres.MapUpdateGenre();
        genres.MapDeleteGenre();

        var authors = app.MapGroup("/api/authors").WithTags("Authors");
        authors.MapCreateAuthor();
        authors.MapGetAuthors();
        authors.MapGetAuthorById();
        authors.MapUpdateAuthor();
        authors.MapDeleteAuthor();

        var books = app.MapGroup("/api/books").WithTags("Books");
        books.MapCreateBook();
        books.MapGetBooks();
        books.MapGetBookById();
        books.MapUpdateBook();
        books.MapDeleteBook();

        return app;
    }
}
