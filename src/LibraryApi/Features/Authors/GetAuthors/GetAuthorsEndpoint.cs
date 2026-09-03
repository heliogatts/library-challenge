using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;
using LibraryApi.Shared.Models;

namespace LibraryApi.Features.Authors.GetAuthors;

public static class GetAuthorsEndpoint
{
    public static RouteGroupBuilder MapGetAuthors(this RouteGroupBuilder group)
    {
        group.MapGet("/", Handle)
            .WithName("GetAuthors")
            .Produces<PagedResponse<GetAuthorsResponseItem>>();

        return group;
    }

    private static async Task<IResult> Handle(
        [AsParameters] PagedRequest request,
        LibraryDbContext db,
        CancellationToken ct)
    {
        var query = db.Authors.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            query = query.Where(a => EF.Functions.ILike(a.Name, $"%{request.SearchTerm}%"));

        var totalCount = await query.CountAsync(ct);

        query = request.SortBy?.ToLowerInvariant() switch
        {
            "name" => request.SortDirection?.ToLowerInvariant() == "desc"
                ? query.OrderByDescending(a => a.Name)
                : query.OrderBy(a => a.Name),
            _ => query.OrderBy(a => a.Name)
        };

        var items = await query
            .Skip((request.Page!.Value - 1) * request.PageSize!.Value)
            .Take(request.PageSize!.Value)
            .Select(a => new GetAuthorsResponseItem(
                a.Id,
                a.Name,
                a.Books.Count))
            .ToListAsync(ct);

        return Results.Ok(new PagedResponse<GetAuthorsResponseItem>
        {
            Items = items,
            Page = request.Page.Value,
            PageSize = request.PageSize.Value,
            TotalCount = totalCount
        });
    }
}
