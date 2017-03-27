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
                "Participant_ID_Table_Contact_ID_Table.[Nickname]",
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

            var eventId = 1231;

            var children = new List<MpEventParticipantDto>
            {
                new MpEventParticipantDto
                {
                    EventId = 1231,
                    EventParticipantId = 1,
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
                    EventParticipantId = 2,
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

            var child1 = @"{'Event_ID': 1231, 'Event_Participant_ID': 1, 'Participant_status_ID': 1, 'First_Name': 'FirstName1', 'Last_Name': 'LastName1',
                'Nickname': 'blahblah', 'Call_Number': '1123', 'Room_ID': 1, 'Room_Name': 'Room1', 'Time_In': '1/2/2003', 'Time_Confirmed': '1/2/2003', 'Checkin_Household_ID': 1}";
            var child1JsonResult = JObject.Parse(child1);

            var child2 = @"{'Event_ID': 1231, 'Event_Participant_ID': 2, 'Participant_status_ID': 1, 'First_Name': 'FirstName2', 'Last_Name': 'LastName2',
                'Nickname': 'blahblah', 'Call_Number': '1124', 'Room_ID': 1, 'Room_Name': 'Room1', 'Time_In': '1/2/2003', 'Time_Confirmed': '1/2/2003', 'Checkin_Household_ID': 2}";
            var child2JsonResult = JObject.Parse(child2);

            var childrenResults = new List<JObject>
            {
                child1JsonResult,
                child2JsonResult
            };

            var householdOne = @"{'Event_ID': 1231, 'Event_Participant_ID': 3, 'Household_ID': 1, 'First_Name': 'FirstName3', 'Last_Name': 'LastName3', 'NickName': 't'}";
            var household1Result = JObject.Parse(householdOne);

            var householdTwo = @"{'Event_ID': 1231, 'Event_Participant_ID': 4, 'Household_ID': 1, 'First_Name': 'FirstName4', 'Last_Name': 'LastName4', 'NickName': 't'}";
            var household2Result = JObject.Parse(householdTwo);

            var householdThree = @"{'Event_ID': 1231, 'Event_Participant_ID': 5, 'Household_ID': 2, 'First_Name': 'FirstName5', 'Last_Name': 'LastName5', 'NickName': 't'}";
            var household3Result = JObject.Parse(householdThree);

            var houseHoldResults = new List<JObject>
            {
                household1Result,
                household2Result,
                household3Result
            };

            var spResult = new List<List<JObject>>
            {
                childrenResults,
                houseHoldResults
            };


            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(It.IsAny<string>())).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(m => m.GetFromStoredProc<JObject>(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>())).Returns(spResult);

            var result = _fixture.GetChildParticipantsByEvent(token, eventId);
            _ministryPlatformRestRepository.VerifyAll();

            Assert.AreEqual(result[0].EventParticipantId, children[0].EventParticipantId);
            Assert.AreEqual(result[1].EventParticipantId, children[1].EventParticipantId);
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
                $"Event_Participants.Event_ID = {eventId} AND Event_Participants.Participant_ID in ({string.Join(",", participantIds)}) AND End_Date IS NULL AND Participation_Status_ID IN (2, 3, 4)", _eventParticipantColumns, null, false)).Returns(mpEventParticipantDtos);

            // Act
            var result = _fixture.GetEventParticipantsByEventAndParticipant(eventId, participantIds);

            // Assert
            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(result[0].EventId, 1234567);
            Assert.AreEqual(result[0].ParticipantId, 4455544);
        }

        [Test]
        public void ShouldGetGroupParticipantsByParticipantAndGroupId()
        {
            // Arrange
            var token = "123abc";

            List<string> groupParticipantColumns = new List<string>
            {
                "Group_Participant_ID",
                "Group_ID",
                "Participant_ID",
                "Group_Role_ID",
                "Start_Date",
                "Employee_Role",
                "Auto_Promote"
            };

            var participantIds = new List<int>
            {
                123456,
                234567,
                345678
            };

            var groupId = 4455667;

            var mpGroupParticipantDtos = new List<MpGroupParticipantDto>
            {
                new MpGroupParticipantDto
                {
                    ParticipantId = 123456,
                    GroupId = 4455667
                },
                new MpGroupParticipantDto
                {
                    ParticipantId = 234567,
                    GroupId = 4455667
                },
                new MpGroupParticipantDto
                {
                    ParticipantId = 345678,
                    GroupId = 4455667
                }
            };

            _apiUserRepository.Setup(mocked => mocked.GetToken()).Returns(token);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpGroupParticipantDto>(
                $"Group_Participants.Participant_ID IN ({string.Join(",", participantIds)}) AND Group_Participants.Group_ID = ({groupId}) AND End_Date IS NULL", groupParticipantColumns, null, false)).Returns(mpGroupParticipantDtos);

            // Act
            var result = _fixture.GetGroupParticipantsByParticipantAndGroupId(groupId, participantIds);

            // Assert
            Assert.AreEqual(3, result.Count);
            _ministryPlatformRestRepository.VerifyAll();
        }
    }
}
