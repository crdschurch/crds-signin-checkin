using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using NUnit.Framework;
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
            _eventRepository = new Mock<IEventRepository>();
            _roomRepository = new Mock<IRoomRepository>();
            _fixture = new RoomService(_eventRepository.Object, _roomRepository.Object);
        }

        [Test]
        public void ShouldGetEventRooms()
        {
            // Arrange

            MpEventDto mpEventDto = new MpEventDto
            {
                EventId = 1234567,
                LocationId = 3
            };

            _eventRepository.Setup(m => m.GetEventById(1234567)).Returns(mpEventDto);

            List<MpEventRoomDto> mpEventRoomDtos = new List<MpEventRoomDto>
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
    }
}
