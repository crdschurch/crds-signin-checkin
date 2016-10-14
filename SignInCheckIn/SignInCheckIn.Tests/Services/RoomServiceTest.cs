using AutoMapper;
using FluentAssertions;
using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using NUnit.Framework;
using SignInCheckIn.App_Start;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Services;

namespace SignInCheckIn.Tests.Services
{
    public class RoomServiceTest
    {
        private Mock<IEventRepository> _eventRepository;
        private Mock<IRoomRepository> _roomRepository;

        private RoomService _fixture;

        [SetUp]
        public void SetUp()
        {
            AutoMapperConfig.RegisterMappings();

            _eventRepository = new Mock<IEventRepository>(MockBehavior.Strict);
            _roomRepository = new Mock<IRoomRepository>(MockBehavior.Strict);

            _fixture = new RoomService(_eventRepository.Object, _roomRepository.Object);
        }

        public void ShouldGetEventRooms()
        {
            // Arrange
            var mpEventDto = new MpEventDto
            {
                EventId = 1234567,
                LocationId = 3
            };

            _eventRepository.Setup(m => m.GetEventById(1234567)).Returns(mpEventDto);

            var mpEventRoomDtos = new List<MpEventRoomDto>
            {
                new MpEventRoomDto
                {
                    AllowSignIn = false,
                    Capacity = 0,
                    CheckedIn = 0,
                    EventId = 1234567,
                    EventRoomId = 123,
                    RoomName = "Test Room",
                    SignedIn = 0,
                    Volunteers = 0
                }
            };

            _roomRepository.Setup(m => m.GetRoomsForEvent(mpEventDto.EventId, mpEventDto.LocationId)).Returns(mpEventRoomDtos);

            // Act
            var result = _fixture.GetLocationRoomsByEventId(1234567);

            // Assert
            Assert.IsNotNull(result);
            _roomRepository.VerifyAll();
            _eventRepository.VerifyAll();
        }

        [Test]
        public void TestCreateOrUpdateEventRoom()
        {
            var eventRoom = new EventRoomDto
            {
                AllowSignIn = true,
                Capacity = 1,
                CheckedIn = 2,
                EventId = 3,
                EventRoomId = 999,
                RoomId = 4,
                RoomName = "name",
                RoomNumber = "number",
                SignedIn = 5,
                Volunteers = 6
            };

            var newMpEventRoom = new MpEventRoomDto
            {
                AllowSignIn = false,
                Capacity = 11,
                CheckedIn = 22,
                EventId = 33,
                EventRoomId = 9999,
                RoomId = 44,
                RoomName = "namename",
                RoomNumber = "numbernumber",
                SignedIn = 55,
                Volunteers = 66
            };

            var newEventRoom = Mapper.Map<EventRoomDto>(newMpEventRoom);

            _roomRepository.Setup(mocked => mocked.CreateOrUpdateEventRoom("token", It.IsAny<MpEventRoomDto>())).Returns(newMpEventRoom);
            var result = _fixture.CreateOrUpdateEventRoom("token", eventRoom);
            _roomRepository.VerifyAll();

            Assert.IsNotNull(result);
            result.ShouldBeEquivalentTo(newEventRoom);
        }
    }
}
