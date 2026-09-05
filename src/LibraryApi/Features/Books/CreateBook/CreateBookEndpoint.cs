using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;
using LibraryApi.Domain;
using LibraryApi.Domain.ValueObjects;
using LibraryApi.Shared.Filters;

namespace LibraryApi.Features.Books.CreateBook;

public static class CreateBookEndpoint
{
    public static RouteGroupBuilder MapCreateBook(this RouteGroupBuilder group)
    {
        group.MapPost("/", Handle)
            .AddEndpointFilter<ValidationFilter<CreateBookRequest>>()
            .WithName("CreateBook")
            .Produces<CreateBookResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        return group;
    }

    private static async Task<IResult> Handle(
        CreateBookRequest request,
        LibraryDbContext db,
        CancellationToken ct)
    {
        var authorExists = await db.Authors.AnyAsync(a => a.Id == request.AuthorId, ct);
        var genreExists = await db.Genres.AnyAsync(g => g.Id == request.GenreId, ct);

        if (!authorExists || !genreExists)
            return Results.Problem(
                title: "Invalid Reference",
                detail: "The specified Author or Genre does not exist.",
                statusCode: StatusCodes.Status422UnprocessableEntity);

        var book = Book.Create(
            request.Title,
            Isbn.Create(request.ISBN),
            request.PublishedYear,
            request.Description,
            request.AuthorId,
            request.GenreId);

        db.Books.Add(book);
        await db.SaveChangesAsync(ct);

        return Results.Created(
            $"/api/books/{book.Id}",
            new CreateBookResponse(book.Id, book.Title));
    }
}
