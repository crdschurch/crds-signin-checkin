using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using NUnit.Framework;

namespace MinistryPlatform.Translation.Test.Repositories
{
    public class ChildCheckinRepositoryTest
    {
        private ChildCheckinRepository _fixture;
        private Mock<IApiUserRepository> _apiUserRepository;
        private Mock<IMinistryPlatformRestRepository> _ministryPlatformRestRepository;

        private List<string> _getEventParticipantColumns;

        [SetUp]
        public void SetUp()
        {
            _apiUserRepository = new Mock<IApiUserRepository>(MockBehavior.Strict);
            _ministryPlatformRestRepository = new Mock<IMinistryPlatformRestRepository>(MockBehavior.Strict);
            _fixture = new ChildCheckinRepository(_apiUserRepository.Object, _ministryPlatformRestRepository.Object);

            _getEventParticipantColumns = new List<string>
            {
                "Event_ID_Table.Event_ID",
                "Room_ID_Table.Room_ID",
                "Event_Participants.Event_Participant_ID",
                "Participant_ID_Table.Participant_ID",
                "Participant_ID_Table_Contact_ID_Table.Contact_ID",
                "Participant_ID_Table_Contact_ID_Table.First_Name",
                "Participant_ID_Table_Contact_ID_Table.Last_Name",
                "Participation_Status_ID_Table.Participation_Status_ID"
            };
        }

        [Test]
        public void TestGetChildrenByEventAndRoom()
        {
            var eventId = 987;
            var roomId = 123;

            var participants = new List<MpParticipantDto>
            {
                new MpParticipantDto
                {
                    EventParticipantId = 1,
                    ParticipantId = 12,
                    ContactId = 1443,
                    FirstName = "First1",
                    LastName = "Last1",
                    ParticipationStatusId = 3
                }
            };

            _apiUserRepository.Setup(mocked => mocked.GetToken()).Returns("auth");
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken("auth")).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.SearchTable<MpParticipantDto>("Event_Participants", $"Event_ID_Table.[Event_ID] = {eventId} AND Room_ID_Table.[Room_ID] = {roomId}", _getEventParticipantColumns)).Returns(participants);
            
            var result = _fixture.GetChildrenByEventAndRoom(eventId, roomId);
            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();
            
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(result[0].EventParticipantId, participants[0].EventParticipantId);
            Assert.AreEqual(result[0].ParticipantId, participants[0].ParticipantId);
            Assert.AreEqual(result[0].ContactId, participants[0].ContactId);
            Assert.AreEqual(result[0].FirstName, participants[0].FirstName);
            Assert.AreEqual(result[0].LastName, participants[0].LastName);
            Assert.AreEqual(result[0].ParticipationStatusId, participants[0].ParticipationStatusId);
        }

        [Test]
        public void TestCheckinChildrenForCurrentEventAndRoom()
        {
            _apiUserRepository.Setup(mocked => mocked.GetToken()).Returns("auth");
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken("auth")).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UpdateRecord("Event_Participants", It.IsAny<int>(), It.IsAny<Dictionary<string, object>>()));

            _fixture.CheckinChildrenForCurrentEventAndRoom(3, 123);
            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();
        }
    }
}