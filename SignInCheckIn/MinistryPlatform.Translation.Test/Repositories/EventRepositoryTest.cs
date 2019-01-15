using Crossroads.Web.Common.MinistryPlatform;
using FluentAssertions;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace MinistryPlatform.Translation.Test.Repositories
{
    public class EventRepositoryTest
    {
        private EventRepository _fixture;

        private Mock<IApiUserRepository> _apiUserRepository;
        private Mock<IMinistryPlatformRestRepository> _ministryPlatformRestRepository;
        private List<string> _eventGroupsColumns;

        [SetUp]
        public void SetUp()
        {
            _apiUserRepository = new Mock<IApiUserRepository>(MockBehavior.Strict);
            _ministryPlatformRestRepository = new Mock<IMinistryPlatformRestRepository>(MockBehavior.Strict);

            _eventGroupsColumns = new List<string>
            {
                "Event_Groups.[Event_Group_ID]",
                "Event_ID_Table.[Event_ID]",
                "Group_ID_Table.[Group_ID]",
                "Event_Room_ID_Table.[Event_Room_ID]",
                "Event_Room_ID_Table_Room_ID_Table.[Room_ID]",
                "Event_Room_ID_Table.[Capacity]",
                "Event_Room_ID_Table.[Label]",
                "Event_Room_ID_Table.[Allow_Checkin]",
                "Event_Room_ID_Table.[Volunteers]",
                "[dbo].crds_getEventParticipantStatusCount(Event_ID_Table.[Event_ID], Event_Room_ID_Table_Room_ID_Table.[Room_ID], 3) AS Signed_In",
                "[dbo].crds_getEventParticipantStatusCount(Event_ID_Table.[Event_ID], Event_Room_ID_Table_Room_ID_Table.[Room_ID], 4) AS Checked_In"
            };

            _fixture = new EventRepository(_apiUserRepository.Object, _ministryPlatformRestRepository.Object);
        }

        [Test]
        public void TestGetEventGroupsForEvent()
        {
            const int eventId = 123;
            const string token = "tok 123";
            var events = new List<MpEventGroupDto>();

            _apiUserRepository.Setup(mocked => mocked.GetDefaultApiClientToken()).Returns(token);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpEventGroupDto>($"Event_Groups.Event_ID IN ({eventId})", _eventGroupsColumns, null, false)).Returns(events);
            var result = _fixture.GetEventGroupsForEvent(eventId);
            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();
            result.Should().BeSameAs(events);
        }

        [Test]
        public void TestImportEventSetup()
        {
            const string token = "tok123";
            const int sourceEventId = 12345;
            const int destinationEventId = 67890;

            _apiUserRepository.Setup(m => m.GetDefaultApiClientToken()).Returns(token);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.PostStoredProc("api_crds_ImportEcheckEvent", It.IsAny<Dictionary<string, object>>())).Returns(1);

            _fixture.ImportEventSetup(destinationEventId, sourceEventId);

            _ministryPlatformRestRepository.VerifyAll();
            _ministryPlatformRestRepository.Verify(
                mocked =>
                    mocked.PostStoredProc("api_crds_ImportEcheckEvent",
                                          It.Is<Dictionary<string, object>>(d => (int)d["@SourceEventId"] == sourceEventId && (int)d["@DestinationEventId"] == destinationEventId)));
        }

        [Test]
        public void TestResetEventSetup()
        {
            const string token = "tok123";
            const int eventId = 12345;

            _apiUserRepository.Setup(m => m.GetDefaultApiClientToken()).Returns(token);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.PostStoredProc("api_crds_ResetEcheckEvent", It.IsAny<Dictionary<string, object>>())).Returns(1);

            _fixture.ResetEventSetup(eventId);

            _ministryPlatformRestRepository.VerifyAll();
            _ministryPlatformRestRepository.Verify(
                mocked =>
                    mocked.PostStoredProc("api_crds_ResetEcheckEvent", It.Is<Dictionary<string, object>>(d => (int)d["@EventId"] == eventId)));
        }

        [Test]
        public void ItShouldGetEventAndSubevents()
        {
            // Arrange
            var token = "123abc";
            var eventId = 1234567;

            List<MpEventDto> mpEventDtos = new List<MpEventDto>();

            _apiUserRepository.Setup(m => m.GetDefaultApiClientToken()).Returns(token);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpEventDto>(It.IsAny<string>(), It.IsAny<List<string>>(), null, false)).Returns(mpEventDtos);

            // Act
            _fixture.GetEventAndCheckinSubevents(eventId, false);

            // Assert
            _ministryPlatformRestRepository.VerifyAll();
        }

        [Test]
        public void ShouldGetEventGroupsByGroupTypeId()
        {
            // Arrange
            var token = "123ABC";
            var eventId = 1234567;
            var groupTypeId = 27;

            var mpEventGroups = new List<MpEventGroupDto>
            {
                new MpEventGroupDto
                {
                    EventId = eventId,
                    GroupId = 2233445
                }
            };

            _apiUserRepository.Setup(mocked => mocked.GetDefaultApiClientToken()).Returns(token);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpEventGroupDto>(
                $"Event_Groups.Event_ID = {eventId} AND Group_ID_Table.[Group_Type_ID] = {groupTypeId}", _eventGroupsColumns, null, false)).Returns(mpEventGroups);

            // Act
            var result = _fixture.GetEventGroupsForEventByGroupTypeId(eventId, groupTypeId);

            // Assert
            Assert.AreEqual(1, result.Count);
            _ministryPlatformRestRepository.VerifyAll();
        }
    }
}
