using FluentAssertions;
using LibraryApi.Domain.Exceptions;
using LibraryApi.Domain.ValueObjects;

namespace LibraryApi.UnitTests.Domain.ValueObjects;

public class IsbnTests
{
    [Theory]
    [InlineData("9780451524935", "9780451524935")]
    [InlineData("978-0-451-52493-5", "9780451524935")]
    [InlineData("978 0 451 52493 5", "9780451524935")]
    public void Create_WithValid13DigitIsbn_NormalizesAndReturnsInstance(string input, string expected)
    {
        var isbn = Isbn.Create(input);

        isbn.Value.Should().Be(expected);
        isbn.ToString().Should().Be(expected);
    }

    [Theory]
    [InlineData("0451524934", "0451524934")]
    [InlineData("0-451-52493-4", "0451524934")]
    [InlineData("045152493X", "045152493X")]
    [InlineData("0-451-52493-x", "045152493X")]
    public void Create_WithValid10DigitIsbn_NormalizesAndReturnsInstance(string input, string expected)
    {
        var isbn = Isbn.Create(input);

        isbn.Value.Should().Be(expected);
        isbn.ToString().Should().Be(expected);
    }

    [Fact]
    public void Isbn_HasValueEqualitySemantics()
    {
        var isbn1 = Isbn.Create("978-0-451-52493-5");
        var isbn2 = Isbn.Create("9780451524935");
        var isbn3 = Isbn.Create("9780553293357");

        isbn1.Should().Be(isbn2);
        (isbn1 == isbn2).Should().BeTrue();
        isbn1.Should().NotBe(isbn3);
    }

    [Fact]
    public void ImplicitConversionToString_ReturnsValue()
    {
        var isbn = Isbn.Create("9780451524935");
        string raw = isbn;

        raw.Should().Be("9780451524935");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithNullOrWhitespace_ThrowsDomainException(string? input)
    {
        var act = () => Isbn.Create(input);

        act.Should().Throw<DomainException>()
            .WithMessage("*cannot be empty*");
    }

    [Theory]
    [InlineData("123")]
    [InlineData("123456789")]
    [InlineData("12345678901")]
    [InlineData("12345678901234")]
    public void Create_WithInvalidLength_ThrowsDomainException(string input)
    {
        var act = () => Isbn.Create(input);

        act.Should().Throw<DomainException>()
            .WithMessage("*10 or 13 characters long*");
    }

    [Theory]
    [InlineData("978045152493A")]
    [InlineData("978045152493#")]
    public void Create_WithInvalid13DigitCharacters_ThrowsDomainException(string input)
    {
        var act = () => Isbn.Create(input);

        act.Should().Throw<DomainException>()
            .WithMessage("*must contain only digits*");
    }

    [Theory]
    [InlineData("045152493Y")]
    [InlineData("045A524934")]
    public void Create_WithInvalid10DigitCharacters_ThrowsDomainException(string input)
    {
        var act = () => Isbn.Create(input);

        act.Should().Throw<DomainException>();
    }
}
