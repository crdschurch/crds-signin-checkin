using System;
using System.Text.RegularExpressions;

namespace SignInCheckIn.ApiVersioning
{
    public class SemanticVersion : IComparable<SemanticVersion>
    {
        private const string SemanticVersionRegexPatterm = @"^(\d+)\.(\d+)\.(\d+)";
        private readonly Regex _semanticVersionRegex = new Regex(SemanticVersionRegexPatterm);
        public Version Version { get; }
        public SemanticVersion(string version)
        {
            var match = _semanticVersionRegex.Match(version + ".0.0");
            if (!match.Success || match.Groups.Count != 4)
            {
                throw new ArgumentException($"Version provided ({version}) is not valid");
            }
            var groups = match.Groups;
            var major = int.Parse(groups[1].Captures[0].Value);
            var minor = int.Parse(groups[2].Captures[0].Value);
            var patch = int.Parse(groups[3].Captures[0].Value);
            Version = new Version(major, minor, patch);
        }

        public bool IsBetween(string fromVersion, string toVersion)
        {
            return IsBetween(new SemanticVersion(fromVersion), new SemanticVersion(toVersion));
        }

        public bool IsBetween(SemanticVersion fromVersion, SemanticVersion toVersion)
        {
            return IsFrom(fromVersion) && IsBefore(toVersion);
        }

        public bool IsFrom(string fromVersion)
        {
            return IsFrom(new SemanticVersion(fromVersion));
        }

        public bool IsFrom(SemanticVersion fromVersion)
        {
            return Version.CompareTo(fromVersion.Version) >= 0;
        }

        public bool IsAfter(string afterVersion)
        {
            return IsAfter(new SemanticVersion(afterVersion));
        }

        public bool IsAfter(SemanticVersion afterVersion)
        {
            return Version.CompareTo(afterVersion.Version) > 0;
        }

        public bool IsBefore(string beforeVersion)
        {
            return IsBefore(new SemanticVersion(beforeVersion));
        }

        public bool IsBefore(SemanticVersion beforeVersion)
        {
            return Version.CompareTo(beforeVersion.Version) < 0;
        }

        public int CompareTo(SemanticVersion other)
        {
            if (other == null || IsAfter(other))
                return 1;
            else if (IsBefore(other))
                return -1;
            else
                return 0;
        }

        public override string ToString()
        {
            return Version.ToString();
        }
    }
}