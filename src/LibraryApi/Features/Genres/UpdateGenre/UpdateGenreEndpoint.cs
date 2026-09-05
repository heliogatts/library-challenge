using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;
using LibraryApi.Shared.Filters;

namespace LibraryApi.Features.Genres.UpdateGenre;

public static class UpdateGenreEndpoint
{
    public static RouteGroupBuilder MapUpdateGenre(this RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}", Handle)
            .AddEndpointFilter<ValidationFilter<UpdateGenreRequest>>()
            .WithName("UpdateGenre")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        return group;
    }

    private static async Task<IResult> Handle(
        Guid id,
        UpdateGenreRequest request,
        LibraryDbContext db,
        CancellationToken ct)
    {
        var genre = await db.Genres.FirstOrDefaultAsync(g => g.Id == id, ct);

        if (genre is null)
            return Results.Problem(
                title: "Not Found",
                detail: $"Genre with ID '{id}' was not found.",
                statusCode: StatusCodes.Status404NotFound);

        genre.UpdateName(request.Name);
        await db.SaveChangesAsync(ct);

        return Results.NoContent();
    }
}
