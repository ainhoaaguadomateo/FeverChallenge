namespace Fever.Presentation.Middlewares;

public class HandleExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<HandleExceptionMiddleware> _logger;

    public HandleExceptionMiddleware(
        RequestDelegate next,
        ILogger<HandleExceptionMiddleware> logger
    )
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        _logger.LogError(ex, "An exception occurred.");

        var response = new
        {
            error = new
            {
                code = ex.Source,
                message = ex.Message ?? "An internal server error occurred.",
            },
            data = (object?)null,
        };

        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(response);
    }
}
