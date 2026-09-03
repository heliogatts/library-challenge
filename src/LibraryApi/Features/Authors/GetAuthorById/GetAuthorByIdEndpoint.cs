using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;

namespace LibraryApi.Features.Authors.GetAuthorById;

public static class GetAuthorByIdEndpoint
{
    public static RouteGroupBuilder MapGetAuthorById(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", Handle)
            .WithName("GetAuthorById")
            .Produces<GetAuthorByIdResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }

    private static async Task<IResult> Handle(
        Guid id,
        LibraryDbContext db,
        CancellationToken ct)
    {
        var author = await db.Authors
            .Where(a => a.Id == id)
            .Select(a => new GetAuthorByIdResponse(
                a.Id,
                a.Name,
                a.Books.Count))
            .FirstOrDefaultAsync(ct);

        return author is null
            ? Results.Problem(
                title: "Not Found",
                detail: $"Author with ID '{id}' was not found.",
                statusCode: StatusCodes.Status404NotFound)
            : Results.Ok(author);
    }
}
