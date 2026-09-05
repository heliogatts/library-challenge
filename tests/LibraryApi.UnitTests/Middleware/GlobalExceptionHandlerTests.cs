using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using LibraryApi.Domain.Exceptions;
using LibraryApi.Shared.Middleware;
using Npgsql;

namespace LibraryApi.UnitTests.Middleware;

public class GlobalExceptionHandlerTests
{
    private class FakeProblemDetailsService : IProblemDetailsService
    {
        public ProblemDetailsContext? CapturedContext { get; private set; }

        public ValueTask<bool> TryWriteAsync(ProblemDetailsContext context)
        {
            CapturedContext = context;
            return ValueTask.FromResult(true);
        }

        public ValueTask WriteAsync(ProblemDetailsContext context)
        {
            CapturedContext = context;
            return ValueTask.CompletedTask;
        }
    }

    private readonly FakeProblemDetailsService _problemDetailsService = new();
    private readonly GlobalExceptionHandler _handler;

    public GlobalExceptionHandlerTests()
    {
        _handler = new GlobalExceptionHandler(
            _problemDetailsService,
            NullLogger<GlobalExceptionHandler>.Instance
        );
    }

    [Fact]
    public async Task TryHandleAsync_WithDomainException_MapsTo422UnprocessableEntity()
    {
        var httpContext = new DefaultHttpContext();
        var exception = new DomainException("Custom domain rule was broken.");

        var handled = await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        handled.Should().BeTrue();
        httpContext.Response.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
        _problemDetailsService.CapturedContext.Should().NotBeNull();
        _problemDetailsService.CapturedContext!.ProblemDetails.Status.Should().Be(StatusCodes.Status422UnprocessableEntity);
        _problemDetailsService.CapturedContext.ProblemDetails.Title.Should().Be("Domain Rule Violation");
        _problemDetailsService.CapturedContext.ProblemDetails.Detail.Should().Be("Custom domain rule was broken.");
    }

    [Fact]
    public async Task TryHandleAsync_WithGenericException_MapsTo500InternalServerError()
    {
        var httpContext = new DefaultHttpContext();
        var exception = new InvalidOperationException("Unexpected internal failure");

        var handled = await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        handled.Should().BeTrue();
        httpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        _problemDetailsService.CapturedContext.Should().NotBeNull();
        _problemDetailsService.CapturedContext!.ProblemDetails.Status.Should().Be(StatusCodes.Status500InternalServerError);
        _problemDetailsService.CapturedContext.ProblemDetails.Title.Should().Be("Internal Server Error");
    }
}
