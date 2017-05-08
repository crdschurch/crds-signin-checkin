using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using NUnit.Framework;
using SignInCheckIn.Services;

namespace SignInCheckIn.Tests.Services
{
    public class LookupServiceTest
    {
        private Mock<ILookupRepository> _lookupRepository;
        private LookupService _fixture;

        [SetUp]
        public void SetUp()
        {
            AutoMapperConfig.RegisterMappings();
            _lookupRepository = new Mock<ILookupRepository>();
            _fixture = new LookupService(_lookupRepository.Object);
        }
        
        [Test]
        public void ShouldGetStates()
        {
            // Arrange
            var mpStates = new List<MpStateDto>
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

            _lookupRepository.Setup(m => m.GetStates()).Returns(mpStates);

            // Act
            var result = _fixture.GetStates();

            // Assert
            _lookupRepository.VerifyAll();

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, mpStates.Count);

            Assert.AreEqual(result[0].StateId, mpStates[0].StateId);
            Assert.AreEqual(result[0].StateName, mpStates[0].StateName);
            Assert.AreEqual(result[0].StateAbbreviation, mpStates[0].StateAbbreviation);

            Assert.AreEqual(result[1].StateId, mpStates[1].StateId);
            Assert.AreEqual(result[1].StateName, mpStates[1].StateName);
            Assert.AreEqual(result[1].StateAbbreviation, mpStates[1].StateAbbreviation);
        }

        [Test]
        public void ShouldGetCountries()
        {
            // Arrange
            var mpCountries = new List<MpCountryDto>
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

            _lookupRepository.Setup(m => m.GetCountries()).Returns(mpCountries);

            // Act
            var result = _fixture.GetCountries();

            // Assert
            _lookupRepository.VerifyAll();

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, mpCountries.Count);

            Assert.AreEqual(result[0].CountryId, mpCountries[0].CountryId);
            Assert.AreEqual(result[0].Country, mpCountries[0].Country);
            Assert.AreEqual(result[0].Code3, mpCountries[0].Code3);

            Assert.AreEqual(result[1].CountryId, mpCountries[1].CountryId);
            Assert.AreEqual(result[1].Country, mpCountries[1].Country);
            Assert.AreEqual(result[1].Code3, mpCountries[1].Code3);
        }

        [Test]
        public void ShouldGetAllCongregations()
        {
            var mpCongreationDtos = new List<MpCongregationDto>
            {
                new MpCongregationDto
                {
                    CongregationName = "Dade County",
                    CongregationId = 444
                },
                new MpCongregationDto
                {
                    CongregationName = "Compton",
                    CongregationId = 555
                }
            };

            _lookupRepository.Setup(m => m.GetCongregations()).Returns(mpCongreationDtos);

            // Act
            var result = _fixture.GetCongregations();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result[0].CongregationName, mpCongreationDtos[0].CongregationName);
            Assert.AreEqual(result[1].CongregationName, mpCongreationDtos[1].CongregationName);
            _lookupRepository.VerifyAll();
        }
    }
}
