using LibraryApi.IntegrationTests.Infrastructure;

namespace LibraryApi.IntegrationTests.Features.Genres;

public class GenreEndpointsTests(LibraryApiFactory factory) : IClassFixture<LibraryApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetGenres_ReturnsSeededData()
    {
        // Act
        var response = await _client.GetAsync("/api/genres");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<PagedResult<GenreItem>>();
        content.Should().NotBeNull();
        content!.Items.Should().NotBeEmpty();
        content.Items.Should().Contain(g => g.Name == "Fiction");
        content.Items.Should().Contain(g => g.Name == "Science Fiction");
        content.Items.Should().Contain(g => g.Name == "Fantasy");
    }

    [Fact]
    public async Task CreateGenre_WithValidData_Returns201()
    {
        // Arrange
        var request = new { Name = "Horror" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/genres", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadFromJsonAsync<GenreCreated>();
        content.Should().NotBeNull();
        content!.Name.Should().Be("Horror");
        content.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateGenre_WithEmptyName_Returns400()
    {
        // Arrange
        var request = new { Name = "" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/genres", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateGenre_WithDuplicateName_Returns409()
    {
        // Arrange — "Fiction" exists from seed data
        var request = new { Name = "Fiction" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/genres", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task UpdateGenre_ExistingGenre_Returns204()
    {
        // Arrange — create a genre first
        var createResponse = await _client.PostAsJsonAsync("/api/genres", new { Name = "UpdateTest" });
        var created = await createResponse.Content.ReadFromJsonAsync<GenreCreated>();

        // Act
        var response = await _client.PutAsJsonAsync($"/api/genres/{created!.Id}", new { Name = "UpdatedName" });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteGenre_WithoutBooks_Returns204()
    {
        // Arrange — create a genre with no books
        var createResponse = await _client.PostAsJsonAsync("/api/genres", new { Name = "ToDelete" });
        var created = await createResponse.Content.ReadFromJsonAsync<GenreCreated>();

        // Act
        var response = await _client.DeleteAsync($"/api/genres/{created!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteGenre_WithBooks_Returns409()
    {
        // Act — "Fiction" has seeded books
        // First, get Fiction genre ID
        var genresResponse = await _client.GetFromJsonAsync<PagedResult<GenreItem>>("/api/genres?searchTerm=Fiction&pageSize=50");
        var fictionGenre = genresResponse!.Items.First(g => g.Name == "Fiction");

        var response = await _client.DeleteAsync($"/api/genres/{fictionGenre.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetGenres_WithSearchTerm_FiltersResults()
    {
        // Act
        var response = await _client.GetFromJsonAsync<PagedResult<GenreItem>>("/api/genres?searchTerm=Sci");

        // Assert
        response.Should().NotBeNull();
        response!.Items.Should().ContainSingle(g => g.Name == "Science Fiction");
    }

    // Helper DTOs for deserialization
    private record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount, int TotalPages, bool HasPreviousPage, bool HasNextPage);
    private record GenreItem(Guid Id, string Name, int BookCount);
    private record GenreCreated(Guid Id, string Name);
}
