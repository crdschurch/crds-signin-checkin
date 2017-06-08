using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace MinistryPlatform.Translation.Test.Repositories
{
    public class ContactRepositoryTest
    {
        private ContactRepository _fixture;

        private Mock<IApiUserRepository> _apiUserRepository;
        private Mock<IMinistryPlatformRestRepository> _ministryPlatformRestRepository;

        [SetUp]
        public void SetUp()
        {
            _apiUserRepository = new Mock<IApiUserRepository>(MockBehavior.Strict);
            _ministryPlatformRestRepository = new Mock<IMinistryPlatformRestRepository>();

            _fixture = new ContactRepository(_apiUserRepository.Object, _ministryPlatformRestRepository.Object);
        }

        [Test]
        public void ShouldSaveContactRelationships()
        {
            // Arrange
            DateTime contactStartTime = new DateTime(2017, 1, 1, 0, 0, 0);
            string token = "123abc";

            List<MpContactRelationshipDto> contactRelationshipDtos = new List<MpContactRelationshipDto>
            {
                new MpContactRelationshipDto
                {
                    ContactId = 1234567,
                    RelatedContactId = 7654321,
                    RelationshipId = 1,
                    StartDate = contactStartTime
                }
            };

            List<string> columnList = new List<string>
            {
                "Contact_ID",
                "Related_Contact_ID",
                "Relationship_ID",
                "Start_Date"
            };

            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Create<MpContactRelationshipDto>(contactRelationshipDtos, columnList));

            // Act
            _fixture.CreateContactRelationships(token, contactRelationshipDtos);

            // Assert
            _ministryPlatformRestRepository.VerifyAll();
        }
    }
}
