using Microsoft.EntityFrameworkCore;
using LibraryApi.Domain;
using LibraryApi.Domain.ValueObjects;

namespace LibraryApi.Data;

public static class SeedData
{
    public static async Task SeedAsync(LibraryDbContext db, ILogger logger)
    {
        if (await db.Genres.AnyAsync())
        {
            logger.LogInformation("Database already seeded. Skipping.");
            return;
        }

        logger.LogInformation("Seeding database with initial data...");

        // Genres
        var fiction = Genre.Create("Fiction", id: Guid.Parse("a1b2c3d4-0001-0001-0001-000000000001"));
        var sciFi = Genre.Create("Science Fiction", id: Guid.Parse("a1b2c3d4-0001-0001-0001-000000000002"));
        var fantasy = Genre.Create("Fantasy", id: Guid.Parse("a1b2c3d4-0001-0001-0001-000000000003"));

        db.Genres.AddRange(fiction, sciFi, fantasy);

        // Authors
        var orwell = Author.Create("George Orwell", id: Guid.Parse("b1b2c3d4-0002-0002-0002-000000000001"));
        var asimov = Author.Create("Isaac Asimov", id: Guid.Parse("b1b2c3d4-0002-0002-0002-000000000002"));
        var tolkien = Author.Create("J.R.R. Tolkien", id: Guid.Parse("b1b2c3d4-0002-0002-0002-000000000003"));
        var fitzgerald = Author.Create("F. Scott Fitzgerald", id: Guid.Parse("b1b2c3d4-0002-0002-0002-000000000004"));

        db.Authors.AddRange(orwell, asimov, tolkien, fitzgerald);

        // Books
        db.Books.AddRange(
            Book.Create(
                "1984",
                Isbn.Create("9780451524935"),
                1949,
                "A dystopian novel set in a totalitarian society ruled by Big Brother.",
                orwell.Id,
                fiction.Id
            ),
            Book.Create(
                "Foundation",
                Isbn.Create("9780553293357"),
                1951,
                "The first novel in Isaac Asimov's Foundation series.",
                asimov.Id,
                sciFi.Id
            ),
            Book.Create(
                "The Hobbit",
                Isbn.Create("9780547928227"),
                1937,
                "A fantasy novel about the adventures of Bilbo Baggins.",
                tolkien.Id,
                fantasy.Id
            ),
            Book.Create(
                "The Great Gatsby",
                Isbn.Create("9780743273565"),
                1925,
                "A novel about the mysterious millionaire Jay Gatsby.",
                fitzgerald.Id,
                fiction.Id
            ),
            Book.Create(
                "I, Robot",
                Isbn.Create("9780553382563"),
                1950,
                "A collection of nine science fiction short stories.",
                asimov.Id,
                sciFi.Id
            ),
            Book.Create(
                "The Lord of the Rings",
                Isbn.Create("9780618640157"),
                1954,
                "An epic high-fantasy novel by J.R.R. Tolkien.",
                tolkien.Id,
                fantasy.Id
            )
        );

        await db.SaveChangesAsync();
        logger.LogInformation("Database seeded successfully with {GenreCount} genres, {AuthorCount} authors, and {BookCount} books.", 3, 4, 6);
    }
}
