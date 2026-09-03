using LibraryApi.Data;
using LibraryApi.Domain;
using LibraryApi.Shared.Filters;

namespace LibraryApi.Features.Genres.CreateGenre;

public static class CreateGenreEndpoint
{
    public static RouteGroupBuilder MapCreateGenre(this RouteGroupBuilder group)
    {
        group.MapPost("/", Handle)
            .AddEndpointFilter<ValidationFilter<CreateGenreRequest>>()
            .WithName("CreateGenre")
            .Produces<CreateGenreResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);

        return group;
    }

    private static async Task<IResult> Handle(
        CreateGenreRequest request,
        LibraryDbContext db,
        CancellationToken ct)
    {
        var genre = new Genre { Name = request.Name.Trim() };

        db.Genres.Add(genre);
        await db.SaveChangesAsync(ct);

        return Results.Created(
            $"/api/genres/{genre.Id}",
            new CreateGenreResponse(genre.Id, genre.Name));
    }
}
