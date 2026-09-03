using FluentValidation;
using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;
using LibraryApi.Shared.Middleware;

namespace LibraryApi.Shared.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        const string dockerSecretPath = "/run/secrets/db_password";
        if (File.Exists(dockerSecretPath) && !string.IsNullOrWhiteSpace(connectionString))
        {
            var secretPassword = File.ReadAllText(dockerSecretPath).Trim();
            if (!string.IsNullOrWhiteSpace(secretPassword))
            {
                var csb = new Npgsql.NpgsqlConnectionStringBuilder(connectionString);
                if (string.IsNullOrEmpty(csb.Password))
                {
                    csb.Password = secretPassword;
                    connectionString = csb.ConnectionString;
                }
            }
        }

        services.AddDbContext<LibraryDbContext>(options =>
            options.UseNpgsql(connectionString));

        // FluentValidation — register all validators from this assembly
        services.AddValidatorsFromAssemblyContaining<Program>();

        // Problem Details & Exception Handling
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        // OpenAPI & Observability
        services.AddOpenApi();
        services.AddHealthChecks();

        // CORS
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                    ?? ["http://localhost:4200"];

                policy.WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }
}
