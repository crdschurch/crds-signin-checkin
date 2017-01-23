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
        private List<string> _eventParticipantColumns;

        [SetUp]
        public void SetUp()
        {
            _apiUserRepository = new Mock<IApiUserRepository>(MockBehavior.Strict);
            _ministryPlatformRestRepository = new Mock<IMinistryPlatformRestRepository>();
            _contactRepository = new Mock<IContactRepository>();

            _eventParticipantColumns = new List<string>
            {
                "Event_Participant_ID",
                "Event_ID",
                "Participant_ID_Table_Contact_ID_Table.[First_Name]",
                "Participant_ID_Table_Contact_ID_Table.[Last_Name]",
                "Participant_ID_Table.[Participant_ID]",
                "Participation_Status_ID",
                "Time_In",
                "Time_Confirmed",
                "Time_Out",
                "Event_Participants.[Notes]",
                "Group_Participant_ID",
                "[Check-in_Station]",
                "Group_ID",
                "Room_ID_Table.[Room_ID]",
                "Room_ID_Table.[Room_Name]",
                "Call_Parents",
                "Group_Role_ID",
                "Response_ID",
                "Opportunity_ID",
                "Registrant_Message_Sent",
                "Call_Number",
                "Checkin_Phone",
                "Checkin_Household_ID"
            };

            _fixture = new ParticipantRepository(_apiUserRepository.Object, _ministryPlatformRestRepository.Object, _contactRepository.Object);
        }

        [Test]
        public void ItShouldGetChildParticipantsForEvent()
        {
            var token = "123abc";

            var eventIds = new List<int>
            {
                1231
            };

            var columns = new List<string>
            {
                "Event_ID_Table.Event_ID",
                "Event_Participant_ID",
                "Participation_Status_ID_Table.Participation_Status_ID",
                "Participant_ID_Table_Contact_ID_Table.First_Name",
                "Participant_ID_Table_Contact_ID_Table.Last_Name",
                "Event_Participants.Call_Number",
                "Room_ID_Table.Room_ID",
                "Room_ID_Table.Room_Name",
                "Time_In",
                "Event_Participants.Checkin_Household_ID",
                "Participant_ID_Table_Contact_ID_Table_Household_ID_Table.Household_ID"
            };

            var children = new List<MpEventParticipantDto>
            {
                new MpEventParticipantDto
                {
                    EventId = 1231,
                    ParticipantId = 1,
                    ParticipantStatusId = 1,
                    FirstName = "FirstName1",
                    LastName = "LastName1",
                    CallNumber = "1123",
                    RoomId = 1,
                    RoomName = "Room1",
                    TimeIn = DateTime.Now,
                    CheckinHouseholdId = 1
                },
                new MpEventParticipantDto
                {
                    EventId = 1231,
                    ParticipantId = 2,
                    ParticipantStatusId = 1,
                    FirstName = "FirstName2",
                    LastName = "LastName2",
                    CallNumber = "1124",
                    RoomId = 1,
                    RoomName = "Room1",
                    TimeIn = DateTime.Now,
                    CheckinHouseholdId = 2
                }
            };

            var household1 = new List<MpContactDto>
            {
                new MpContactDto
                {
                    HouseholdId = 1,
                    FirstName = "FirstName3",
                    LastName = "LastName3"
                },
                new MpContactDto
                {
                    HouseholdId = 1,
                    FirstName = "FirstName4",
                    LastName = "LastName4"
                }
            };

            var household2 = new List<MpContactDto>
            {
                new MpContactDto
                {
                    HouseholdId = 2,
                    FirstName = "FirstName5",
                    LastName = "LastName5"
                }
            };

            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(m => m.Search<MpEventParticipantDto>($"Event_ID_Table.Event_ID in ({string.Join(",", eventIds)}) AND End_Date IS NULL", columns)).Returns(children);
            _contactRepository.Setup(m => m.GetHeadsOfHouseholdByHouseholdId(1)).Returns(household1);
            _contactRepository.Setup(m => m.GetHeadsOfHouseholdByHouseholdId(2)).Returns(household2);

            var result = _fixture.GetChildParticipantsByEvent(token, eventIds);
            _ministryPlatformRestRepository.VerifyAll();

            Assert.AreEqual(result[0].ParticipantId, children[0].ParticipantId);
            Assert.AreEqual(result[1].ParticipantId, children[1].ParticipantId);
            Assert.AreEqual(result[0].HeadsOfHousehold.Count, household1.Count);
            Assert.AreEqual(result[0].HeadsOfHousehold[0].FirstName, household1[0].FirstName);
            Assert.AreEqual(result[0].HeadsOfHousehold[1].FirstName, household1[1].FirstName);
            Assert.AreEqual(result[1].HeadsOfHousehold.Count, household2.Count);
            Assert.AreEqual(result[1].HeadsOfHousehold[0].FirstName, household2[0].FirstName);
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

        [Test]
        public void ItShouldGetParticipantsByEventAndParticipantId()
        {
            // Arrange
            var token = "123abc";

            var eventId = 1234567;

            List<int> participantIds = new List<int>
            {
                4455544
            };

            List<MpEventParticipantDto> mpEventParticipantDtos = new List<MpEventParticipantDto>
            {
                new MpEventParticipantDto
                {
                    EventId = 1234567,
                    ParticipantId = 4455544
                }
            };

            _apiUserRepository.Setup(mocked => mocked.GetToken()).Returns(token);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpEventParticipantDto>(
                $"Event_Participants.Event_ID = {eventId} AND Event_Participants.Participant_ID in ({string.Join(",", participantIds)}) AND End_Date IS NULL", _eventParticipantColumns)).Returns(mpEventParticipantDtos);

            // Act
            var result = _fixture.GetEventParticipantsByEventAndParticipant(eventId, participantIds);

            // Assert
            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(result[0].EventId, 1234567);
            Assert.AreEqual(result[0].ParticipantId, 4455544);
        }
    }
}
