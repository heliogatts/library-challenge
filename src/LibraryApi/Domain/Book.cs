namespace LibraryApi.Domain;

public class Book
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public int PublishedYear { get; set; }
    public string? Description { get; set; }
    public Guid AuthorId { get; set; }
    public Guid GenreId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Author Author { get; set; } = null!;
    public Genre Genre { get; set; } = null!;
}
