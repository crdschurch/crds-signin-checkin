using System.Configuration;
using Microsoft.Practices.Unity;
using System.Web.Http;
using Microsoft.Practices.Unity.Configuration;
using Unity.WebApi;

namespace SignInCheckIn
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            var container = new UnityContainer();

            section.Configure(container);

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}