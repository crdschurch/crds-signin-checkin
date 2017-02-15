using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using NUnit.Framework;
using SignInCheckIn.Filters;
using System.Web.Cors;
using System.Web.Http;
using System.Web.Http.Controllers;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;

namespace SignInCheckIn.Tests.Filters
{
    public class DomainLockedApiKeyFilterTest
    {
        private DomainLockedApiKeyFilter _fixture;
        private Mock<IApiUserRepository> _apiUserRepository;
        private Mock<ICorsEngine> _corsEngine;
        private Mock<IMinistryPlatformRestRepository> _ministryPlatformRestRepository;
        private HttpActionContext _httpActionContext;

        private const string MpApiUserToken = "api token";

        private List<DomainLockedApiKey> _registeredApiKeys;
        private Guid _activeGuid = Guid.NewGuid();
        private Guid _inactiveGuid = Guid.NewGuid();
        private Guid _notFoundGuid = Guid.NewGuid();

        [SetUp]
        public void SetUp()
        {
            _httpActionContext = new HttpActionContext(new HttpControllerContext {Request = new HttpRequestMessage()
            {
                RequestUri = new Uri("http://www.crossroads.net/test")
            } }, new ReflectedHttpActionDescriptor());
            _httpActionContext.Request.Headers.Add("Origin", "http://localhost1/test");
            _httpActionContext.Request.Headers.Host = "www.crossroads.net";

            _ministryPlatformRestRepository = new Mock<IMinistryPlatformRestRepository>(MockBehavior.Strict);
            _corsEngine = new Mock<ICorsEngine>(MockBehavior.Strict);
            _apiUserRepository = new Mock<IApiUserRepository>(MockBehavior.Strict);

            _registeredApiKeys = new List<DomainLockedApiKey>
            {
                new DomainLockedApiKey
                {
                    AllowedDomains = "localhost1,postman:1",
                    Key = _activeGuid,
                    StartDate = DateTime.Today.AddDays(-7),
                    EndDate = DateTime.Today.AddDays(2)
                },
                new DomainLockedApiKey
                {
                    AllowedDomains = "localhost2,postman:2",
                    Key = _inactiveGuid,
                    StartDate = DateTime.Today.AddDays(-7),
                    EndDate = DateTime.Today.AddDays(-2)
                }
            };
        }

        private void CreateFixture()
        {
            _apiUserRepository.Setup(mocked => mocked.GetToken()).Returns(MpApiUserToken);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(MpApiUserToken)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<DomainLockedApiKey>(null, (string) null, null, false)).Returns(_registeredApiKeys);
            _fixture = new DomainLockedApiKeyFilter(_ministryPlatformRestRepository.Object, _corsEngine.Object, _apiUserRepository.Object);
        }

        [Test]
        public void TestConstruct()
        {
            CreateFixture();
            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();
            Assert.AreEqual(_registeredApiKeys, _fixture.RegisteredKeys);
        }

        [Test]
        public void TestReloadKeys()
        {
            CreateFixture();
            _apiUserRepository.Reset();
            _ministryPlatformRestRepository.Reset();

            _apiUserRepository.Setup(mocked => mocked.GetToken()).Returns(MpApiUserToken);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(MpApiUserToken)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<DomainLockedApiKey>(null, (string)null, null, false)).Returns(new List<DomainLockedApiKey>());

            _fixture.ReloadKeys();
            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();
            Assert.IsNotNull(_fixture.RegisteredKeys);
            Assert.IsFalse(_fixture.RegisteredKeys.Any());
        }

        [Test]
        public void TestOnActionExecutingNoApiKeyHeader()
        {
            CreateFixture();
            _fixture.OnActionExecuting(_httpActionContext);
            _corsEngine.VerifyAll();
        }

        [Test]
        public void TestOnActionExecutingApiKeyHeaderNoRemoteHost()
        {
            CreateFixture();
            _httpActionContext.Request.Headers.Remove("Origin");
            _httpActionContext.Request.Headers.Add(DomainLockedApiKeyFilter.ApiKeyHeader, _activeGuid.ToString());

            try
            {
                _fixture.OnActionExecuting(_httpActionContext);
                Assert.Fail("Expected exception not thrown");
            }
            catch (HttpResponseException e)
            {
                _corsEngine.VerifyAll();
                Assert.AreEqual(HttpStatusCode.BadRequest, e.Response.StatusCode);
            }
        }

        [Test]
        public void TestOnActionExecutingApiKeyHeaderKeyNotFound()
        {
            CreateFixture();
            _httpActionContext.Request.Headers.Add(DomainLockedApiKeyFilter.ApiKeyHeader, _notFoundGuid.ToString());

            try
            {
                _fixture.OnActionExecuting(_httpActionContext);
                Assert.Fail("Expected exception not thrown");
            }
            catch (HttpResponseException e)
            {
                _corsEngine.VerifyAll();
                Assert.AreEqual(HttpStatusCode.Forbidden, e.Response.StatusCode);
            }
        }

        [Test]
        public void TestOnActionExecutingApiKeyNoOriginsOnKey()
        {
            CreateFixture();
            _registeredApiKeys.Find(k => k.Key.Equals(_activeGuid)).AllowedDomains = null;
            _httpActionContext.Request.Headers.Add(DomainLockedApiKeyFilter.ApiKeyHeader, _activeGuid.ToString());

            _fixture.OnActionExecuting(_httpActionContext);
            _corsEngine.VerifyAll();
        }

        [Test]
        public void TestOnActionExecutingApiKeyHeaderInvalidOrigin()
        {
            var corsResult = new CorsResult();
            corsResult.ErrorMessages.Add("bad bad bad");

            CreateFixture();
            _httpActionContext.Request.Headers.Remove("Origin");
            _httpActionContext.Request.Headers.Referrer = new Uri("http://localhost3/test");
            _httpActionContext.Request.Headers.Add(DomainLockedApiKeyFilter.ApiKeyHeader, _activeGuid.ToString());
            _corsEngine.Setup(
                mocked =>
                    mocked.EvaluatePolicy(
                        It.Is<CorsRequestContext>(
                            ctx => ctx.Host.Equals("www.crossroads.net") && ctx.Origin.Equals("localhost3") && ctx.RequestUri.Equals(_httpActionContext.Request.RequestUri)),
                        It.Is<CorsPolicy>(pol => pol.AllowAnyHeader && pol.AllowAnyMethod && pol.Origins.Contains("localhost1") && pol.Origins.Contains("postman:1"))))
                .Returns(corsResult);

            try
            {
                _fixture.OnActionExecuting(_httpActionContext);
                Assert.Fail("Expected exception not thrown");
            }
            catch (HttpResponseException e)
            {
                _corsEngine.VerifyAll();
                Assert.AreEqual(HttpStatusCode.Forbidden, e.Response.StatusCode);
            }
        }

        [Test]
        public void TestOnActionExecutingApiKeyHeaderExpiredKey()
        {
            CreateFixture();
            _httpActionContext.Request.Headers.Remove("Origin");
            _httpActionContext.Request.Headers.Add("Origin", "http://localhost2/test");
            _httpActionContext.Request.Headers.Add(DomainLockedApiKeyFilter.ApiKeyHeader, _inactiveGuid.ToString());

            try
            {
                _fixture.OnActionExecuting(_httpActionContext);
                Assert.Fail("Expected exception not thrown");
            }
            catch (HttpResponseException e)
            {
                _corsEngine.VerifyAll();
                Assert.AreEqual(HttpStatusCode.Forbidden, e.Response.StatusCode);
            }
        }

        [Test]
        public void TestOnActionExecuting()
        {
            var corsResult = new CorsResult();

            CreateFixture();
            _httpActionContext.Request.Headers.Remove("Origin");
            _httpActionContext.Request.Headers.Add("Origin", "http://localhost1/test");
            _httpActionContext.Request.Headers.Add(DomainLockedApiKeyFilter.ApiKeyHeader, _activeGuid.ToString());
            _corsEngine.Setup(
                mocked =>
                    mocked.EvaluatePolicy(
                        It.Is<CorsRequestContext>(
                            ctx => ctx.Host.Equals("www.crossroads.net") && ctx.Origin.Equals("localhost1") && ctx.RequestUri.Equals(_httpActionContext.Request.RequestUri)),
                        It.Is<CorsPolicy>(pol => pol.AllowAnyHeader && pol.AllowAnyMethod && pol.Origins.Contains("localhost1") && pol.Origins.Contains("postman:1"))))
                .Returns(corsResult);

            _fixture.OnActionExecuting(_httpActionContext);
            _corsEngine.VerifyAll();
        }
    }
}