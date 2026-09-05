using FluentAssertions;
using LibraryApi.Features.Books.CreateBook;

namespace LibraryApi.UnitTests.Validation;

public class CreateBookValidatorTests
{
    private readonly CreateBookValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidRequest_Succeeds()
    {
        var request = new CreateBookRequest(
            Title: "The Fellowship of the Ring",
            ISBN: "9780618640157",
            PublishedYear: 1954,
            Description: "An epic high-fantasy novel.",
            AuthorId: Guid.NewGuid(),
            GenreId: Guid.NewGuid()
        );

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("978-0-618-64015-7")] // 13-digit hyphenated (17 chars)
    [InlineData("0-451-52493-4")]     // 10-digit hyphenated (13 chars)
    [InlineData("0451524934")]        // 10-digit plain
    [InlineData("080442957X")]        // 10-digit with terminal X
    public async Task Validate_WithValidHyphenatedOrFormattedIsbn_Succeeds(string validIsbn)
    {
        var request = new CreateBookRequest(
            Title: "Valid Book",
            ISBN: validIsbn,
            PublishedYear: 2000,
            Description: null,
            AuthorId: Guid.NewGuid(),
            GenreId: Guid.NewGuid()
        );

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_WithEmptyTitle_Fails(string? title)
    {
        var request = new CreateBookRequest(
            Title: title!,
            ISBN: "9780618640157",
            PublishedYear: 2000,
            Description: null,
            AuthorId: Guid.NewGuid(),
            GenreId: Guid.NewGuid()
        );

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public async Task Validate_WithTitleExceeding200Chars_Fails()
    {
        var request = new CreateBookRequest(
            Title: new string('A', 201),
            ISBN: "9780618640157",
            PublishedYear: 2000,
            Description: null,
            AuthorId: Guid.NewGuid(),
            GenreId: Guid.NewGuid()
        );

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title" && e.ErrorMessage.Contains("200"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_WithEmptyIsbn_Fails(string? isbn)
    {
        var request = new CreateBookRequest(
            Title: "Valid Title",
            ISBN: isbn!,
            PublishedYear: 2000,
            Description: null,
            AuthorId: Guid.NewGuid(),
            GenreId: Guid.NewGuid()
        );

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ISBN");
    }

    [Theory]
    [InlineData("123")]                 // Too short
    [InlineData("123456789012345678")]  // Exceeds 17 characters
    [InlineData("978061864015A")]       // 13-digit with letter
    [InlineData("X0451524934")]         // 10-digit with non-terminal X
    public async Task Validate_WithInvalidIsbn_Fails(string invalidIsbn)
    {
        var request = new CreateBookRequest(
            Title: "Valid Title",
            ISBN: invalidIsbn,
            PublishedYear: 2000,
            Description: null,
            AuthorId: Guid.NewGuid(),
            GenreId: Guid.NewGuid()
        );

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ISBN");
    }

    [Theory]
    [InlineData(1449)] // Before Gutenberg press threshold
    [InlineData(2099)] // Future year
    public async Task Validate_WithInvalidPublishedYear_Fails(int year)
    {
        var request = new CreateBookRequest(
            Title: "Valid Title",
            ISBN: "9780618640157",
            PublishedYear: year,
            Description: null,
            AuthorId: Guid.NewGuid(),
            GenreId: Guid.NewGuid()
        );

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PublishedYear");
    }

    [Fact]
    public async Task Validate_WithDescriptionExceeding2000Chars_Fails()
    {
        var request = new CreateBookRequest(
            Title: "Valid Title",
            ISBN: "9780618640157",
            PublishedYear: 2000,
            Description: new string('D', 2001),
            AuthorId: Guid.NewGuid(),
            GenreId: Guid.NewGuid()
        );

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }

    [Fact]
    public async Task Validate_WithEmptyAuthorOrGenre_Fails()
    {
        var request = new CreateBookRequest(
            Title: "Valid Title",
            ISBN: "9780618640157",
            PublishedYear: 2000,
            Description: null,
            AuthorId: Guid.Empty,
            GenreId: Guid.Empty
        );

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AuthorId");
        result.Errors.Should().Contain(e => e.PropertyName == "GenreId");
    }
}
