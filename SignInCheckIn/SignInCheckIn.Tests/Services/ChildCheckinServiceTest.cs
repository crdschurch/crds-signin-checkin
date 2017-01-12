using System;
using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using NUnit.Framework;
using SignInCheckIn.App_Start;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Services;
using SignInCheckIn.Services.Interfaces;

namespace SignInCheckIn.Tests.Services
{
    public class ChildCheckinServiceTest
    {
        private Mock<IChildCheckinRepository> _childCheckinRepository;
        private Mock<IContactRepository> _contactRepository;
        private Mock<IEventService> _eventService;

        private ChildCheckinService _fixture;

        [SetUp]
        public void SetUp()
        {
            AutoMapperConfig.RegisterMappings();

            _childCheckinRepository = new Mock<IChildCheckinRepository>();
            _contactRepository = new Mock<IContactRepository>();
            _eventService = new Mock<IEventService>();

            _fixture = new ChildCheckinService(_childCheckinRepository.Object, _contactRepository.Object, _eventService.Object);
        }

        [Test]
        public void ShouldGetChildrenForCurrentEventAndRoomNoEventId()
        {
            var siteId = 1;
            var roomId = 321;

            var eventDto = new EventDto
            {
                EarlyCheckinPeriod = 30,
                EventEndDate = DateTime.Now.AddDays(1),
                EventId = 1234567,
                EventStartDate = DateTime.Now,
                EventTitle = "test event",
                EventType = "type test",
                LateCheckinPeriod = 30,
            };

            var mpParticipantsDto = new List<MpParticipantDto>
            {
                new MpParticipantDto
                {
                    ParticipantId = 12,
                    ContactId = 1443,
                    HouseholdPositionId = 2,
                    FirstName = "First1",
                    LastName = "Last1",
                    DateOfBirth = new DateTime()
                }
            };

            _eventService.Setup(m => m.GetCurrentEventForSite(It.IsAny<int>())).Returns(eventDto);
            _childCheckinRepository.Setup(m => m.GetChildrenByEventAndRoom(eventDto.EventId, roomId)).Returns(mpParticipantsDto);
            var result = _fixture.GetChildrenForCurrentEventAndRoom(roomId, siteId, null);
            _childCheckinRepository.VerifyAll();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(mpParticipantsDto[0].ParticipantId, result.Participants[0].ParticipantId);
            Assert.AreEqual(mpParticipantsDto[0].ContactId, result.Participants[0].ContactId);
            Assert.AreEqual(result.CurrentEvent.EventId, eventDto.EventId);
        }

        [Test]
        public void ShouldGetChildrenForCurrentEventAndRoomEventId()
        {
            var siteId = 1;
            var roomId = 321;

            var eventDto = new EventDto
            {
                EarlyCheckinPeriod = 30,
                EventEndDate = DateTime.Now.AddDays(1),
                EventId = 1234567,
                EventStartDate = DateTime.Now,
                EventTitle = "test event",
                EventType = "type test",
                LateCheckinPeriod = 30,
            };

            var mpParticipantsDto = new List<MpParticipantDto>
            {
                new MpParticipantDto
                {
                    ParticipantId = 12,
                    ContactId = 1443,
                    HouseholdPositionId = 2,
                    FirstName = "First1",
                    LastName = "Last1",
                    DateOfBirth = new DateTime()
                }
            };

            _eventService.Setup(m => m.GetEvent(It.IsAny<int>())).Returns(eventDto);
            _childCheckinRepository.Setup(m => m.GetChildrenByEventAndRoom(eventDto.EventId, roomId)).Returns(mpParticipantsDto);
            var result = _fixture.GetChildrenForCurrentEventAndRoom(roomId, siteId, eventDto.EventId);
            _childCheckinRepository.VerifyAll();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(mpParticipantsDto[0].ParticipantId, result.Participants[0].ParticipantId);
            Assert.AreEqual(mpParticipantsDto[0].ContactId, result.Participants[0].ContactId);
            Assert.AreEqual(result.CurrentEvent.EventId, eventDto.EventId);
        }

        [Test]
        public void TestCheckinChildrenForCurrentEventAndRoom()
        {
            var dto = new ParticipantDto
            {
                ParticipantId = 12,
                ContactId = 1443,
                HouseholdPositionId = 2,
                FirstName = "First1",
                LastName = "Last1",
                DateOfBirth = new DateTime(),
                ParticipationStatusId = 3
            };

            _childCheckinRepository.Setup(m => m.CheckinChildrenForCurrentEventAndRoom(It.IsAny<int>(), It.IsAny<int>()));
            _fixture.CheckinChildrenForCurrentEventAndRoom(dto);
            _childCheckinRepository.VerifyAll();
        }
    }
}
