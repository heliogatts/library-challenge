using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;
using LibraryApi.Shared.Filters;

namespace LibraryApi.Features.Books.UpdateBook;

public static class UpdateBookEndpoint
{
    public static RouteGroupBuilder MapUpdateBook(this RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}", Handle)
            .AddEndpointFilter<ValidationFilter<UpdateBookRequest>>()
            .WithName("UpdateBook")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        return group;
    }

    private static async Task<IResult> Handle(
        Guid id,
        UpdateBookRequest request,
        LibraryDbContext db,
        CancellationToken ct)
    {
        var book = await db.Books.FirstOrDefaultAsync(b => b.Id == id, ct);

        if (book is null)
            return Results.Problem(
                title: "Not Found",
                detail: $"Book with ID '{id}' was not found.",
                statusCode: StatusCodes.Status404NotFound);

        var authorExists = await db.Authors.AnyAsync(a => a.Id == request.AuthorId, ct);
        var genreExists = await db.Genres.AnyAsync(g => g.Id == request.GenreId, ct);

        if (!authorExists || !genreExists)
            return Results.Problem(
                title: "Invalid Reference",
                detail: "The specified Author or Genre does not exist.",
                statusCode: StatusCodes.Status422UnprocessableEntity);

        book.Title = request.Title.Trim();
        book.ISBN = request.ISBN.Trim();
        book.PublishedYear = request.PublishedYear;
        book.Description = request.Description?.Trim();
        book.AuthorId = request.AuthorId;
        book.GenreId = request.GenreId;

        await db.SaveChangesAsync(ct);

        return Results.NoContent();
    }
}
