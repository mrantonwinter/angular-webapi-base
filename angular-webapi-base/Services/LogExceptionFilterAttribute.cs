using System.Web.Http.Filters;

namespace angular_webapi_base.Services
{
   public class LogExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger("MyProject");

        public override void OnException(HttpActionExecutedContext context)
        {
            _logger.Error("Exception", context.Exception);
            base.OnException(context);
        }
    }
}

