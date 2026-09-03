using Microsoft.EntityFrameworkCore;
using LibraryApi.Domain;

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
        var fiction = new Genre { Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000001"), Name = "Fiction" };
        var sciFi = new Genre { Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000002"), Name = "Science Fiction" };
        var fantasy = new Genre { Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000003"), Name = "Fantasy" };

        db.Genres.AddRange(fiction, sciFi, fantasy);

        // Authors
        var orwell = new Author { Id = Guid.Parse("b1b2c3d4-0002-0002-0002-000000000001"), Name = "George Orwell" };
        var asimov = new Author { Id = Guid.Parse("b1b2c3d4-0002-0002-0002-000000000002"), Name = "Isaac Asimov" };
        var tolkien = new Author { Id = Guid.Parse("b1b2c3d4-0002-0002-0002-000000000003"), Name = "J.R.R. Tolkien" };
        var fitzgerald = new Author { Id = Guid.Parse("b1b2c3d4-0002-0002-0002-000000000004"), Name = "F. Scott Fitzgerald" };

        db.Authors.AddRange(orwell, asimov, tolkien, fitzgerald);

        // Books
        db.Books.AddRange(
            new Book
            {
                Title = "1984",
                ISBN = "9780451524935",
                PublishedYear = 1949,
                Description = "A dystopian novel set in a totalitarian society ruled by Big Brother.",
                AuthorId = orwell.Id,
                GenreId = fiction.Id
            },
            new Book
            {
                Title = "Foundation",
                ISBN = "9780553293357",
                PublishedYear = 1951,
                Description = "The first novel in Isaac Asimov's Foundation series.",
                AuthorId = asimov.Id,
                GenreId = sciFi.Id
            },
            new Book
            {
                Title = "The Hobbit",
                ISBN = "9780547928227",
                PublishedYear = 1937,
                Description = "A fantasy novel about the adventures of Bilbo Baggins.",
                AuthorId = tolkien.Id,
                GenreId = fantasy.Id
            },
            new Book
            {
                Title = "The Great Gatsby",
                ISBN = "9780743273565",
                PublishedYear = 1925,
                Description = "A novel about the mysterious millionaire Jay Gatsby.",
                AuthorId = fitzgerald.Id,
                GenreId = fiction.Id
            },
            new Book
            {
                Title = "I, Robot",
                ISBN = "9780553382563",
                PublishedYear = 1950,
                Description = "A collection of nine science fiction short stories.",
                AuthorId = asimov.Id,
                GenreId = sciFi.Id
            },
            new Book
            {
                Title = "The Lord of the Rings",
                ISBN = "9780618640157",
                PublishedYear = 1954,
                Description = "An epic high-fantasy novel by J.R.R. Tolkien.",
                AuthorId = tolkien.Id,
                GenreId = fantasy.Id
            }
        );

        await db.SaveChangesAsync();
        logger.LogInformation("Database seeded successfully with {GenreCount} genres, {AuthorCount} authors, and {BookCount} books.", 3, 4, 6);
    }
}
