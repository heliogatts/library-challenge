using FluentAssertions;
using LibraryApi.Domain;
using LibraryApi.Domain.Exceptions;

namespace LibraryApi.UnitTests.Domain.Entities;

public class AuthorTests
{
    [Fact]
    public void Create_WithValidName_SetsPropertiesAndTimestamp()
    {
        var before = DateTime.UtcNow;
        var author = Author.Create("Ursula K. Le Guin");
        var after = DateTime.UtcNow;

        author.Id.Should().NotBeEmpty();
        author.Name.Should().Be("Ursula K. Le Guin");
        author.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        author.UpdatedAt.Should().BeNull();
        author.Books.Should().BeEmpty();
    }

    [Fact]
    public void Create_WithCustomId_PreservesId()
    {
        var customId = Guid.NewGuid();
        var author = Author.Create("Frank Herbert", id: customId);

        author.Id.Should().Be(customId);
        author.Name.Should().Be("Frank Herbert");
    }

    [Fact]
    public void Create_TrimsWhitespace()
    {
        var author = Author.Create("   Philip K. Dick   ");

        author.Name.Should().Be("Philip K. Dick");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidName_ThrowsDomainException(string? invalidName)
    {
        var act = () => Author.Create(invalidName!);

        act.Should().Throw<DomainException>()
            .WithMessage("*cannot be empty*");
    }

    [Fact]
    public void Create_WithNameExceeding200Chars_ThrowsDomainException()
    {
        var longName = new string('A', 201);

        var act = () => Author.Create(longName);

        act.Should().Throw<DomainException>()
            .WithMessage("*cannot exceed 200 characters*");
    }

    [Fact]
    public void UpdateName_WithValidName_UpdatesNameAndSetsUpdatedAt()
    {
        var author = Author.Create("Old Name");
        var before = DateTime.UtcNow;

        author.UpdateName("New Name");
        var after = DateTime.UtcNow;

        author.Name.Should().Be("New Name");
        author.UpdatedAt.Should().NotBeNull();
        author.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateName_WithInvalidName_ThrowsDomainException(string? invalidName)
    {
        var author = Author.Create("Valid Name");

        var act = () => author.UpdateName(invalidName!);

        act.Should().Throw<DomainException>()
            .WithMessage("*cannot be empty*");
    }
}
