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

            container.RegisterType<IAttributeRepository, AttributeRepository>();
            container.RegisterType<IChildCheckinRepository, ChildCheckinRepository>();
            container.RegisterType<IChildSigninRepository, ChildSigninRepository>();
            container.RegisterType<IConfigRepository, ConfigRepository>();
            container.RegisterType<IContactRepository, ContactRepository>();
            container.RegisterType<IEventRepository, EventRepository>();
            container.RegisterType<IGroupLookupRepository, GroupLookupRepository>();
            container.RegisterType<IGroupRepository, GroupRepository>();
            container.RegisterType<IKioskRepository, KioskRepository>();
            container.RegisterType<ILookupRepository, LookupRepository>();
            container.RegisterType<IParticipantRepository, ParticipantRepository>();
            container.RegisterType<IRoomRepository, RoomRepository>();
            container.RegisterType<ISiteRepository, SiteRepository>();

            container.RegisterType<IChildCheckinService, ChildCheckinService>();
            container.RegisterType<IChildSigninService, ChildSigninService>();
            container.RegisterType<IEventService, EventService>();
            container.RegisterType<IFamilyService, FamilyService>();
            container.RegisterType<IHelloWorldService, HelloWorldService>();
            container.RegisterType<IKioskService, KioskService>();
            container.RegisterType<ILoginService, LoginService>();
            container.RegisterType<ILookupService, LookupService>();
            container.RegisterType<IRoomService, RoomService>();
            container.RegisterType<ISiteService, SiteService>();
            container.RegisterType<IWebsocketService, WebsocketService>();


            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}