using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;

namespace LibraryApi.Features.Books.GetBookById;

public static class GetBookByIdEndpoint
{
    public static RouteGroupBuilder MapGetBookById(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", Handle)
            .WithName("GetBookById")
            .Produces<GetBookByIdResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }

    private static async Task<IResult> Handle(
        Guid id,
        LibraryDbContext db,
        CancellationToken ct)
    {
        var book = await db.Books
            .Where(b => b.Id == id)
            .Select(b => new GetBookByIdResponse(
                b.Id,
                b.Title,
                b.ISBN,
                b.PublishedYear,
                b.Description,
                b.AuthorId,
                b.Author.Name,
                b.GenreId,
                b.Genre.Name))
            .FirstOrDefaultAsync(ct);

        return book is null
            ? Results.Problem(
                title: "Not Found",
                detail: $"Book with ID '{id}' was not found.",
                statusCode: StatusCodes.Status404NotFound)
            : Results.Ok(book);
    }
}
