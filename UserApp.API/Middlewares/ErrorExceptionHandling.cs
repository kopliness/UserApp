using System.Net;
using BusinessLayer.DTO;
using UserApp.Common.Extensions;

namespace UserApp.API.Middlewares;

public class ErrorExceptionHandling
{
    private readonly ILogger<ErrorExceptionHandling> _logger;

    private readonly RequestDelegate _requestDelegate;

    public ErrorExceptionHandling(RequestDelegate requestDelegate, ILogger<ErrorExceptionHandling> logger)
    {
        _requestDelegate = requestDelegate;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _requestDelegate(context);
        }
        catch (UserExistsException e)
        {
            await HandleExceptionAsync(context,
                e.Message,
                HttpStatusCode.BadRequest,
                e.Message);
        }
        catch (NotFoundException e)
        {
            await HandleExceptionAsync(context,
                e.Message,
                HttpStatusCode.NotFound,
                e.Message);
        }
        catch (IncorrectRolesException e)
        {
            await HandleExceptionAsync(context,
                e.Message,
                HttpStatusCode.BadRequest,
                e.Message);
        }
        catch (AgeRangeException e)
        {
            await HandleExceptionAsync(context,
                e.Message,
                HttpStatusCode.UnprocessableEntity,
                e.Message);
        }
        
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());

            _logger.LogTrace(e.InnerException.ToString());

            await HandleExceptionAsync(context,
                e.Message,
                HttpStatusCode.InternalServerError,
                e.Message);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, string exMsg, HttpStatusCode httpStatusCode,
        string message)
    {
        var response = context.Response;
        response.ContentType = "application/json";
        response.StatusCode = (int)httpStatusCode;

        var errorDto = new ErrorDto
        {
            StatusCode = (int)httpStatusCode,
            Message = message
        };

        await response.WriteAsJsonAsync(errorDto);
    }
}