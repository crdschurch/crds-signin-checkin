using System.Collections.Generic;
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
            return DirectRoutes;
        }

        protected override string GetRoutePrefix(HttpControllerDescriptor controllerDescriptor)
        {
            var existingPrefix = base.GetRoutePrefix(controllerDescriptor);
            return existingPrefix == null ? ApiRoutePrefix : $"{ApiRoutePrefix}/{existingPrefix}";
        }
    }
}