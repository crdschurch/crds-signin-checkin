using Newtonsoft.Json.Linq;
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
        public static JObject Report(string route, JArray problems = null)
        {
            dynamic routeReport = new JObject();
            routeReport.Route = route;

            if (routes.ContainsKey(route))
            {
                VersionSpace versionSpace = routes[route];
                versionSpace.Report(routeReport);
            }
            else
                routeReport.Presence("not found");

            int count = 0; // bogus - no JObject way to see how many properties are present
            foreach (var property in routeReport.Properties())
                count++;
            if (count > 1)
            {
                if (problems != null)
                    problems.Add(routeReport);
                return routeReport;
            }
            else
                return null;
        }

        internal static int Count()
        {
            return routes.Count;
        }

        // returns a summary of any version problem for each added route
        public static JArray Problems()
        {
            JArray problems = new JArray();

            foreach (var route in routes.Keys)
                Report(route, problems);

            return problems;
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
        public JObject Report()
        {
            return Report(new JObject());
        }
        public JObject Report(dynamic report)
        {
            string state = null;
            SemanticVersion current = null;
            VersionConstraint versionConstraint;
            JArray states = new JArray();
            JArray gaps = new JArray();
            JArray overlaps = new JArray();

            if (!hasDefault)
                report.Default = "none";

            foreach (var constraint in versionConstraints)
            {
                versionConstraint = constraint.Value;

                if (current == null)
                {
                    if (!hasDefault && versionConstraint.MinimumVersion.IsAfter(origin))
                        report.origin = versionConstraint.MinimumVersion;
                }
                else
                {
                    if (state != null)
                    {
                        if (state == "live" && versionConstraint.Removed)
                            states.Add("live to removed at " + versionConstraint.MinimumVersion);
                        if (state == "live" && versionConstraint.Deprecated)
                            states.Add("live to deprecated at " + versionConstraint.MinimumVersion);
                        if (state == "deprecated" && versionConstraint.Removed)
                            states.Add("deprecated to removed at " + versionConstraint.MinimumVersion);
                    }

                    if (versionConstraint.MinimumVersion.IsAfter(current))
                        gaps.Add("between " + current + " and " + versionConstraint.MinimumVersion);
                    else if (versionConstraint.MinimumVersion.IsBefore(current))
                        overlaps.Add("between " + versionConstraint.MinimumVersion + " and " + current);
                }

                current = versionConstraint.MaximumVersion;
                state = versionConstraint.Removed ? "removed" :
                        versionConstraint.Deprecated ? "deprecated" : "live";
            }

            if (states.Count > 0)
                report.States = states;
            if (gaps.Count > 0)
                report.Gaps = gaps;
            if (overlaps.Count > 0)
                report.Overlaps = overlaps;

            return report;
        }
    }
}