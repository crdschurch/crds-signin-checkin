using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Cors;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using log4net;
using MinistryPlatform.Translation.Models.Attributes;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Newtonsoft.Json;

namespace SignInCheckIn.Filters
{
    public class DomainLockedApiKeyFilter : ActionFilterAttribute
    {
        public const string ApiKeyHeader = "Crds-Api-Key";

        public IReadOnlyList<DomainLockedApiKey> RegisteredKeys => _apiKeys.AsReadOnly();

        private readonly List<DomainLockedApiKey> _apiKeys = new List<DomainLockedApiKey>();

        private readonly IMinistryPlatformRestRepository _ministryPlatformRestRepository;
        private readonly ICorsEngine _corsEngine;
        private readonly IApiUserRepository _apiUserRepository;

        private readonly ILog _logger = LogManager.GetLogger(typeof (DomainLockedApiKeyFilter));
        private readonly ILog _auditLogger = LogManager.GetLogger("EndpointAuditLog");

        public DomainLockedApiKeyFilter(IMinistryPlatformRestRepository ministryPlatformRestRepository, ICorsEngine corsEngine, IApiUserRepository apiUserRepository)
        {
            _ministryPlatformRestRepository = ministryPlatformRestRepository;
            _corsEngine = corsEngine;
            _apiUserRepository = apiUserRepository;
            ReloadKeys();
        }

        public void ReloadKeys()
        {
            _logger.Info("Reloading Domain-locked API keys...");
            _apiKeys.Clear();
            try
            {
                _apiKeys.AddRange(_ministryPlatformRestRepository.UsingAuthenticationToken(_apiUserRepository.GetToken())
                        .Search<DomainLockedApiKey>());
                _logger.Info("Successfully reloaded Domain-locked API keys");
            }
            catch (Exception e)
            {
                _logger.Fatal("Could not load API Keys from MinistryPlatform - will default to rejecting all requests!", e);
            }
        }

        private void AuditLog(string endpoint, string method, string remoteHost, string apiKey, bool allowed)
        {
            _auditLogger.Info($"{endpoint},{method},{remoteHost ?? "N/A"},{apiKey ?? "N/A"},{allowed}");
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var endpoint = actionContext.Request.RequestUri.AbsolutePath;
            var method = actionContext.Request.Method.ToString();
            var remoteHost = GetRemoteHost(actionContext.Request);

            _logger.Debug($"Looking for API header '{ApiKeyHeader}' in request");
            if (!actionContext.Request.Headers.Contains(ApiKeyHeader))
            {
                _logger.Debug($"API header '{ApiKeyHeader}' not found in request - allowing request");
                AuditLog(endpoint, method, remoteHost, null, true);
                base.OnActionExecuting(actionContext);
                return;
            }

            var apiKeyValue = actionContext.Request.Headers.GetValues(ApiKeyHeader).First();
            _logger.Debug($"API key '{apiKeyValue}' found in request, will validate against known API keys");

            if (remoteHost == null)
            {
                _logger.Debug("No remote host found in request, cannot verify - rejecting request");
                AuditLog(endpoint, method, null, apiKeyValue, false);
                actionContext.Response = CreateResponseMessage(HttpStatusCode.BadRequest, $"Cannot verify access for request origin");
                throw new HttpResponseException(actionContext.Response);
            }

            var key = _apiKeys.Find(k => k.Key.Equals(new Guid(apiKeyValue)));
            if (key == null)
            {
                _logger.Debug($"API key '{apiKeyValue}' found in request, but does not match a known key - rejecting request");
                AuditLog(endpoint, method, remoteHost, apiKeyValue, false);
                actionContext.Response = CreateResponseMessage(HttpStatusCode.Forbidden, $"Unknown API Key {apiKeyValue}");
                throw new HttpResponseException(actionContext.Response);
            }

            if (!key.IsActive)
            {
                _logger.Debug($"API key '{apiKeyValue}' found in request, but matches an inactive/expired key - rejecting request");
                AuditLog(endpoint, method, remoteHost, apiKeyValue, false);
                actionContext.Response = CreateResponseMessage(HttpStatusCode.Forbidden, $"Inactive/expired API Key {apiKeyValue}");
                throw new HttpResponseException(actionContext.Response);
            }

            if (!key.Origins.Any())
            {
                _logger.Debug($"API key '{apiKeyValue}' found in request, and matching key allows any origin - accepting request");
                AuditLog(endpoint, method, remoteHost, apiKeyValue, true);
                base.OnActionExecuting(actionContext);
                return;
            }

            var corsContext = new CorsRequestContext
            {
                Host = actionContext.Request.Headers.Host,
                Origin = remoteHost,
                RequestUri = actionContext.Request.RequestUri,
            };

            var policy = new CorsPolicy
            {
                AllowAnyHeader = true,
                AllowAnyMethod = true
            };
            key.Origins.ForEach(origin => policy.Origins.Add(origin));

            var result = _corsEngine.EvaluatePolicy(corsContext, policy);
            _logger.Debug($"Evaluating API key '{apiKeyValue}' from host '{remoteHost}' against allowed origins [{string.Join(",", key.Origins)}]");
            if (result.IsValid)
            {
                _logger.Debug($"API key '{apiKeyValue}' found in request, and request origin '{remoteHost}' found in allowed origins [{string.Join(",", key.Origins)}] - accepting request");
                AuditLog(endpoint, method, remoteHost, apiKeyValue, true);
                base.OnActionExecuting(actionContext);
                return;
            }

            _logger.Debug($"API key '{apiKeyValue}' found in request, but does not match allowed origins [{string.Join(",", key.Origins)}] - rejecting request");
            AuditLog(endpoint, method, remoteHost, apiKeyValue, false);
            actionContext.Response = CreateResponseMessage(HttpStatusCode.Forbidden, $"API Key {key.Key} is not allowed from origin {corsContext.Origin}");
            throw new HttpResponseException(actionContext.Response);
        }

        private static HttpResponseMessage CreateResponseMessage(HttpStatusCode statusCode, string reasonPhrase)
        {
            var response = new HttpResponseMessage(statusCode) { ReasonPhrase = reasonPhrase };
            return response;
        }

        private string GetRemoteHost(HttpRequestMessage request)
        {
            // Just look at Origin for now - this will always exist from javascript/angular frontend, and even from Postman
            if (request.Headers.Contains("Origin"))
            {
                var origin = new Uri(request.Headers.GetValues("Origin").First());
                var remoteHost = $"{origin.Host}";
                _logger.Debug($"Found remote host {remoteHost} in Referrer header");
                return remoteHost;
            }

            // TODO - Look for iOS or Android app keys as well

            // No host - we'll reject the request
            _logger.Debug("Could not find remote host in request");
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

        public bool IsActive
        {
            get
            {
                var now = DateTime.Today.Date;
                // Considering this key Active if current date is on or after the Start Date, and less than the End Date
                return now >= StartDate.Date && (!EndDate.HasValue || now < EndDate.Value.Date);
            }
        }
    }
}