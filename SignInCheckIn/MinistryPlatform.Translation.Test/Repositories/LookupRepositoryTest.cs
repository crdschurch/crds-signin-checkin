using System.Collections.Generic;
using System.Linq;
using Crossroads.Web.Common.MinistryPlatform;
using FluentAssertions;
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

            _apiUserRepository.Setup(mocked => mocked.GetToken()).Returns(token);
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

            _apiUserRepository.Setup(mocked => mocked.GetToken()).Returns(token);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpCountryDto>("1=1", columns, null, false)).Returns(countries);

            var result = _fixture.GetCountries();
            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();

            Assert.AreEqual(countries.Count, result.Count);
        }

        [Test]
        public void TestGetAllCongregations()
        {
            const string token = "tok123";

            var congregationColumnList = new List<string>
            {
                "Congregation_ID",
                "Congregation_Name"
            };

            var mpCongregationDtos = new List<MpCongregationDto>();
            mpCongregationDtos.Add(
                new MpCongregationDto
                {
                    CongregationId = 16,
                    CongregationName = "Oxford"
                }
            );
            mpCongregationDtos.Add(
                new MpCongregationDto
                {
                    CongregationId = 18,
                    CongregationName = "Georgetown"
                }
            );

            _apiUserRepository.Setup(mocked => mocked.GetToken()).Returns(token);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpCongregationDto>($"Available_Online = 1 AND (End_Date IS NULL OR End_Date > '{System.DateTime.Now:yyyy-MM-dd}')", congregationColumnList, null, false)).Returns(mpCongregationDtos);

            var result = _fixture.GetCongregations();

            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();
            result.First().Should().BeSameAs(mpCongregationDtos.First());
            result[1].Should().BeSameAs(mpCongregationDtos[1]);
        }

        [Test]
        public void TestGetAllLocations()
        {
            const string token = "tok123";

            var locationColumnList = new List<string>
            {
                "Location_ID",
                "Location_Name"
            };

            var mpLocationDtos = new List<MpLocationDto>();
            mpLocationDtos.Add(
                new MpLocationDto
                {
                    LocationId = 16,
                    LocationName = "Oxford"
                }
            );
            mpLocationDtos.Add(
                new MpLocationDto
                {
                    LocationId = 18,
                    LocationName = "Georgetown"
                }
            );

            _apiUserRepository.Setup(mocked => mocked.GetToken()).Returns(token);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpLocationDto>($"Move_Out_Date IS NULL OR Move_Out_Date > '{System.DateTime.Now:yyyy-MM-dd}'", locationColumnList, null, false)).Returns(mpLocationDtos);

            var result = _fixture.GetLocations();

            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();
            result.First().Should().BeSameAs(mpLocationDtos.First());
            result[1].Should().BeSameAs(mpLocationDtos[1]);
        }
    }
}
