using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FirstApi.Controllers;

public class ErrorsController : Controller
{
    [Route("/error")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult Error()
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
 
        var exception = context?.Error;

        var problemDetails =  new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Detail = exception!=null? exception.Message : "an error occurred while processing your request",
            Title="Server Error"
        };
        
        if (exception is UnauthorizedAccessException)
        {
            problemDetails.Status = StatusCodes.Status401Unauthorized;
            problemDetails.Title = "Unauthorized access";
        }

        // Add additional cases for other exception types as needed

        return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
    }
    
}