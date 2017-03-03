﻿using System.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;
using Crossroads.ApiVersioning;
using Crossroads.ClientApiKeys;

namespace SignInCheckIn
{
    public static class WebApiConfig
    {
        public static ApiRouteProvider ApiRouteProvider = new ApiRouteProvider();

        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // API Versioning
            VersionConfig.Register(config);

            // Configure CORS
            var cors = new EnableCorsAttribute(ConfigurationManager.AppSettings["CORS"], "*", "*");
            cors.SupportsCredentials = true;
            config.EnableCors(cors);

            // Add a filter to verify domain-locked API keys
            var domainLockedApiKeyFilter = (DomainLockedApiKeyFilter)config.DependencyResolver.GetService(typeof(DomainLockedApiKeyFilter));
            config.Filters.Add(domainLockedApiKeyFilter);
        }
    }
}