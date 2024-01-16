using FirstApi.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FirstApi.Controllers;

public class ErrorsController : Controller
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public ErrorsController(    IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }
    [Route("/error")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult Error()
    {
        var context = httpContextAccessor.HttpContext?.Features.Get<IExceptionHandlerFeature>();
        var exception = context?.Error;
        var problemDetails = SpecificProblem(exception);
        return StatusCode(problemDetails.Status ?? StatusCodes.Status500InternalServerError, problemDetails);
    }

    private ProblemDetails SpecificProblem(Exception? exception)
    {
        exception = exception is AggregateException ? exception.GetBaseException() : exception;

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Detail = exception != null ? exception.Message : "An error occurred",
            Title = "Server Error"
        };
        switch (exception)
        {
            case UnauthorizedAccessException:
                problemDetails.Status = StatusCodes.Status401Unauthorized;
                problemDetails.Title = "Unauthorized access";
                break;
            case KeyNotFoundException:
                problemDetails.Status = StatusCodes.Status404NotFound;
                problemDetails.Title = "Not found";
                break;
        }

        return problemDetails;
    }
}
