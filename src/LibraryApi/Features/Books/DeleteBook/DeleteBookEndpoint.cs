using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;

namespace LibraryApi.Features.Books.DeleteBook;

public static class DeleteBookEndpoint
{
    public static RouteGroupBuilder MapDeleteBook(this RouteGroupBuilder group)
    {
        group.MapDelete("/{id:guid}", Handle)
            .WithName("DeleteBook")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }

    private static async Task<IResult> Handle(
        Guid id,
        LibraryDbContext db,
        CancellationToken ct)
    {
        var book = await db.Books.FirstOrDefaultAsync(b => b.Id == id, ct);

        if (book is null)
            return Results.Problem(
                title: "Not Found",
                detail: $"Book with ID '{id}' was not found.",
                statusCode: StatusCodes.Status404NotFound);

        db.Books.Remove(book);
        await db.SaveChangesAsync(ct);

        return Results.NoContent();
    }
}
