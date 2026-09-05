using LibraryApi.Domain.Exceptions;

namespace LibraryApi.Domain.ValueObjects;

public sealed record Isbn
{
    public string Value { get; }

    private Isbn(string value)
    {
        Value = value;
    }

    public static Isbn Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("ISBN cannot be empty.");

        var normalized = value.Replace("-", "").Replace(" ", "").Trim().ToUpperInvariant();

        if (normalized.Length != 10 && normalized.Length != 13)
            throw new DomainException($"ISBN must be 10 or 13 characters long. Received '{value}'.");

        if (normalized.Length == 13 && !normalized.All(char.IsDigit))
            throw new DomainException($"13-digit ISBN must contain only digits. Received '{value}'.");

        if (normalized.Length == 10)
        {
            for (int i = 0; i < 9; i++)
            {
                if (!char.IsDigit(normalized[i]))
                    throw new DomainException($"10-digit ISBN must start with 9 digits. Received '{value}'.");
            }

            if (!char.IsDigit(normalized[9]) && normalized[9] != 'X')
                throw new DomainException($"10-digit ISBN must end with a digit or 'X'. Received '{value}'.");
        }

        return new Isbn(normalized);
    }

    public static implicit operator string(Isbn isbn) => isbn.Value;

    public override string ToString() => Value;
}
