using System;
using System.Collections.Generic;
using System.Linq;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using NUnit.Framework;
using SignInCheckIn.App_Start;
using SignInCheckIn.Services;

namespace SignInCheckIn.Tests.Services
{
    public class ChildCheckinServiceTest
    {
        private Mock<IChildCheckinRepository> _childCheckinRepository;

        private ChildCheckinService _fixture;

        [SetUp]
        public void SetUp()
        {
            AutoMapperConfig.RegisterMappings();

            _childCheckinRepository = new Mock<IChildCheckinRepository>();
            _fixture = new ChildCheckinService(_childCheckinRepository.Object);
        }

        [Test]
        public void ShouldGetChildrenByPhoneNumber()
        {
            var phoneNumber = "812-812-8877";
            var primaryHouseholdId = 123;

            List<MpParticipantDto> mpParticipantDto = new List<MpParticipantDto>
            {
                new MpParticipantDto
                {
                    ParticipantId = 12,
                    ContactId = 1443,
                    HouseholdId = primaryHouseholdId,
                    HouseholdPositionId = 2,
                    FirstName = "First1",
                    LastName = "Last1",
                    DateOfBirth = new DateTime()
                }
            };

            _childCheckinRepository.Setup(m => m.GetChildrenByPhoneNumber(phoneNumber)).Returns(mpParticipantDto);
            
            var result = _fixture.GetChildrenByPhoneNumber(phoneNumber);
            _childCheckinRepository.VerifyAll();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(mpParticipantDto[0].ParticipantId, result[0].ParticipantId);
            Assert.AreEqual(mpParticipantDto[0].ContactId, result[0].ContactId);
        }

        [Test]
        public void ShouldNotGetChildrenByPhoneNumber()
        {
            var phoneNumber = "812-812-8877";

            List<MpParticipantDto> mpParticipantDto = new List<MpParticipantDto>();

            _childCheckinRepository.Setup(m => m.GetChildrenByPhoneNumber(phoneNumber)).Returns(mpParticipantDto);

            var result = _fixture.GetChildrenByPhoneNumber(phoneNumber);
            _childCheckinRepository.VerifyAll();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.Any());
        }
    }
}
