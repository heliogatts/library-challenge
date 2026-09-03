using LibraryApi.Data;
using LibraryApi.Domain;
using LibraryApi.Shared.Filters;

namespace LibraryApi.Features.Authors.CreateAuthor;

public static class CreateAuthorEndpoint
{
    public static RouteGroupBuilder MapCreateAuthor(this RouteGroupBuilder group)
    {
        group.MapPost("/", Handle)
            .AddEndpointFilter<ValidationFilter<CreateAuthorRequest>>()
            .WithName("CreateAuthor")
            .Produces<CreateAuthorResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);

        return group;
    }

    private static async Task<IResult> Handle(
        CreateAuthorRequest request,
        LibraryDbContext db,
        CancellationToken ct)
    {
        var author = new Author { Name = request.Name.Trim() };

        db.Authors.Add(author);
        await db.SaveChangesAsync(ct);

        return Results.Created(
            $"/api/authors/{author.Id}",
            new CreateAuthorResponse(author.Id, author.Name));
    }
}
