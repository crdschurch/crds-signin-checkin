using System.Collections.Generic;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories;
using Moq;
using NUnit.Framework;

namespace MinistryPlatform.Translation.Test.Repositories
{
    public class LookupRepositoryTest
    {
        private LookupRepository _fixture;

        private Mock<IApiUserRepository> _apiUserRepository;
        private Mock<IMinistryPlatformRestRepository> _ministryPlatformRestRepository;

        [SetUp]
        public void SetUp()
        {
            _apiUserRepository = new Mock<IApiUserRepository>(MockBehavior.Strict);
            _ministryPlatformRestRepository = new Mock<IMinistryPlatformRestRepository>(MockBehavior.Strict);

            _fixture = new LookupRepository(_apiUserRepository.Object, _ministryPlatformRestRepository.Object);
        }

        [Test]
        public void TestGetStates()
        {
            const string token = "tok 123";

            var columns = new List<string>
            {
                "States.[State_ID]",
                "States.[State_Name]",
                "States.[State_Abbreviation]",
            };

            var states = new List<MpStateDto>
            {
                new MpStateDto
                {
                    StateId = 12,
                    StateName = "Kentucky",
                    StateAbbreviation = "KY"
                },
                new MpStateDto
                {
                    StateId = 13,
                    StateName = "Ohio",
                    StateAbbreviation = "OH"
                }
            };

            _apiUserRepository.Setup(mocked => mocked.GetApiClientToken("CRDS.Service.SignCheckIn")).Returns(token);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpStateDto>("1=1", columns, null, false)).Returns(states);

            var result = _fixture.GetStates();
            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();
            
            Assert.AreEqual(states.Count, result.Count);
        }

        [Test]
        public void TestGetCountries()
        {
            const string token = "tok 123";

            var columns = new List<string>
            {
                "Countries.[Country_ID]",
                "Countries.[Country]",
                "Countries.[Code3]",
            };

            var countries = new List<MpCountryDto>
            {
                new MpCountryDto
                {
                    CountryId = 12,
                    Country = "United States of America",
                    Code3 = "USA"
                },
                new MpCountryDto
                {
                    CountryId = 13,
                    Country = "China",
                    Code3 = "CHN"
                }
            };

            _apiUserRepository.Setup(mocked => mocked.GetApiClientToken("CRDS.Service.SignCheckIn")).Returns(token);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpCountryDto>("1=1", columns, null, false)).Returns(countries);

            var result = _fixture.GetCountries();
            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();

            Assert.AreEqual(countries.Count, result.Count);
        }
    }
}
