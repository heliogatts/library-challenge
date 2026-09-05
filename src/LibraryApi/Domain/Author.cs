using LibraryApi.Domain.Exceptions;

namespace LibraryApi.Domain;

public class Author
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public ICollection<Book> Books { get; private set; } = [];

    // EF Core parameterless constructor
    private Author() { }

    public static Author Create(string name, Guid? id = null)
    {
        ValidateName(name);

        return new Author
        {
            Id = id ?? Guid.NewGuid(),
            Name = name.Trim(),
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateName(string newName)
    {
        ValidateName(newName);

        Name = newName.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Author name cannot be empty.");

        if (name.Trim().Length > 200)
            throw new DomainException("Author name cannot exceed 200 characters.");
    }
}
