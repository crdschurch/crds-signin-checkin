using FluentAssertions;
using System.Collections.Generic;
using AutoMapper;
using Crossroads.Utilities.Services.Interfaces;
using MinistryPlatform.Translation.Models;
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
        private Mock<IAttributeRepository> _attributeRepository;
        private Mock<IGroupRepository> _groupRepository;
        private Mock<IApplicationConfiguration> _applicationConfiguration;

        private const int AgesAttributeTypeId = 123;
        private const int BirthMonthsAttributeTypeId = 456;
        private const int GradesAttributeTypeId = 789;
        private const int NurseryAgeAttributeId = 234;
        private const int NurseryAgesAttributeTypeId = 567;

        private RoomService _fixture;

        [SetUp]
        public void SetUp()
        {
            AutoMapperConfig.RegisterMappings();

            _eventRepository = new Mock<IEventRepository>(MockBehavior.Strict);
            _roomRepository = new Mock<IRoomRepository>(MockBehavior.Strict);
            _attributeRepository = new Mock<IAttributeRepository>(MockBehavior.Strict);
            _groupRepository = new Mock<IGroupRepository>(MockBehavior.Strict);
            _applicationConfiguration = new Mock<IApplicationConfiguration>();
            _applicationConfiguration.SetupGet(mocked => mocked.AgesAttributeTypeId).Returns(AgesAttributeTypeId);
            _applicationConfiguration.SetupGet(mocked => mocked.BirthMonthsAttributeTypeId).Returns(BirthMonthsAttributeTypeId);
            _applicationConfiguration.SetupGet(mocked => mocked.GradesAttributeTypeId).Returns(GradesAttributeTypeId);
            _applicationConfiguration.SetupGet(mocked => mocked.NurseryAgeAttributeId).Returns(NurseryAgeAttributeId);
            _applicationConfiguration.SetupGet(mocked => mocked.NurseryAgesAttributeTypeId).Returns(NurseryAgesAttributeTypeId);

            _fixture = new RoomService(_eventRepository.Object, _roomRepository.Object, _attributeRepository.Object, _groupRepository.Object, _applicationConfiguration.Object);
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

        [Test]
        public void TestGetEventRoomAgesAndGradesNoEventGroups()
        {
            // TODO Finish this test
            var ages = new List<MpAttributeDto>
            {
                new MpAttributeDto
                {

                }
            };
            _attributeRepository.Setup(mocked => mocked.GetAttributesByAttributeTypeId(AgesAttributeTypeId, null)).Returns(ages);
        }
    }
}
