using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FirstApi.ValidationAttributes;

public class ApiValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(new
            {
                Code = 400,
                Title= "One or more validation errors occurred.",
                Errors = context.ModelState.Values.SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage)
            });
        }

        base.OnActionExecuting(context);
    }
}