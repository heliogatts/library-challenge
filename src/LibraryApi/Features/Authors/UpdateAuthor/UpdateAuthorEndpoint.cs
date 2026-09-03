using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;
using LibraryApi.Shared.Filters;

namespace LibraryApi.Features.Authors.UpdateAuthor;

public static class UpdateAuthorEndpoint
{
    public static RouteGroupBuilder MapUpdateAuthor(this RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}", Handle)
            .AddEndpointFilter<ValidationFilter<UpdateAuthorRequest>>()
            .WithName("UpdateAuthor")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        return group;
    }

    private static async Task<IResult> Handle(
        Guid id,
        UpdateAuthorRequest request,
        LibraryDbContext db,
        CancellationToken ct)
    {
        var author = await db.Authors.FirstOrDefaultAsync(a => a.Id == id, ct);

        if (author is null)
            return Results.Problem(
                title: "Not Found",
                detail: $"Author with ID '{id}' was not found.",
                statusCode: StatusCodes.Status404NotFound);

        author.Name = request.Name.Trim();
        await db.SaveChangesAsync(ct);

        return Results.NoContent();
    }
}
