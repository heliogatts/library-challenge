using Microsoft.AspNetCore.Diagnostics;
using Npgsql;

namespace LibraryApi.Shared.Middleware;

public class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);

        var (statusCode, title, detail) = MapException(exception);

        httpContext.Response.StatusCode = statusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails =
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Type = $"https://httpstatuses.com/{statusCode}"
            }
        });
    }

    private static (int StatusCode, string Title, string Detail) MapException(Exception exception)
    {
        return exception switch
        {
            Microsoft.EntityFrameworkCore.DbUpdateException
            {
                InnerException: PostgresException { SqlState: PostgresErrorCodes.ForeignKeyViolation }
            } => (
                StatusCodes.Status409Conflict,
                "Conflict",
                "Cannot delete this record because it has dependent records. Remove all associated books first."
            ),

            Microsoft.EntityFrameworkCore.DbUpdateException
            {
                InnerException: PostgresException { SqlState: PostgresErrorCodes.UniqueViolation }
            } => (
                StatusCodes.Status409Conflict,
                "Conflict",
                "A record with this value already exists."
            ),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred. Please try again later."
            )
        };
    }
}
