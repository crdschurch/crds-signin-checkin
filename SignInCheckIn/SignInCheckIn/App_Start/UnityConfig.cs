using System.Configuration;
using System.Linq;
using Microsoft.Practices.Unity;
using System.Web.Http;
using Microsoft.Practices.Unity.Configuration;
using Unity.WebApi;
using Crossroads.Web.Common.Configuration;
using MinistryPlatform.Translation.Repositories.Interfaces;
using SignInCheckIn.Services;
using SignInCheckIn.Services.Interfaces;
using MinistryPlatform.Translation.Repositories;

namespace SignInCheckIn
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();
            //var unitySections = new[] { "crossroadsCommonUnity", "crossroadsClientApiKeysUnity", "unity" };

            //foreach (var section in unitySections.Select(sectionName => (UnityConfigurationSection)ConfigurationManager.GetSection(sectionName)))
            //{
            //    container.LoadConfiguration(section);
            //}

            //GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);

            //var container = new Microsoft.Practices.Unity.UnityContainer();
            CrossroadsWebCommonConfig.Register(container);

            //var container = new UnityContainer();
            //CrossroadsWebCommonConfig.Register(container);
            //var unitySections = new[] { "crossroadsCommonUnity", "unity" };

            //foreach (var section in unitySections.Select(sectionName => (UnityConfigurationSection)ConfigurationManager.GetSection(sectionName)))
            //{
            //    container.LoadConfiguration(section);
            //}

            container.RegisterType<ISiteRepository, SiteRepository>();

            container.RegisterType<IHelloWorldService, HelloWorldService>();
            container.RegisterType<IFamilyService, FamilyService>();
            container.RegisterType<ISiteService, SiteService>();

            

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}