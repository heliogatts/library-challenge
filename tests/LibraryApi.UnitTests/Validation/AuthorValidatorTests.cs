using FluentAssertions;
using LibraryApi.Features.Authors.CreateAuthor;
using LibraryApi.Features.Authors.UpdateAuthor;

namespace LibraryApi.UnitTests.Validation;

public class AuthorValidatorTests
{
    private readonly CreateAuthorValidator _createValidator = new();
    private readonly UpdateAuthorValidator _updateValidator = new();

    [Theory]
    [InlineData("J.R.R. Tolkien")]
    [InlineData("George Orwell")]
    [InlineData("A")]
    public async Task CreateAuthor_WithValidName_Succeeds(string validName)
    {
        var request = new CreateAuthorRequest(validName);
        var result = await _createValidator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateAuthor_WithEmptyName_Fails(string? emptyName)
    {
        var request = new CreateAuthorRequest(emptyName!);
        var result = await _createValidator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task CreateAuthor_WithNameExceeding200Chars_Fails()
    {
        var request = new CreateAuthorRequest(new string('A', 201));
        var result = await _createValidator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage.Contains("200"));
    }

    [Theory]
    [InlineData("Arthur C. Clarke")]
    [InlineData("Isaac Asimov")]
    public async Task UpdateAuthor_WithValidName_Succeeds(string validName)
    {
        var request = new UpdateAuthorRequest(validName);
        var result = await _updateValidator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task UpdateAuthor_WithEmptyName_Fails(string? emptyName)
    {
        var request = new UpdateAuthorRequest(emptyName!);
        var result = await _updateValidator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task UpdateAuthor_WithNameExceeding200Chars_Fails()
    {
        var request = new UpdateAuthorRequest(new string('X', 201));
        var result = await _updateValidator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage.Contains("200"));
    }
}
