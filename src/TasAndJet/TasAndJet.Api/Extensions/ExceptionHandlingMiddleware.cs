using System.Text.Json;
using FluentValidation;
using SharedKernel.Common.Api;
using SharedKernel.Common.Exceptions;

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
        ErrorList errorList;
        int statusCode;

        switch (exception)
        {
            case ValidationException validationException:
                errorList = validationException.Errors
                    .Select(e => Error.Validation(e.PropertyName, e.ErrorMessage))
                    .ToList(); // автоматически преобразуется в ErrorList
                statusCode = StatusCodes.Status400BadRequest;
                break;

            case NotFoundException notFoundException:
                errorList = Error.NotFound(null, notFoundException.Message); // implicit
                statusCode = StatusCodes.Status404NotFound;
                break;

            default:
                errorList = Error.Failure("UnhandledException", exception.Message); // implicit
                statusCode = StatusCodes.Status500InternalServerError;
                break;
        }

        var envelope = Envelope.Error(errorList);
        var result = JsonSerializer.Serialize(envelope);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        return context.Response.WriteAsync(result);
    }
}