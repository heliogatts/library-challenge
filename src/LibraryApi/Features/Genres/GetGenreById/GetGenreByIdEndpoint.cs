using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;

namespace LibraryApi.Features.Genres.GetGenreById;

public static class GetGenreByIdEndpoint
{
    public static RouteGroupBuilder MapGetGenreById(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", Handle)
            .WithName("GetGenreById")
            .Produces<GetGenreByIdResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }

    private static async Task<IResult> Handle(
        Guid id,
        LibraryDbContext db,
        CancellationToken ct)
    {
        var genre = await db.Genres
            .Where(g => g.Id == id)
            .Select(g => new GetGenreByIdResponse(
                g.Id,
                g.Name,
                g.Books.Count))
            .FirstOrDefaultAsync(ct);

        return genre is null
            ? Results.Problem(
                title: "Not Found",
                detail: $"Genre with ID '{id}' was not found.",
                statusCode: StatusCodes.Status404NotFound)
            : Results.Ok(genre);
    }
}
