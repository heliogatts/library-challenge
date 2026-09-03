using LibraryApi.IntegrationTests.Infrastructure;

namespace LibraryApi.IntegrationTests.Features.Authors;

public class AuthorEndpointsTests(LibraryApiFactory factory) : IClassFixture<LibraryApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetAuthors_ReturnsSeededData()
    {
        var response = await _client.GetAsync("/api/authors");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<PagedResult<AuthorItem>>();
        content!.Items.Should().NotBeEmpty();
        content.Items.Should().Contain(a => a.Name == "Isaac Asimov");
    }

    [Fact]
    public async Task CreateAuthor_WithValidData_Returns201()
    {
        var response = await _client.PostAsJsonAsync("/api/authors", new { Name = "Stephen King" });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<AuthorCreated>();
        content!.Name.Should().Be("Stephen King");
    }

    [Fact]
    public async Task CreateAuthor_WithEmptyName_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/authors", new { Name = "" });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteAuthor_WithBooks_Returns409()
    {
        // "Isaac Asimov" has seeded books
        var authorsResponse = await _client.GetFromJsonAsync<PagedResult<AuthorItem>>("/api/authors?searchTerm=Asimov&pageSize=50");
        var asimov = authorsResponse!.Items.First(a => a.Name == "Isaac Asimov");

        var response = await _client.DeleteAsync($"/api/authors/{asimov.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task DeleteAuthor_WithoutBooks_Returns204()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/authors", new { Name = "ToDeleteAuthor" });
        var created = await createResponse.Content.ReadFromJsonAsync<AuthorCreated>();

        var response = await _client.DeleteAsync($"/api/authors/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount, int TotalPages, bool HasPreviousPage, bool HasNextPage);
    private record AuthorItem(Guid Id, string Name, int BookCount);
    private record AuthorCreated(Guid Id, string Name);
}
