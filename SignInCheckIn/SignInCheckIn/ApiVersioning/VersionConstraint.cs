using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Routing;
using SignInCheckIn.ApiVersioning.Attributes;

namespace SignInCheckIn.ApiVersioning
{
    internal class VersionConstraint : IHttpRouteConstraint, IComparable<VersionConstraint>
    {
        private readonly SemanticVersion _defaultVersion = new SemanticVersion("1.0.0");
        public SemanticVersion MinimumVersion { get; }
        public SemanticVersion MaximumVersion { get; }
        public bool Deprecated { get; }
        public bool Removed { get; }


        public VersionConstraint(SemanticVersion minimumVersion, SemanticVersion maximumVersion, bool deprecated, bool removed)
        {
            if (maximumVersion != null && maximumVersion.IsBefore(minimumVersion))
            {
                throw new ArgumentOutOfRangeException($"Cannot specify a maximumVersion ({maximumVersion}) less than the minimumVersion ({minimumVersion})");
            }

            MinimumVersion = minimumVersion;
            MaximumVersion = maximumVersion;
            Deprecated = deprecated;
            Removed = removed;
        }

        public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            if (routeDirection != HttpRouteDirection.UriResolution)
            {
                return true;
            }

            var version = GetVersion(values) ?? _defaultVersion;

            var matchesVersion = MaximumVersion == null
                ? version.IsFrom(MinimumVersion)
                : version.IsBetween(MinimumVersion, MaximumVersion);

            if (matchesVersion)
            {
                if (Deprecated)
                {
                    request.Properties.Add(new KeyValuePair<string, object>("deprecated", true));
                }
                if (Removed)
                {
                    request.Properties.Add(new KeyValuePair<string, object>("removed", true));
                }
            }

            return matchesVersion;
        }

        public int CompareTo(VersionConstraint other)
        {
            if (other == null)
            {
                return 1;
            }

            var thisMin = MinimumVersion;
            var thisMax = MaximumVersion;
            var otherMin = other.MinimumVersion;
            var otherMax = other.MaximumVersion;

            // If both versions have a max, just compare max (there should never be an overlap between min and max)
            if (otherMax != null && thisMax != null)
            {
                return thisMax.CompareTo(otherMax);
            }

            if (otherMax != null)
            {
                return thisMin.IsFrom(otherMax) ? 1 : thisMin.CompareTo(otherMin);
            }

            if (thisMax != null)
            {
                return otherMin.IsFrom(thisMax) ? -1 : thisMin.CompareTo(otherMin);
            }

            return thisMin.CompareTo(otherMin);
        }

        public override string ToString()
        {
            return $"[{MinimumVersion},{MaximumVersion?.ToString() ?? "latest"}){(Deprecated ? "  **Deprecated**" : "")}";
        }

        private static SemanticVersion GetVersion(IDictionary<string, object> parameters)
        {
            var versionString = parameters[VersionedRoute.ApiVersionParameter];
            return versionString == null ? null : new SemanticVersion(versionString.ToString());
        }
    }
}
