using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FirstApi.Controllers;

public class ErrorsController : Controller
{
    [Route("/error")]
    [HttpGet]
    public IActionResult Error()
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
 
        var exception = context?.Error;

        var problemDetails =  new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = exception!=null? exception.Message:"Server Error"
        };

        return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
    }
    
}