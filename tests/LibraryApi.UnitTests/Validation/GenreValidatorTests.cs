using FluentAssertions;
using LibraryApi.Features.Genres.CreateGenre;
using LibraryApi.Features.Genres.UpdateGenre;

namespace LibraryApi.UnitTests.Validation;

public class GenreValidatorTests
{
    private readonly CreateGenreValidator _createValidator = new();
    private readonly UpdateGenreValidator _updateValidator = new();

    [Theory]
    [InlineData("Science Fiction")]
    [InlineData("Fantasy")]
    [InlineData("G")]
    public async Task CreateGenre_WithValidName_Succeeds(string validName)
    {
        var request = new CreateGenreRequest(validName);
        var result = await _createValidator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateGenre_WithEmptyName_Fails(string? emptyName)
    {
        var request = new CreateGenreRequest(emptyName!);
        var result = await _createValidator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task CreateGenre_WithNameExceeding100Chars_Fails()
    {
        var request = new CreateGenreRequest(new string('G', 101));
        var result = await _createValidator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage.Contains("100"));
    }

    [Theory]
    [InlineData("Non-Fiction")]
    [InlineData("Mystery")]
    public async Task UpdateGenre_WithValidName_Succeeds(string validName)
    {
        var request = new UpdateGenreRequest(validName);
        var result = await _updateValidator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task UpdateGenre_WithEmptyName_Fails(string? emptyName)
    {
        var request = new UpdateGenreRequest(emptyName!);
        var result = await _updateValidator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task UpdateGenre_WithNameExceeding100Chars_Fails()
    {
        var request = new UpdateGenreRequest(new string('Z', 101));
        var result = await _updateValidator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage.Contains("100"));
    }
}
