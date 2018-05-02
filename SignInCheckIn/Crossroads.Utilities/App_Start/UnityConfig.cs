using Crossroads.Utilities.Services;
using Crossroads.Utilities.Services.Interfaces;
using Microsoft.Practices.Unity;
using System.Web.Http;
using Unity.WebApi;

namespace Crossroads.Utilities
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            // container.RegisterType<IApplicationConfiguration, ApplicationConfiguration>;            
            container.RegisterType<IApplicationConfiguration, ApplicationConfiguration>();
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}