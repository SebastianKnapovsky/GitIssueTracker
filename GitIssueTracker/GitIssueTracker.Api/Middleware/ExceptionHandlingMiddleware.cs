using System.Net;
using System.Text.Json;

namespace GitIssueTracker.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotSupportedException ex)
        {
            _logger.LogWarning(ex, "Unsupported operation");
            await WriteErrorResponse(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument");
            await WriteErrorResponse(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (HttpRequestException ex)
        {
            var status = ex.StatusCode ?? HttpStatusCode.BadGateway;
            string message;

            switch (status)
            {
                case HttpStatusCode.NotFound:
                    _logger.LogWarning(ex, "Requested resource not found");
                    message = "Requested resource not found.";
                    break;

                case HttpStatusCode.Forbidden:
                case HttpStatusCode.Unauthorized:
                    _logger.LogWarning(ex, "Access denied to external API");
                    message = "Access to the resource is denied.";
                    break;

                default:
                    _logger.LogError(ex, "Unexpected external API error");
                    message = "Upstream API error.";
                    break;
            }

            await WriteErrorResponse(context, status, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteErrorResponse(context, HttpStatusCode.InternalServerError, "Unexpected error occurred.");
        }
    }

    private static async Task WriteErrorResponse(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var error = new { error = message };
        await context.Response.WriteAsync(JsonSerializer.Serialize(error));
    }
}
