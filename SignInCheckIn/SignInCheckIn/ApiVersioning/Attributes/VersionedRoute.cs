using System.Collections.Generic;
using System.Web.Http.Routing;

namespace SignInCheckIn.ApiVersioning.Attributes
{
    public class VersionedRoute : RouteFactoryAttribute
    {
        public SemanticVersion MinimumVersion { get; }
        public SemanticVersion MaximumVersion { get; }
        public bool Deprecated { get; }
        public const string ApiVersionParameter = "apiVersion";
        public const string VersionedRouteConstraint = "allowedVersions";

        public VersionedRoute(string template, string minimumVersion, string maximumVersion = null, bool deprecated = false)
            : base($"v{{{ApiVersionParameter}}}/{template}")
        {
            MinimumVersion = new SemanticVersion(minimumVersion);
            MaximumVersion = maximumVersion == null
                ? null
                : new SemanticVersion(maximumVersion);
            Deprecated = deprecated;
        }

        public override IDictionary<string, object> Constraints
        {
            get
            {
                var constraints = new HttpRouteValueDictionary
                {
                    {VersionedRouteConstraint, new VersionConstraint(MinimumVersion, MaximumVersion, Deprecated)}
                };
                return constraints;
            }
        }
    }
}
