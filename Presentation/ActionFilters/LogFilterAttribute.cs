using Entities.LogModel;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Services.Contracts;

namespace Presentation.ActionFilters
{
    public class LogFilterAttribute : ActionFilterAttribute
    {
        private readonly ILoggerService _logger;

        public LogFilterAttribute(ILoggerService logger)
        {
            _logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInfo(Log("OnActionExecuting", context.RouteData));
        }

        private string Log(string modalName, RouteData routeData)
        {
            var logDetails = new LogDetail()
            {
                ModalModel = modalName,
                Controller = routeData.Values["controller"],
                Action = routeData.Values["action"]
            };

            if(routeData.Values.Count >=3)
            {
                logDetails.Id=routeData.Values["id"];
            }

            return logDetails.ToString();
        }
    }
}
