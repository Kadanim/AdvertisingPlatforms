using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AdvertisingPlatforms.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            context.Result = new BadRequestObjectResult($"Ошибка: {context.Exception.Message}");
            context.ExceptionHandled = true;
        }
    }
}