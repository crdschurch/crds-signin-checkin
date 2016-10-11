using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace SignInCheckIn.ApiVersioning
{
    public class ApiRouteProvider : DefaultDirectRouteProvider
    {
        private const string ApiRoutePrefix = "api";

        public IReadOnlyList<RouteEntry> DirectRoutes { get; private set; }

        public override IReadOnlyList<RouteEntry> GetDirectRoutes(HttpControllerDescriptor controllerDescriptor, IReadOnlyList<HttpActionDescriptor> actionDescriptors,
            IInlineConstraintResolver constraintResolver)
        {
            DirectRoutes = base.GetDirectRoutes(controllerDescriptor, actionDescriptors, constraintResolver);
            DetermineRouteIntegrity();
            return DirectRoutes;
        }

        private static bool _routeIntegrityDetermined = false;

        private void DetermineRouteIntegrity()
        {
            if (_routeIntegrityDetermined)
                return;
            _routeIntegrityDetermined = true;

            foreach(RouteEntry routeEntry in DirectRoutes)
            {
                string routePath = routeEntry.Route.RouteTemplate;
                string route = routeBasename(routePath);
                VersionConstraint constraint = null;
                if (routeEntry.Route.Constraints.ContainsKey("allowedVersions"))
                    constraint = routeEntry.Route.Constraints["allowedVersions"] as VersionConstraint;
                VersionSpace.Add(route, constraint);
            }
            int routeCount = VersionSpace.Count();
            JArray problems = VersionSpace.Problems();
            System.Diagnostics.Debug.WriteLine("-----------------------------------");
            if (problems.Count > 0)
                System.Diagnostics.Debug.WriteLine("API Versioning (" + routeCount + " routes) Problems: " + problems);
            else
                System.Diagnostics.Debug.WriteLine("API Versioning (" + routeCount + " routes) is OK");
            System.Diagnostics.Debug.WriteLine("-----------------------------------");
        }

        private const string _versionedRoutePattern = @"^api/v\{apiVersion\}/(.*)$";
        private readonly Regex _versionedRouteRegex = new Regex(_versionedRoutePattern);
        private const string _unversionedRoutePattern = @"^api/(.*)$";
        private readonly Regex _unversionedRouteRegex = new Regex(_unversionedRoutePattern);

        private string routeBasename(string routePath)
        {
            var match = _versionedRouteRegex.Match(routePath);
            if (!match.Success)
            {
                match = _unversionedRouteRegex.Match(routePath);
                if (!match.Success)
                    return "";
            }
            return match.Groups[1].Captures[0].Value;
        }

        protected override string GetRoutePrefix(HttpControllerDescriptor controllerDescriptor)
        {
            var existingPrefix = base.GetRoutePrefix(controllerDescriptor);
            return existingPrefix == null ? ApiRoutePrefix : $"{ApiRoutePrefix}/{existingPrefix}";
        }
    }
}