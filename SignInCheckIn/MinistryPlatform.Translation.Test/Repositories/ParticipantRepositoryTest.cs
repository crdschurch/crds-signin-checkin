using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using NUnit.Framework;

namespace MinistryPlatform.Translation.Test.Repositories
{
    public class ParticipantRepositoryTest
    {
        private ParticipantRepository _fixture;

        private Mock<IApiUserRepository> _apiUserRepository;
        private Mock<IMinistryPlatformRestRepository> _ministryPlatformRestRepository;
        private Mock<IContactRepository> _contactRepository;

        [SetUp]
        public void SetUp()
        {
            _apiUserRepository = new Mock<IApiUserRepository>(MockBehavior.Strict);
            _ministryPlatformRestRepository = new Mock<IMinistryPlatformRestRepository>();
            _contactRepository = new Mock<IContactRepository>();

            _fixture = new ParticipantRepository(_apiUserRepository.Object, _ministryPlatformRestRepository.Object, _contactRepository.Object);
        }

        [Test]
        public void ItShouldCreateNewParticipants()
        {
            // Arrange
            string token = "123abc";

            List<string> participantColumns = new List<string>
            {
                "Participants.Participant_ID",
                "Participants.Participant_Type_ID",
                "Participants.Participant_Start_Date"
            };

            var mpNewParticipantDto = new MpNewParticipantDto
            {
                FirstName = "TestFirst1",
                LastName = "TestLast1"
            };

            var returnDto = new MpNewParticipantDto();

            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(m => m.Create(mpNewParticipantDto, participantColumns)).Returns(returnDto);

            // Act
            _fixture.CreateParticipantWithContact(token, mpNewParticipantDto);

            // Assert
            _ministryPlatformRestRepository.VerifyAll();
        }

        [Test]
        public void ItShouldCreateParticipantWithContact()
        {
            // Arrange
            string token = "123abc";

            List<string> participantColumns = new List<string>
            {
                "Participants.Participant_ID",
                "Participants.Participant_Type_ID",
                "Participants.Participant_Start_Date"
            };

            MpNewParticipantDto mpNewParticipantDto = new MpNewParticipantDto
            {
                FirstName = "TestFirst1",
                LastName = "TestLast1",
                Contact = new MpContactDto
                {

                }
            };

            MpNewParticipantDto returnDto = new MpNewParticipantDto
            {
                ParticipantId = 1234567,
                FirstName = "TestFirst1",
                LastName = "TestLast1",
                Contact = new MpContactDto
                {
                    ContactId = 2345678
                }
            };

            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(m => m.Create(mpNewParticipantDto, participantColumns)).Returns(returnDto);

            // Act
            var result = _fixture.CreateParticipantWithContact(token, mpNewParticipantDto);

            // Assert
            _ministryPlatformRestRepository.VerifyAll();

            Assert.AreEqual(returnDto, result);
        }
    }
}
