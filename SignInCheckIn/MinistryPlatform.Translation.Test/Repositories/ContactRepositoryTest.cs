using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

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

            _apiUserRepository.Setup(mocked => mocked.GetDefaultApiClientToken()).Returns(token);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Create<MpContactRelationshipDto>(contactRelationshipDtos, columnList));

            // Act
            _fixture.CreateContactRelationships(contactRelationshipDtos);

            // Assert
            _ministryPlatformRestRepository.VerifyAll();
        }

        [Test]
        public void ShouldCreateUserRecord()
        {
            // Arrange
            var token = "123abc";

            var columnList = new List<string>
            {
                "User_ID",
                "User_Email",
                "Password",
                "Display_Name",
                "Domain_ID",
                "User_Name",
                "Contact_ID",
                "PasswordResetToken"
            };

            var mpUserDto = new MpUserDto
            {
                FirstName = "test_first", // contact?
                LastName = "test_last", // contact?
                UserEmail = "test@test.com",
                Password = "test_password",
                Company = false, //contact?
                DisplayName = "test_first",
                DomainId = 1,
                UserName = "test@test.com",
                ContactId = 1234567,
                PasswordResetToken = "abcdefgh12345678"
            };

            _apiUserRepository.Setup(mocked => mocked.GetDefaultApiClientToken()).Returns(token);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Create<MpUserDto>(mpUserDto, columnList));

            // Act
            _fixture.CreateUserRecord(mpUserDto);

            // Assert
            _ministryPlatformRestRepository.VerifyAll();
        }
    }
}