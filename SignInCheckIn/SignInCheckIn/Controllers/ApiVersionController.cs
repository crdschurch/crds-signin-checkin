using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Routing;
using Newtonsoft.Json;
using SignInCheckIn.ApiVersioning;
using SignInCheckIn.ApiVersioning.Attributes;

namespace SignInCheckIn.Controllers
{
    public class ApiVersionController : ApiController
    {
        [HttpGet]
        [ResponseType(typeof(IList<ApiVersion>))]
        [Route("activeVersion")]
        public IHttpActionResult GetActiveVersion([FromUri(Name = "call")] string call = null)
        {
            var active = GetActiveRoutes(call);

            var results = active.Select(r => new ApiVersion
            {
                Deprecated = r.Value.Deprecated,
                MinimumVersion = r.Value.MinimumVersion.ToString(),
                MaximumVersion = r.Value.MaximumVersion?.ToString(),
                Endpoint = r.Key
            }).ToList();
            return Ok(results);
        }

        [HttpGet]
        [Route("activeUse")]
        public IHttpActionResult GetActiveUse([FromUri(Name = "call")] string call = null)
        {
            var callName = call ?? "all calls";
            return Ok($@"Active use for {callName}");
        }

        private static Dictionary<string, VersionConstraint> GetActiveRoutes(string call)
        {
            var results = new Dictionary<string, VersionConstraint>();
            foreach (
                var route in
                    GlobalConfiguration.Configuration.Routes.SelectMany(
                        routeCollection =>
                            ((IEnumerable<IHttpRoute>) routeCollection).Where(
                                route => string.IsNullOrWhiteSpace(call) || route.RouteTemplate.EndsWith(call))))
            {
                if (!results.ContainsKey(route.RouteTemplate))
                {
                    results.Add(route.RouteTemplate,
                        route.Constraints.Any() && route.Constraints.ContainsKey(VersionedRoute.VersionedRouteConstraint)
                            ? (VersionConstraint) route.Constraints[VersionedRoute.VersionedRouteConstraint]
                            : new VersionConstraint(new SemanticVersion("1.0.0"), new SemanticVersion("1.0.0"), false));
                }
                else
                {
                    var current = results[route.RouteTemplate];
                    var constraint = route.Constraints.Any() && route.Constraints.ContainsKey(VersionedRoute.VersionedRouteConstraint)
                        ? (VersionConstraint) route.Constraints[VersionedRoute.VersionedRouteConstraint]
                        : new VersionConstraint(new SemanticVersion("1.0.0"), new SemanticVersion("1.0.0"), false);
                    if (constraint.CompareTo(current) > 0)
                    {
                        results[route.RouteTemplate] = constraint;
                    }
                }
            }
            return results;
        }
    }

    class ApiVersion
    {
        [JsonProperty("endpoint")]
        public string Endpoint { get; set; }
        [JsonProperty("minimumVersion")]
        public string MinimumVersion { get; set; }
        [JsonProperty(PropertyName = "maximumVersion", NullValueHandling = NullValueHandling.Ignore)]
        public string MaximumVersion { get; set; }
        [JsonProperty("deprecated")]
        public bool Deprecated { get; set; }
    }
}