using LibraryApi.IntegrationTests.Infrastructure;

namespace LibraryApi.IntegrationTests.Features.Books;

public class BookEndpointsTests(LibraryApiFactory factory) : IClassFixture<LibraryApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetBooks_ReturnsSeededData()
    {
        var response = await _client.GetAsync("/api/books");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<PagedResult<BookItem>>();
        content!.Items.Should().NotBeEmpty();
        content.Items.Should().Contain(b => b.Title == "1984");
    }

    [Fact]
    public async Task GetBooks_IncludesAuthorAndGenreNames()
    {
        var content = await _client.GetFromJsonAsync<PagedResult<BookItem>>("/api/books");

        var book = content!.Items.First(b => b.Title == "1984");
        book.AuthorName.Should().Be("George Orwell");
        book.GenreName.Should().Be("Fiction");
    }

    [Fact]
    public async Task GetBooks_WithGenreFilter_FiltersResults()
    {
        // Get the Science Fiction genre ID
        var genres = await _client.GetFromJsonAsync<PagedResult<GenreItem>>("/api/genres?searchTerm=Science Fiction&pageSize=50");
        var sciFiId = genres!.Items.First(g => g.Name == "Science Fiction").Id;

        var content = await _client.GetFromJsonAsync<PagedResult<BookItem>>($"/api/books?genreId={sciFiId}");

        content!.Items.Should().NotBeEmpty();
        content.Items.Should().AllSatisfy(b => b.GenreName.Should().Be("Science Fiction"));
    }

    [Fact]
    public async Task GetBooks_WithSearchTerm_FiltersResults()
    {
        var content = await _client.GetFromJsonAsync<PagedResult<BookItem>>("/api/books?searchTerm=Foundation");

        content!.Items.Should().ContainSingle(b => b.Title == "Foundation");
    }

    [Fact]
    public async Task GetBooks_Pagination_Works()
    {
        var content = await _client.GetFromJsonAsync<PagedResult<BookItem>>("/api/books?page=1&pageSize=2");

        content!.Items.Should().HaveCount(2);
        content.TotalCount.Should().BeGreaterThanOrEqualTo(6);
        content.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public async Task CreateBook_WithValidData_Returns201()
    {
        // Get existing author and genre
        var authors = await _client.GetFromJsonAsync<PagedResult<AuthorItem>>("/api/authors?pageSize=50");
        var genres = await _client.GetFromJsonAsync<PagedResult<GenreItem>>("/api/genres?pageSize=50");

        var request = new
        {
            Title = "Test Book",
            ISBN = "9781234567890",
            PublishedYear = 2023,
            Description = "A test book.",
            AuthorId = authors!.Items.First().Id,
            GenreId = genres!.Items.First().Id
        };

        var response = await _client.PostAsJsonAsync("/api/books", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateBook_WithInvalidAuthor_Returns422()
    {
        var genres = await _client.GetFromJsonAsync<PagedResult<GenreItem>>("/api/genres?pageSize=50");

        var request = new
        {
            Title = "Invalid Book",
            ISBN = "9789999999999",
            PublishedYear = 2023,
            AuthorId = Guid.NewGuid(),
            GenreId = genres!.Items.First().Id
        };

        var response = await _client.PostAsJsonAsync("/api/books", request);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task CreateBook_WithEmptyTitle_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/books", new
        {
            Title = "",
            ISBN = "9781111111111",
            PublishedYear = 2023,
            AuthorId = Guid.NewGuid(),
            GenreId = Guid.NewGuid()
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetBookById_ReturnsFullDetails()
    {
        var books = await _client.GetFromJsonAsync<PagedResult<BookItem>>("/api/books?searchTerm=1984");
        var bookId = books!.Items.First().Id;

        var response = await _client.GetAsync($"/api/books/{bookId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var detail = await response.Content.ReadFromJsonAsync<BookDetail>();
        detail!.Title.Should().Be("1984");
        detail.AuthorName.Should().Be("George Orwell");
        detail.GenreName.Should().Be("Fiction");
        detail.Description.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task DeleteBook_ExistingBook_Returns204()
    {
        // Create a book to delete
        var authors = await _client.GetFromJsonAsync<PagedResult<AuthorItem>>("/api/authors?pageSize=50");
        var genres = await _client.GetFromJsonAsync<PagedResult<GenreItem>>("/api/genres?pageSize=50");

        var createResponse = await _client.PostAsJsonAsync("/api/books", new
        {
            Title = "ToDelete",
            ISBN = "9780000000000",
            PublishedYear = 2023,
            AuthorId = authors!.Items.First().Id,
            GenreId = genres!.Items.First().Id
        });
        var created = await createResponse.Content.ReadFromJsonAsync<BookCreated>();

        var response = await _client.DeleteAsync($"/api/books/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    // Helper DTOs
    private record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount, int TotalPages, bool HasPreviousPage, bool HasNextPage);
    private record BookItem(Guid Id, string Title, string ISBN, int PublishedYear, string AuthorName, string GenreName);
    private record BookDetail(Guid Id, string Title, string ISBN, int PublishedYear, string? Description, Guid AuthorId, string AuthorName, Guid GenreId, string GenreName);
    private record BookCreated(Guid Id, string Title);
    private record GenreItem(Guid Id, string Name, int BookCount);
    private record AuthorItem(Guid Id, string Name, int BookCount);
}
