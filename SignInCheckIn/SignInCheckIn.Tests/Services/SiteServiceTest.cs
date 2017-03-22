using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using NUnit.Framework;
using SignInCheckIn.Services;

namespace SignInCheckIn.Tests.Services
{
    public class SiteServiceTest
    {
        private Mock<ISiteRepository> _siteRepository;

        private SiteService _fixture;

        [SetUp]
        public void SetUp()
        {
            AutoMapperConfig.RegisterMappings();

            _siteRepository = new Mock<ISiteRepository>();

            _fixture = new SiteService(_siteRepository.Object);
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

            _siteRepository.Setup(m => m.GetAll()).Returns(mpCongreationDtos);

            // Act
            var result = _fixture.GetAll();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result[0].CongregationName, mpCongreationDtos[0].CongregationName);
            Assert.AreEqual(result[1].CongregationName, mpCongreationDtos[1].CongregationName);
            _siteRepository.VerifyAll();
        }
    }
}
