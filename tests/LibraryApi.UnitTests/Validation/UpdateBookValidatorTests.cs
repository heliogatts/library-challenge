using FluentAssertions;
using LibraryApi.Features.Books.UpdateBook;

namespace LibraryApi.UnitTests.Validation;

public class UpdateBookValidatorTests
{
    private readonly UpdateBookValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidRequest_Succeeds()
    {
        var request = new UpdateBookRequest(
            Title: "The Two Towers",
            ISBN: "978-0-618-64015-7",
            PublishedYear: 1954,
            Description: "Second volume.",
            AuthorId: Guid.NewGuid(),
            GenreId: Guid.NewGuid()
        );

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_WithEmptyTitle_Fails(string? title)
    {
        var request = new UpdateBookRequest(
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

    [Theory]
    [InlineData("invalid-isbn")]
    [InlineData("123456789012345678")]
    public async Task Validate_WithInvalidIsbn_Fails(string invalidIsbn)
    {
        var request = new UpdateBookRequest(
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
    [InlineData(1400)]
    [InlineData(2100)]
    public async Task Validate_WithYearOutOfRange_Fails(int year)
    {
        var request = new UpdateBookRequest(
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
}
