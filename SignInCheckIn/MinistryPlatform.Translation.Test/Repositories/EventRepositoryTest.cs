﻿using System.Collections.Generic;
using Castle.Components.DictionaryAdapter;
using FluentAssertions;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using NUnit.Framework;

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

            _apiUserRepository.Setup(mocked => mocked.GetToken()).Returns(token);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpEventGroupDto>($"Event_Groups.Event_ID IN ({eventId})", _eventGroupsColumns)).Returns(events);
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

            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.PostStoredProc("api_crds_ImportEcheckEvent", It.IsAny<Dictionary<string, object>>())).Returns(1);

            _fixture.ImportEventSetup(token, destinationEventId, sourceEventId);

            _ministryPlatformRestRepository.VerifyAll();
            _ministryPlatformRestRepository.Verify(
                mocked =>
                    mocked.PostStoredProc("api_crds_ImportEcheckEvent",
                                          It.Is<Dictionary<string, object>>(d => (int) d["@SourceEventId"] == sourceEventId && (int) d["@DestinationEventId"] == destinationEventId)));
        }

        [Test]
        public void TestResetEventSetup()
        {
            const string token = "tok123";
            const int eventId = 12345;

            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.PostStoredProc("api_crds_ResetEcheckEvent", It.IsAny<Dictionary<string, object>>())).Returns(1);

            _fixture.ResetEventSetup(token, eventId);

            _ministryPlatformRestRepository.VerifyAll();
            _ministryPlatformRestRepository.Verify(
                mocked =>
                    mocked.PostStoredProc("api_crds_ResetEcheckEvent", It.Is<Dictionary<string, object>>(d => (int) d["@EventId"] == eventId)));
        }

        [Test]
        public void ItShouldGetEventAndSubevents()
        {
            // Arrange
            var token = "123abc";
            var eventId = 1234567;

            List<MpEventDto> mpEventDtos = new List<MpEventDto>();

            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpEventDto>(It.IsAny<string>(), It.IsAny<List<string>>())).Returns(mpEventDtos);

            // Act
            _fixture.GetEventAndCheckinSubevents(token, eventId);

            // Assert
            _ministryPlatformRestRepository.VerifyAll();
        }
    }
}
