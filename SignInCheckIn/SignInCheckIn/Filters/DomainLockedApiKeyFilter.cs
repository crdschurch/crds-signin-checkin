using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.Cors;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using MinistryPlatform.Translation.Models.Attributes;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Newtonsoft.Json;

namespace SignInCheckIn.Filters
{
    public class DomainLockedApiKeyFilter : ActionFilterAttribute
    {
        public const string ApiKeyHeader = "Crds-Api-Key";

        private readonly List<DomainLockedApiKey> _apiKeys;
        private readonly ICorsEngine _corsEngine;

        public DomainLockedApiKeyFilter(IMinistryPlatformRestRepository ministryPlatformRestRepository, ICorsEngine corsEngine, IApiUserRepository apiUserRepository)
        {
            _apiKeys = ministryPlatformRestRepository.UsingAuthenticationToken(apiUserRepository.GetToken()).Search<DomainLockedApiKey>();
            _corsEngine = corsEngine;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.Request.Headers.Contains(ApiKeyHeader))
            {
                base.OnActionExecuting(actionContext);
                return;
            }

            var apiKeyValue = actionContext.Request.Headers.GetValues(ApiKeyHeader).First();

            var key = _apiKeys.Find(k => k.Key.Equals(new Guid(apiKeyValue)));
            if (key == null)
            {
                actionContext.Response = CreateResponseMessage(HttpStatusCode.Forbidden, $"Unknown API Key {apiKeyValue}");
                throw new HttpResponseException(actionContext.Response);
            }

            if (!key.Origins.Any())
            {
                base.OnActionExecuting(actionContext);
                return;
            }

            var remoteHost = GetRemoteHost(actionContext.Request);
            var corsContext = new CorsRequestContext
            {
                Host = remoteHost,
                Origin = remoteHost,
                RequestUri = actionContext.Request.RequestUri,
            };

            var policy = new CorsPolicy();
            key.Origins.ForEach(origin => policy.Origins.Add(origin));

            var result = _corsEngine.EvaluatePolicy(corsContext, policy);
            if (result.IsValid)
            {
                base.OnActionExecuting(actionContext);
                return;
            }

            actionContext.Response = CreateResponseMessage(HttpStatusCode.Forbidden, $"API Key {key.Key} is not allowed from origin {corsContext.Origin}");
            throw new HttpResponseException(actionContext.Response);
        }

        private static HttpResponseMessage CreateResponseMessage(HttpStatusCode statusCode, string reasonPhrase)
        {
            var response = new HttpResponseMessage(statusCode) { ReasonPhrase = reasonPhrase };
            return response;
        }

        private static string GetRemoteHost(HttpRequestMessage request)
        {
            // Try X-Forwarded-For first - this should be set if coming through apache mod_proxy (or any proxy/load-balancer)
            if (request.Headers.Contains("X-Forwarded-For"))
            {
                return request.Headers.GetValues("X-Forwarded-For").First();
            }

            // Try Referer next - this may be set if coming from a frontend like crossroads.net
            if (request.Headers.Referrer != null)
            {
                return request.Headers.Referrer.Host;
            }

            // Now try some more expensive stuff - get User Host Name from the request, if it is there
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostName;
            }

            // Ok, even more expensive - get the remote endpoint IP address, then lookup the associated hostname
            if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                var requestEndpoint = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
                return Dns.GetHostEntry(requestEndpoint.Address).HostName;
            }

            // Last resort
            return null;
        }
    }

    [MpRestApiTable(Name = "cr_Client_Api_Keys")]
    public class DomainLockedApiKey
    {
        [JsonProperty("_Api_Key")]
        public Guid Key { get; set; }
        [JsonProperty("Client_Name")]
        public string ClientName { get; set; }
        [JsonProperty("Start_Date")]
        public DateTime StartDate { get; set; }
        [JsonProperty("End_Date")]
        public DateTime? EndDate { get; set; }
        [JsonProperty("Allowed_Domains")]
        public string AllowedDomains
        {
            get
            {
                return string.Join(", ", Origins.ToArray());
            }
            set
            {
                if (value == null)
                {
                    Origins = new List<string>();
                    return;
                }
                Origins = value.Split(',').Select(s => s.Trim()).ToList();
            }
        }

        [JsonIgnore]
        public List<string> Origins { get; set; } = new List<string>();
    }
}