using System;
using System.Net;
using MinistryPlatform.Translation.Repositories;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using System.Collections.Generic;
using Crossroads.Web.Common.Extensions;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Translation.Extensions;

namespace MinistryPlatform.Translation.Test.Repositories
{
    public class MinistryPlatformRestServiceTest
    {
        private MinistryPlatformRestRepository _fixture;

        private Mock<IRestClient> _restClient;

        [SetUp]
        public void SetUp()
        {
            _restClient = new Mock<IRestClient>();
            _fixture = new MinistryPlatformRestRepository(_restClient.Object);
        }

        [Test]
        public void TestSearch()
        {
            var models = new List<TestModelWithRestApiTable>
            {
                new TestModelWithRestApiTable
                {
                    Id = 1,
                    Name = "name 1"
                },
                new TestModelWithRestApiTable
                {
                    Id = 2,
                    Name = "name 2"
                },
            };

            var restResponse = new Mock<IRestResponse>(MockBehavior.Strict);
            restResponse.SetupGet(mocked => mocked.ResponseStatus).Returns(ResponseStatus.Completed).Verifiable();
            restResponse.SetupGet(mocked => mocked.StatusCode).Returns(HttpStatusCode.OK).Verifiable();
            restResponse.SetupGet(mocked => mocked.Content).Returns(JsonConvert.SerializeObject(models)).Verifiable();

            _restClient.Setup(
                mocked =>
                    mocked.Execute(
                        It.Is<IRestRequest>(
                            r =>
                                r.Resource.Contains("/tables/MP_Table_Name") && r.Parameters.Find(p => p.Name.Equals("$filter")).Value.Equals("search string") &&
                                r.Parameters.Find(p => p.Name.Equals("$select")).Value.Equals("col1,col2")))).Returns(restResponse.Object);

            var results = _fixture.Search<TestModelWithRestApiTable>("search string", "col1,col2");
            _restClient.VerifyAll();
            restResponse.VerifyAll();

            Assert.IsNotNull(results);
            Assert.AreEqual(models.Count, results.Count);
            for (var i = 1; i < models.Count; i++)
            {
                Assert.AreEqual(models[i].Id, results[i].Id);
                Assert.AreEqual(models[i].Name, results[i].Name);
            }
        }

        [Test]
        public void TestSearchNotFound()
        {
            var restResponse = new Mock<IRestResponse>(MockBehavior.Strict);
            restResponse.SetupGet(mocked => mocked.ResponseStatus).Returns(ResponseStatus.Completed).Verifiable();
            restResponse.SetupGet(mocked => mocked.StatusCode).Returns(HttpStatusCode.NotFound).Verifiable();
            restResponse.SetupGet(mocked => mocked.Content).Returns("[]").Verifiable();

            _restClient.Setup(
                mocked =>
                    mocked.Execute(
                        It.Is<IRestRequest>(
                            r =>
                                r.Resource.Contains("/tables/MP_Table_Name") && r.Parameters.Find(p => p.Name.Equals("$filter")).Value.Equals("search string") &&
                                r.Parameters.Find(p => p.Name.Equals("$select")).Value.Equals("col1,col2")))).Returns(restResponse.Object);

            var results = _fixture.Search<TestModelWithRestApiTable>("search string", "col1,col2");
            _restClient.VerifyAll();
            restResponse.VerifyAll();

            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count);
        }

        [Test]
        public void TestGet()
        {
            var models = new List<TestModelWithRestApiTable>
            {
                new TestModelWithRestApiTable
                {
                    Id = 1,
                    Name = "name 1"
                }
            };

            var restResponse = new Mock<IRestResponse>(MockBehavior.Strict);
            restResponse.SetupGet(mocked => mocked.ResponseStatus).Returns(ResponseStatus.Completed).Verifiable();
            restResponse.SetupGet(mocked => mocked.StatusCode).Returns(HttpStatusCode.OK).Verifiable();
            restResponse.SetupGet(mocked => mocked.Content).Returns(JsonConvert.SerializeObject(models)).Verifiable();

            _restClient.Setup(
                mocked =>
                    mocked.Execute(
                        It.Is<IRestRequest>(r => r.Resource.Contains("/tables/MP_Table_Name/123") && r.Parameters.Find(p => p.Name.Equals("$select")).Value.Equals("col1,col2"))))
                .Returns(restResponse.Object);

            var result = _fixture.Get<TestModelWithRestApiTable>(123, "col1,col2");
            _restClient.VerifyAll();
            restResponse.VerifyAll();

            Assert.IsNotNull(result);
            Assert.AreEqual(models[0].Id, result.Id);
            Assert.AreEqual(models[0].Name, result.Name);
        }

        [Test]
        public void TestGetNotFound()
        {
            var restResponse = new Mock<IRestResponse>(MockBehavior.Strict);
            restResponse.SetupGet(mocked => mocked.ResponseStatus).Returns(ResponseStatus.Completed).Verifiable();
            restResponse.SetupGet(mocked => mocked.StatusCode).Returns(HttpStatusCode.NotFound).Verifiable();

            const string errorMessage = "error message";
            const string content = "response content";
            var ex = new Exception("Doh!!");
            restResponse.SetupGet(mocked => mocked.ErrorMessage).Returns(errorMessage).Verifiable();
            restResponse.SetupGet(mocked => mocked.Content).Returns(content).Verifiable();
            restResponse.SetupGet(mocked => mocked.ErrorException).Returns(ex).Verifiable();

            _restClient.Setup(
                mocked =>
                    mocked.Execute(
                        It.Is<IRestRequest>(r => r.Resource.Contains("/tables/MP_Table_Name/123") && r.Parameters.Find(p => p.Name.Equals("$select")).Value.Equals("col1,col2"))))
                .Returns(restResponse.Object);

            try
            {
                _fixture.Get<TestModelWithRestApiTable>(123, "col1,col2");
                Assert.Fail("Expected exception was not thrown");
            }
            catch (RestResponseException e)
            {
                Assert.AreSame(ex, e.InnerException);
                Assert.IsTrue(e.Message.Contains(errorMessage));
                Assert.IsTrue(e.Message.Contains(content));
                Assert.IsTrue(e.Message.Contains(HttpStatusCode.NotFound.ToString()));
                Assert.IsTrue(e.Message.Contains(ResponseStatus.Completed.ToString()));
            }

            _restClient.VerifyAll();
            restResponse.VerifyAll();
        }

        [Test]
        public void TestCreate()
        {
            var models = new List<TestModelWithRestApiTable>
            {
                new TestModelWithRestApiTable
                {
                    Id = 1,
                    Name = "name 2"
                }
            };

            var input = new TestModelWithRestApiTable
            {
                Id = 1,
                Name = "name 1"
            };

            var restResponse = new Mock<IRestResponse>(MockBehavior.Strict);
            restResponse.SetupGet(mocked => mocked.ResponseStatus).Returns(ResponseStatus.Completed).Verifiable();
            restResponse.SetupGet(mocked => mocked.StatusCode).Returns(HttpStatusCode.OK).Verifiable();
            restResponse.SetupGet(mocked => mocked.Content).Returns(JsonConvert.SerializeObject(models)).Verifiable();

            _restClient.Setup(
                mocked =>
                    mocked.Execute(
                        It.Is<IRestRequest>(
                            r =>
                                r.Method == Method.POST && r.Resource.Contains("/tables/MP_Table_Name") &&
                                r.Parameters.Find(p => p.Name.Equals("$select")).Value.Equals("col1,col2")))).Returns(restResponse.Object);

            var result = _fixture.Create(input, "col1,col2");
            _restClient.VerifyAll();
            restResponse.VerifyAll();

            Assert.IsNotNull(result);
            Assert.AreEqual(models[0].Id, result.Id);
            Assert.AreEqual(models[0].Name, result.Name);
        }

        [Test]
        public void TestUpdate()
        {
            var models = new List<TestModelWithRestApiTable>
            {
                new TestModelWithRestApiTable
                {
                    Id = 1,
                    Name = "name 2"
                }
            };

            var input = new TestModelWithRestApiTable
            {
                Id = 1,
                Name = "name 1"
            };

            var restResponse = new Mock<IRestResponse>(MockBehavior.Strict);
            restResponse.SetupGet(mocked => mocked.ResponseStatus).Returns(ResponseStatus.Completed).Verifiable();
            restResponse.SetupGet(mocked => mocked.StatusCode).Returns(HttpStatusCode.OK).Verifiable();
            restResponse.SetupGet(mocked => mocked.Content).Returns(JsonConvert.SerializeObject(models)).Verifiable();

            _restClient.Setup(
                mocked =>
                    mocked.Execute(
                        It.Is<IRestRequest>(
                            r =>
                                r.Method == Method.PUT && r.Resource.Contains("/tables/MP_Table_Name") && r.Parameters.Find(p => p.Name.Equals("$select")).Value.Equals("col1,col2"))))
                .Returns(restResponse.Object);

            var result = _fixture.Update(input, "col1,col2");
            _restClient.VerifyAll();
            restResponse.VerifyAll();

            Assert.IsNotNull(result);
            Assert.AreEqual(models[0].Id, result.Id);
            Assert.AreEqual(models[0].Name, result.Name);
        }

    }

    [MpRestApiTable(Name = "MP_Table_Name")]
    internal class TestModelWithRestApiTable
    {
        [JsonProperty(PropertyName = "ID")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }
    }

    internal class TestModelWithoutRestApiTable
    {
        
    }
}
