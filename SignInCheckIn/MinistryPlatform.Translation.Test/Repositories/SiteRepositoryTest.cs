using System;
using System.Collections.Generic;
using System.Linq;
using Crossroads.Web.Common.MinistryPlatform;
using FluentAssertions;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using NUnit.Framework;

namespace MinistryPlatform.Translation.Test.Repositories
{
    public class SiteRepositoryTest
    {
        private SiteRepository _fixture;

        private Mock<IApiUserRepository> _apiUserRepository;
        private Mock<IMinistryPlatformRestRepository> _ministryPlatformRestRepository;
        private List<string> _congregationColumnList;

        [SetUp]
        public void SetUp()
        {
            _apiUserRepository = new Mock<IApiUserRepository>(MockBehavior.Strict);
            _ministryPlatformRestRepository = new Mock<IMinistryPlatformRestRepository>(MockBehavior.Strict);

            _congregationColumnList = new List<string>
            {
                "Congregation_ID",
                "Congregation_Name"
            };

            _fixture = new SiteRepository(_apiUserRepository.Object, _ministryPlatformRestRepository.Object);
        }

        [Test]
        public void TestGetAllCongregations()
        {
            const string token = "tok123";

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

            _apiUserRepository.Setup(mocked => mocked.GetDefaultApiUserToken()).Returns(token);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpCongregationDto>($"Available_Online = 1 AND (End_Date IS NULL OR End_Date > '{DateTime.Now:yyyy-MM-dd}')", _congregationColumnList, null, false)).Returns(mpCongregationDtos);

            var result = _fixture.GetAll();

            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();
            result.First().Should().BeSameAs(mpCongregationDtos.First());
            result[1].Should().BeSameAs(mpCongregationDtos[1]);
        }    
    }
}
