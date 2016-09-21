using System.Web.Http;
using SDammann.WebApi.Versioning.Configuration;
using SDammann.WebApi.Versioning.Request;

namespace SignInCheckIn
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            ApiVersioning.Configure(config)
             .ConfigureRequestVersionDetector<DefaultRouteKeyVersionDetector>();
        }
    }
}
