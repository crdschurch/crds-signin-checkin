using System;
using System.Collections.Generic;

namespace SignInCheckIn.ApiVersioning
{
    // reports on gaps in the versions of API calls
    public class VersionSpace
    {
        private static SortedList<string, VersionSpace> routes = new SortedList<string, VersionSpace>();

        private SortedList<SemanticVersion, VersionConstraint> versionConstraints;
        private bool hasDefault;
        private SemanticVersion origin = new SemanticVersion("1");

        private VersionSpace(string route)
        {
            versionConstraints = new SortedList<SemanticVersion, VersionConstraint>();
            hasDefault = false;
        }

        // associates a constraint with a route
        internal static void Add(string route, VersionConstraint constraint)
        {
            if (!routes.ContainsKey(route))
                routes[route] = new VersionSpace(route);
            VersionSpace versionSpace = routes[route];
            versionSpace.Add(constraint);
        }

        // returns a summary of any version gaps in a route
        public static string Report(string route)
        {
            string report = "not found";

            if (routes.ContainsKey(route))
            {
                VersionSpace versionSpace = routes[route];
                report = versionSpace.Report();
            }

            return (report == "") ? "" : ("route " + route + ": " + report);
        }

        // returns a summary of any version gaps for each added route
        // (woe... this should be Report, but you can have instance and class methods
        //  with the same signatures.)
        public static string GapReport()
        {
            List<string> reports = new List<string>();
            string report;

            foreach (var route in routes.Keys)
            {
                report = Report(route);
                if (report != "")
                    reports.Add(report);
            }

            if (reports.Count > 0)
                reports.Insert(0, "Problems were found with the API routes...");

            return string.Join("\n", reports);
        }

        // adds a version constraint
        internal void Add(VersionConstraint constraint)
        {
            if (constraint == null)
                hasDefault = true;
            else
                versionConstraints.Add(constraint.MinimumVersion, constraint);
        }

        // reports on potential versioning problems
        public string Report()
        {
            List<string> reports = new List<string>();
            string state = null;
            SemanticVersion current = null;
            VersionConstraint versionConstraint;

            if (!hasDefault)
                reports.Add("no default version provided");

            foreach (var constraint in versionConstraints)
            {
                versionConstraint = constraint.Value;

                if (current == null)
                {
                    if (!hasDefault && versionConstraint.MinimumVersion.IsAfter(origin))
                        reports.Add("versioning does not begin at 1.0.0");
                }
                else
                {
                    if (state != null)
                    {
                        if (state == "live" && versionConstraint.Removed)
                            reports.Add("transition from live to removed at " + versionConstraint.MinimumVersion);
                        if (state == "live" && versionConstraint.Deprecated)
                            reports.Add("transition from live to deprecated at " + versionConstraint.MinimumVersion);
                        if (state == "deprecated" && versionConstraint.Removed)
                            reports.Add("transition from deprecated to removed at " + versionConstraint.MinimumVersion);
                    }

                    if (versionConstraint.MinimumVersion.IsAfter(current))
                        reports.Add("version gap between " + current + " and " + versionConstraint.MinimumVersion);
                    else if (versionConstraint.MinimumVersion.IsBefore(current))
                        reports.Add("version overlap between " + versionConstraint.MinimumVersion + " and " + current);
                }

                current = versionConstraint.MaximumVersion;
                state = versionConstraint.Removed ? "removed" :
                        versionConstraint.Deprecated ? "deprecated" : "live";
            }

            return string.Join(", ", reports);
        }
    }
}