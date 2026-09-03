using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;

namespace LibraryApi.Features.Genres.DeleteGenre;

public static class DeleteGenreEndpoint
{
    public static RouteGroupBuilder MapDeleteGenre(this RouteGroupBuilder group)
    {
        group.MapDelete("/{id:guid}", Handle)
            .WithName("DeleteGenre")
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
        var genre = await db.Genres.FirstOrDefaultAsync(g => g.Id == id, ct);

        if (genre is null)
            return Results.Problem(
                title: "Not Found",
                detail: $"Genre with ID '{id}' was not found.",
                statusCode: StatusCodes.Status404NotFound);

        db.Genres.Remove(genre);
        await db.SaveChangesAsync(ct); // FK violation caught by GlobalExceptionHandler → 409

        return Results.NoContent();
    }
}
