using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;
using LibraryApi.Shared.Models;

namespace LibraryApi.Features.Books.GetBooks;

public static class GetBooksEndpoint
{
    public static RouteGroupBuilder MapGetBooks(this RouteGroupBuilder group)
    {
        group.MapGet("/", Handle)
            .WithName("GetBooks")
            .Produces<PagedResponse<GetBooksResponseItem>>();

        return group;
    }

    private static async Task<IResult> Handle(
        [AsParameters] GetBooksRequest request,
        LibraryDbContext db,
        CancellationToken ct)
    {
        var query = db.Books.AsQueryable();

        // Filter by GenreId
        if (request.GenreId.HasValue)
            query = query.Where(b => b.GenreId == request.GenreId.Value);

        // Filter by AuthorId
        if (request.AuthorId.HasValue)
            query = query.Where(b => b.AuthorId == request.AuthorId.Value);

        // Search by title
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            query = query.Where(b => EF.Functions.ILike(b.Title, $"%{request.SearchTerm}%"));

        var totalCount = await query.CountAsync(ct);

        // Sort
        query = request.SortBy?.ToLowerInvariant() switch
        {
            "title" => request.SortDirection?.ToLowerInvariant() == "desc"
                ? query.OrderByDescending(b => b.Title)
                : query.OrderBy(b => b.Title),
            "year" => request.SortDirection?.ToLowerInvariant() == "desc"
                ? query.OrderByDescending(b => b.PublishedYear)
                : query.OrderBy(b => b.PublishedYear),
            "author" => request.SortDirection?.ToLowerInvariant() == "desc"
                ? query.OrderByDescending(b => b.Author.Name)
                : query.OrderBy(b => b.Author.Name),
            "genre" => request.SortDirection?.ToLowerInvariant() == "desc"
                ? query.OrderByDescending(b => b.Genre.Name)
                : query.OrderBy(b => b.Genre.Name),
            _ => query.OrderBy(b => b.Title)
        };

        // Projection — prevents N+1, generates a single SQL JOIN
        var items = await query
            .Skip((request.Page!.Value - 1) * request.PageSize!.Value)
            .Take(request.PageSize!.Value)
            .Select(b => new GetBooksResponseItem(
                b.Id,
                b.Title,
                b.ISBN,
                b.PublishedYear,
                b.Author.Name,
                b.Genre.Name))
            .ToListAsync(ct);

        return Results.Ok(new PagedResponse<GetBooksResponseItem>
        {
            Items = items,
            Page = request.Page.Value,
            PageSize = request.PageSize.Value,
            TotalCount = totalCount
        });
    }
}
