using FluentAssertions;
using LibraryApi.Domain;
using LibraryApi.Domain.Exceptions;
using LibraryApi.Domain.ValueObjects;

namespace LibraryApi.UnitTests.Domain.Entities;

public class BookTests
{
    private static readonly Guid ValidAuthorId = Guid.NewGuid();
    private static readonly Guid ValidGenreId = Guid.NewGuid();
    private static readonly Isbn ValidIsbn = Isbn.Create("9780451524935");

    [Fact]
    public void Create_WithValidParameters_SetsPropertiesAndTimestamp()
    {
        var before = DateTime.UtcNow;
        var book = Book.Create(
            "Dune",
            ValidIsbn,
            1965,
            "A science fiction masterpiece.",
            ValidAuthorId,
            ValidGenreId
        );
        var after = DateTime.UtcNow;

        book.Id.Should().NotBeEmpty();
        book.Title.Should().Be("Dune");
        book.ISBN.Should().Be(ValidIsbn);
        book.PublishedYear.Should().Be(1965);
        book.Description.Should().Be("A science fiction masterpiece.");
        book.AuthorId.Should().Be(ValidAuthorId);
        book.GenreId.Should().Be(ValidGenreId);
        book.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        book.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void Create_WithCustomId_PreservesId()
    {
        var customId = Guid.NewGuid();
        var book = Book.Create(
            "Dune",
            ValidIsbn,
            1965,
            null,
            ValidAuthorId,
            ValidGenreId,
            id: customId
        );

        book.Id.Should().Be(customId);
    }

    [Fact]
    public void Create_TrimsWhitespaceFromTitleAndDescription()
    {
        var book = Book.Create(
            "   Dune Messiah   ",
            ValidIsbn,
            1969,
            "   Sequel to Dune.   ",
            ValidAuthorId,
            ValidGenreId
        );

        book.Title.Should().Be("Dune Messiah");
        book.Description.Should().Be("Sequel to Dune.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidTitle_ThrowsDomainException(string? invalidTitle)
    {
        var act = () => Book.Create(
            invalidTitle!,
            ValidIsbn,
            2000,
            null,
            ValidAuthorId,
            ValidGenreId
        );

        act.Should().Throw<DomainException>()
            .WithMessage("*title cannot be empty*");
    }

    [Fact]
    public void Create_WithTitleExceeding200Chars_ThrowsDomainException()
    {
        var longTitle = new string('T', 201);

        var act = () => Book.Create(
            longTitle,
            ValidIsbn,
            2000,
            null,
            ValidAuthorId,
            ValidGenreId
        );

        act.Should().Throw<DomainException>()
            .WithMessage("*cannot exceed 200 characters*");
    }

    [Theory]
    [InlineData(1449)]
    [InlineData(3000)]
    public void Create_WithYearOutOfRange_ThrowsDomainException(int invalidYear)
    {
        var act = () => Book.Create(
            "Valid Title",
            ValidIsbn,
            invalidYear,
            null,
            ValidAuthorId,
            ValidGenreId
        );

        act.Should().Throw<DomainException>()
            .WithMessage("*Published year must be between*");
    }

    [Fact]
    public void Create_WithDescriptionExceeding2000Chars_ThrowsDomainException()
    {
        var longDesc = new string('D', 2001);

        var act = () => Book.Create(
            "Valid Title",
            ValidIsbn,
            2000,
            longDesc,
            ValidAuthorId,
            ValidGenreId
        );

        act.Should().Throw<DomainException>()
            .WithMessage("*cannot exceed 2000 characters*");
    }

    [Fact]
    public void Create_WithEmptyAuthorId_ThrowsDomainException()
    {
        var act = () => Book.Create(
            "Valid Title",
            ValidIsbn,
            2000,
            null,
            Guid.Empty,
            ValidGenreId
        );

        act.Should().Throw<DomainException>()
            .WithMessage("*Author ID cannot be empty*");
    }

    [Fact]
    public void Create_WithEmptyGenreId_ThrowsDomainException()
    {
        var act = () => Book.Create(
            "Valid Title",
            ValidIsbn,
            2000,
            null,
            ValidAuthorId,
            Guid.Empty
        );

        act.Should().Throw<DomainException>()
            .WithMessage("*Genre ID cannot be empty*");
    }

    [Fact]
    public void UpdateDetails_WithValidData_UpdatesPropertiesAndSetsUpdatedAt()
    {
        var book = Book.Create(
            "Initial Title",
            ValidIsbn,
            2000,
            "Initial Desc",
            ValidAuthorId,
            ValidGenreId
        );

        var newAuthorId = Guid.NewGuid();
        var newGenreId = Guid.NewGuid();
        var newIsbn = Isbn.Create("9780553293357");

        var before = DateTime.UtcNow;
        book.UpdateDetails(
            "Updated Title",
            newIsbn,
            2010,
            "Updated Desc",
            newAuthorId,
            newGenreId
        );
        var after = DateTime.UtcNow;

        book.Title.Should().Be("Updated Title");
        book.ISBN.Should().Be(newIsbn);
        book.PublishedYear.Should().Be(2010);
        book.Description.Should().Be("Updated Desc");
        book.AuthorId.Should().Be(newAuthorId);
        book.GenreId.Should().Be(newGenreId);
        book.UpdatedAt.Should().NotBeNull();
        book.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }
}
