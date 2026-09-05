using Serilog;
using Serilog.Formatting.Compact;
using Scalar.AspNetCore;
using LibraryApi.Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Serilog — structured JSON logging
builder.Host.UseSerilog((context, config) =>
{
    config
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console(new RenderedCompactJsonFormatter());
});

// Application services (DB, Validation, CORS, Error Handling)
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Middleware pipeline
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseSerilogRequestLogging();
app.UseCors();

// Apply migrations and seed data
await app.ApplyMigrationsAsync();

// OpenAPI & API Reference
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.WithOpenApiRoutePattern("/openapi/{documentName}.json");
});


// Redirect root to interactive Scalar API documentation
app.MapGet("/", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();

// Health Check
app.MapHealthChecks("/health");

// Map all feature endpoints
app.MapFeatureEndpoints();

app.Run();

// Make Program accessible for WebApplicationFactory in tests
public partial class Program;
