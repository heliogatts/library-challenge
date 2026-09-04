using LibraryApi.IntegrationTests.Infrastructure;

namespace LibraryApi.IntegrationTests.Infrastructure;

public class HealthCheckTests(LibraryApiFactory factory) : IClassFixture<LibraryApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task HealthCheck_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("Healthy");
    }

    [Fact]
    public async Task Scalar_ReturnsContent()
    {
        var response = await _client.GetAsync("/scalar/v1");
        var content = await response.Content.ReadAsStringAsync();
        throw new Exception($"STATUS: {response.StatusCode}, CONTENT_LENGTH: {content.Length}, PREVIEW: {content[..Math.Min(content.Length, 300)]}");
    }

}
