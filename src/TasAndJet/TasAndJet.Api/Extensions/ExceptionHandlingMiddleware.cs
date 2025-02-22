using System.Text.Json;
using SharedKernel.Common;

namespace TasAndJet.Api.Extensions;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Произошло необработанное исключение");

            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var error = Error.Failure("UnhandledException", exception.Message);

        var envelope = Envelope.Error(error.ToErrorList());

        var result = JsonSerializer.Serialize(envelope);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        return context.Response.WriteAsync(result);
    }
}
