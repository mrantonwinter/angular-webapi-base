using angular_webapi_base.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace angular_webapi_base
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //Web API configuration and services
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);


            // Web API routes
            config.MapHttpAttributeRoutes();
            
            // Log
            log4net.Config.XmlConfigurator.Configure();

            GlobalConfiguration.Configuration.Filters.Add(new LogExceptionFilterAttribute());
        }
    }
}
