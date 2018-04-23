using Crossroads.Web.Common.Configuration;
using Microsoft.Practices.Unity;
using System.Web.Http;
using Unity.WebApi;
using System.Linq;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;

namespace MinistryPlatform.Translation
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            //var container = new UnityContainer();

            //         // register all your components with the container here
            //         // it is NOT necessary to register your controllers

            //         // e.g. container.RegisterType<ITestService, TestService>();

            //         GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);

            var container = new UnityContainer();
            CrossroadsWebCommonConfig.Register(container);
            //var unitySections = new[] { "crossroadsCommonUnity", "unity" };

            //foreach (var section in unitySections.Select(sectionName => (UnityConfigurationSection)ConfigurationManager.GetSection(sectionName)))
            //{
            //    container.LoadConfiguration(section);
            //}

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}