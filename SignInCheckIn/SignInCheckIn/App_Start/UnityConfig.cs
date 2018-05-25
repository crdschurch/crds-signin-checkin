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
            // this register unity config content that was previous registered in the unity sections area
            var container = new UnityContainer();
            CrossroadsWebCommonConfig.Register(container);

            var unitySections = new[] { "unity" };

            foreach (var section in unitySections.Select(sectionName => (UnityConfigurationSection)ConfigurationManager.GetSection(sectionName)))
            {
                container.LoadConfiguration(section);
            }

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}