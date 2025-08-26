using System.Text.Json;
using GuestRegistration.Application.Exceptions; // Your custom exceptions

namespace GuestRegistration.Api.Middleware;


public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        int statusCode = StatusCodes.Status500InternalServerError;
        string title = "Internal Server Error";
        string detail = "An unexpected error occurred. Please contact support.";
        
        switch (exception)
        {
            case NotFoundException notFoundException:
                statusCode = StatusCodes.Status404NotFound;
                title = "Resource Not Found";
                detail = notFoundException.Message;
                break;

            case ValidationException validationException:
                statusCode = StatusCodes.Status400BadRequest;
                title = "Validation Error";
                detail = validationException.Message;
                break;
            
            case BusinessRuleValidationException businessRuleException:
                statusCode = StatusCodes.Status400BadRequest;
                title = "Business Rule Violation";
                detail = businessRuleException.Message;
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        
        var errorResponse = new
        {
            StatusCode = statusCode,
            Title = title,
            Detail = detail
        };
        
        var jsonResponse = JsonSerializer.Serialize(errorResponse);
        return context.Response.WriteAsync(jsonResponse);
    }
}

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }
}
