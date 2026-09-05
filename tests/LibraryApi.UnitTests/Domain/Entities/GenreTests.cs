using FluentAssertions;
using LibraryApi.Domain;
using LibraryApi.Domain.Exceptions;

namespace LibraryApi.UnitTests.Domain.Entities;

public class GenreTests
{
    [Fact]
    public void Create_WithValidName_SetsPropertiesAndTimestamp()
    {
        var before = DateTime.UtcNow;
        var genre = Genre.Create("Cyberpunk");
        var after = DateTime.UtcNow;

        genre.Id.Should().NotBeEmpty();
        genre.Name.Should().Be("Cyberpunk");
        genre.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        genre.UpdatedAt.Should().BeNull();
        genre.Books.Should().BeEmpty();
    }

    [Fact]
    public void Create_WithCustomId_PreservesId()
    {
        var customId = Guid.NewGuid();
        var genre = Genre.Create("Horror", id: customId);

        genre.Id.Should().Be(customId);
        genre.Name.Should().Be("Horror");
    }

    [Fact]
    public void Create_TrimsWhitespace()
    {
        var genre = Genre.Create("   Thriller   ");

        genre.Name.Should().Be("Thriller");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidName_ThrowsDomainException(string? invalidName)
    {
        var act = () => Genre.Create(invalidName!);

        act.Should().Throw<DomainException>()
            .WithMessage("*cannot be empty*");
    }

    [Fact]
    public void Create_WithNameExceeding100Chars_ThrowsDomainException()
    {
        var longName = new string('G', 101);

        var act = () => Genre.Create(longName);

        act.Should().Throw<DomainException>()
            .WithMessage("*cannot exceed 100 characters*");
    }

    [Fact]
    public void UpdateName_WithValidName_UpdatesNameAndSetsUpdatedAt()
    {
        var genre = Genre.Create("Old Genre");
        var before = DateTime.UtcNow;

        genre.UpdateName("New Genre");
        var after = DateTime.UtcNow;

        genre.Name.Should().Be("New Genre");
        genre.UpdatedAt.Should().NotBeNull();
        genre.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateName_WithInvalidName_ThrowsDomainException(string? invalidName)
    {
        var genre = Genre.Create("Valid Genre");

        var act = () => genre.UpdateName(invalidName!);

        act.Should().Throw<DomainException>()
            .WithMessage("*cannot be empty*");
    }
}
