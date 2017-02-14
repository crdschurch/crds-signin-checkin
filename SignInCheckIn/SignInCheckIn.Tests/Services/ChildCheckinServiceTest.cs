using System;
using System.Collections.Generic;
using Crossroads.Utilities.Services.Interfaces;
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
        private Mock<IApplicationConfiguration> _applicationConfiguration;
        private Mock<IRoomRepository> _roomRepository;
        private Mock<IEventRepository> _eventRepository;

        private ChildCheckinService _fixture;

        [SetUp]
        public void SetUp()
        {
            AutoMapperConfig.RegisterMappings();

            _childCheckinRepository = new Mock<IChildCheckinRepository>();
            _contactRepository = new Mock<IContactRepository>();
            _eventService = new Mock<IEventService>();
            _roomRepository = new Mock<IRoomRepository>();
            _applicationConfiguration = new Mock<IApplicationConfiguration>();
            _eventRepository = new Mock<IEventRepository>();

            _fixture = new ChildCheckinService(_childCheckinRepository.Object, _contactRepository.Object, _roomRepository.Object, _applicationConfiguration.Object, _eventService.Object,
                _eventRepository.Object);
        }

        [Test]
        public void ShouldGetChildrenForCurrentEventAndRoomNoEventIdNonAc()
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

            //var subEventDto = new MpEventDto
            //{
            //    EventId = 2345678,
            //    ParentEventId = 1234567
            //};

            List<MpEventDto> subEvents = new List<MpEventDto>
            {
                //subEventDto
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

            var mpEventRoomDto = new MpEventRoomDto
            {
                EventId = 1234567,
                RoomId = roomId
            }; 

            _eventService.Setup(m => m.GetCurrentEventForSite(It.IsAny<int>())).Returns(eventDto);
            _eventRepository.Setup(m => m.GetSubeventsForEvents(It.IsAny<List<int>>(), null)).Returns(subEvents);
            _roomRepository.Setup(m => m.GetEventRoomForEventMaps(It.IsAny<List<int>>(), roomId)).Returns(mpEventRoomDto);
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
        public void ShouldGetChildrenForCurrentEventAndRoomEventIdNonAc()
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

            //var subEventDto = new MpEventDto
            //{
            //    EventId = 2345678,
            //    ParentEventId = 1234567
            //};

            List<MpEventDto> subEvents = new List<MpEventDto>
            {
                //subEventDto
            };

            var mpEventRoomDto = new MpEventRoomDto
            {
                EventId = 1234567,
                RoomId = roomId
            };

            _eventService.Setup(m => m.GetEvent(It.IsAny<int>())).Returns(eventDto);
            _eventRepository.Setup(m => m.GetSubeventsForEvents(It.IsAny<List<int>>(), null)).Returns(subEvents);
            _roomRepository.Setup(m => m.GetEventRoomForEventMaps(It.IsAny<List<int>>(), roomId)).Returns(mpEventRoomDto);
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
        public void ShouldGetChildrenForCurrentEventAndRoomEventIdWithAcSubevent()
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

            var subEventDto = new MpEventDto
            {
                EventId = 2345678,
                ParentEventId = 1234567
            };

            List<MpEventDto> subEvents = new List<MpEventDto>
            {
                subEventDto
            };

            var mpEventRoomDto = new MpEventRoomDto
            {
                EventId = 1234567,
                RoomId = roomId
            };

            _eventService.Setup(m => m.GetEvent(It.IsAny<int>())).Returns(eventDto);
            _eventRepository.Setup(m => m.GetSubeventsForEvents(It.IsAny<List<int>>(), null)).Returns(subEvents);
            _roomRepository.Setup(m => m.GetEventRoomForEventMaps(It.IsAny<List<int>>(), roomId)).Returns(mpEventRoomDto);
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
        public void ShouldGetChildrenForCurrentEventAndRoomEventIdWithAcSubEvent()
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

            var subEventDto = new MpEventDto
            {
                EventId = 2345678,
                ParentEventId = 1234567
            };

            List<MpEventDto> subEvents = new List<MpEventDto>
            {
                subEventDto
            };

            var mpEventRoomDto = new MpEventRoomDto
            {
                EventId = 1234567,
                RoomId = roomId
            };

            _eventService.Setup(m => m.GetEvent(It.IsAny<int>())).Returns(eventDto);
            _eventRepository.Setup(m => m.GetSubeventsForEvents(It.IsAny<List<int>>(), null)).Returns(subEvents);
            _roomRepository.Setup(m => m.GetEventRoomForEventMaps(It.IsAny<List<int>>(), roomId)).Returns(mpEventRoomDto);
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
        public void ShouldGetChildrenForCurrentEventAndAdventureClubRoomEventIdWithAcSubEvent()
        {
            var siteId = 1;
            var roomId = 321;

            var eventDto = new EventDto
            {
                EarlyCheckinPeriod = 30,
                EventEndDate = DateTime.Now.AddDays(1),
                EventId = 2345678,
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

            var subEventDto = new MpEventDto
            {
                EventId = 2345678,
                ParentEventId = 1234567
            };

            List<MpEventDto> subEvents = new List<MpEventDto>
            {
                subEventDto
            };

            var mpEventRoomDto = new MpEventRoomDto
            {
                EventId = 2345678,
                RoomId = roomId
            };

            _eventService.Setup(m => m.GetEvent(It.IsAny<int>())).Returns(eventDto);
            _eventRepository.Setup(m => m.GetSubeventsForEvents(It.IsAny<List<int>>(), null)).Returns(subEvents);
            _roomRepository.Setup(m => m.GetEventRoomForEventMaps(It.IsAny<List<int>>(), roomId)).Returns(mpEventRoomDto);
            _childCheckinRepository.Setup(m => m.GetChildrenByEventAndRoom(2345678, roomId)).Returns(mpParticipantsDto);
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

        [Test]
        public void ShouldGetEventParticipantByCallNumber()
        {
            var eventId = 888;
            var subeventId = 999;
            var callNumber = 44;
            var roomId = 321;

            var mpEventParticipantDto = new MpEventParticipantDto
                {
                    EventParticipantId = 12,
                    FirstName = "First1",
                    LastName = "Last1",
                    CheckinHouseholdId = 432234,
                    DateOfBirth = new DateTime()
                };

            var mpContactDtos = new List<MpContactDto>
            {
                new MpContactDto
                {
                    FirstName = "George"
                }
            };

            var subevent = new MpEventDto
            {
                EventId = 333
            };

            _applicationConfiguration.Setup(m => m.CheckedInParticipationStatusId).Returns(4);
            _childCheckinRepository.Setup(m => m.GetEventParticipantByCallNumber(It.IsAny<List<int>>(), It.IsAny<int>())).Returns(mpEventParticipantDto);
            _contactRepository.Setup(m => m.GetHeadsOfHouseholdByHouseholdId(mpEventParticipantDto.CheckinHouseholdId.Value)).Returns(mpContactDtos);
            _eventRepository.Setup(mocked => mocked.GetSubeventByParentEventId(It.IsAny<int>(), It.IsAny<int>())).Returns(subevent);
            var result = _fixture.GetEventParticipantByCallNumber(eventId, callNumber, roomId, true);
            _childCheckinRepository.VerifyAll();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(mpEventParticipantDto.EventParticipantId, result.EventParticipantId);
            Assert.AreEqual(mpEventParticipantDto.FirstName, result.FirstName);
        }

        [Test]
        public void ShouldOverrideChildIntoRoom()
        {
            int eventId = 321;
            int eventParticipantId = 444;
            int roomId = 111;

            var eventRoom = new MpEventRoomDto
            {
                AllowSignIn = true,
                EventRoomId = 333,
                EventId = 222,
                RoomId = 111,
                CheckedIn = 2,
                SignedIn = 5,
                Capacity = 20
            };

            _roomRepository.Setup(m => m.GetEventRoom(It.IsAny<int>(), It.IsAny<int>())).Returns(eventRoom);
            _childCheckinRepository.Setup(m => m.OverrideChildIntoRoom(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()));
            _fixture.OverrideChildIntoRoom(eventId, eventParticipantId, roomId);
            _childCheckinRepository.VerifyAll();
        }
    }
}
