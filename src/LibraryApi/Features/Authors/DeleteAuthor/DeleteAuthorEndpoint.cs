using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;

namespace LibraryApi.Features.Authors.DeleteAuthor;

public static class DeleteAuthorEndpoint
{
    public static RouteGroupBuilder MapDeleteAuthor(this RouteGroupBuilder group)
    {
        group.MapDelete("/{id:guid}", Handle)
            .WithName("DeleteAuthor")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        return group;
    }

    private static async Task<IResult> Handle(
        Guid id,
        LibraryDbContext db,
        CancellationToken ct)
    {
        var author = await db.Authors.FirstOrDefaultAsync(a => a.Id == id, ct);

        if (author is null)
            return Results.Problem(
                title: "Not Found",
                detail: $"Author with ID '{id}' was not found.",
                statusCode: StatusCodes.Status404NotFound);

        db.Authors.Remove(author);
        await db.SaveChangesAsync(ct); // FK violation caught by GlobalExceptionHandler → 409

        return Results.NoContent();
    }
}
