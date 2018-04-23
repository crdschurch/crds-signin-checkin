using System.Configuration;
using System.Linq;
using Microsoft.Practices.Unity;
using System.Web.Http;
using Microsoft.Practices.Unity.Configuration;
using Unity.WebApi;
using Crossroads.Web.Common.Configuration;

namespace SignInCheckIn
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            //var container = new UnityContainer();
            //var unitySections = new[] { "crossroadsCommonUnity", "crossroadsClientApiKeysUnity", "unity" };

            //foreach (var section in unitySections.Select(sectionName => (UnityConfigurationSection)ConfigurationManager.GetSection(sectionName)))
            //{
            //    container.LoadConfiguration(section);
            //}

            //GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);

            var container = new UnityContainer();
            CrossroadsWebCommonConfig.Register(container);
            var unitySections = new[] { "crossroadsCommonUnity", "unity" };

            foreach (var section in unitySections.Select(sectionName => (UnityConfigurationSection)ConfigurationManager.GetSection(sectionName)))
            {
                container.LoadConfiguration(section);
            }

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}