using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;
using LibraryApi.Shared.Models;

namespace LibraryApi.Features.Genres.GetGenres;

public static class GetGenresEndpoint
{
    public static RouteGroupBuilder MapGetGenres(this RouteGroupBuilder group)
    {
        group.MapGet("/", Handle)
            .WithName("GetGenres")
            .Produces<PagedResponse<GetGenresResponseItem>>();

        return group;
    }

    private static async Task<IResult> Handle(
        [AsParameters] PagedRequest request,
        LibraryDbContext db,
        CancellationToken ct)
    {
        var query = db.Genres.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            query = query.Where(g => EF.Functions.ILike(g.Name, $"%{request.SearchTerm}%"));

        var totalCount = await query.CountAsync(ct);

        query = request.SortBy?.ToLowerInvariant() switch
        {
            "name" => request.SortDirection?.ToLowerInvariant() == "desc"
                ? query.OrderByDescending(g => g.Name)
                : query.OrderBy(g => g.Name),
            _ => query.OrderBy(g => g.Name)
        };

        var items = await query
            .Skip((request.Page!.Value - 1) * request.PageSize!.Value)
            .Take(request.PageSize!.Value)
            .Select(g => new GetGenresResponseItem(
                g.Id,
                g.Name,
                g.Books.Count))
            .ToListAsync(ct);

        return Results.Ok(new PagedResponse<GetGenresResponseItem>
        {
            Items = items,
            Page = request.Page.Value,
            PageSize = request.PageSize.Value,
            TotalCount = totalCount
        });
    }
}
