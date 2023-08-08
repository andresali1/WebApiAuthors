using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiAuthors.Filters
{
    public class MyActionFilter : IActionFilter
    {
        private readonly ILogger<MyActionFilter> _logger;

        public MyActionFilter(ILogger<MyActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("Antes de ejecutar la acción");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("Después de ejecutar la acción");
        }
    }
}
