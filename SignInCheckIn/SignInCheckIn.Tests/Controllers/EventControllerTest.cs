using System.Collections.Generic;
using System.Web.Http.Results;
using FluentAssertions;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using NUnit.Framework;
using SignInCheckIn.Controllers;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Services.Interfaces;

namespace SignInCheckIn.Tests.Controllers
{
    public class EventControllerTest
    {
        private EventController _fixture;
        private Mock<IEventService> _eventService;
        private Mock<IRoomService> _roomService;
        private Mock<IAuthenticationRepository> _authenticationRepository;

        private const string AuthType = "abc";
        private const string AuthToken = "123";
        private readonly string _auth = $"{AuthType} {AuthToken}";


        [SetUp]
        public void SetUp()
        {
            _eventService = new Mock<IEventService>(MockBehavior.Strict);
            _roomService = new Mock<IRoomService>(MockBehavior.Strict);
            _authenticationRepository = new Mock<IAuthenticationRepository>(MockBehavior.Strict);

            _fixture = new EventController(_eventService.Object, _roomService.Object, _authenticationRepository.Object);
            _fixture.SetupAuthorization(AuthType, AuthToken);
        }

        [Test]
        public void TestUpdateEventRoom()
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

            var newEventRoom = new EventRoomDto
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

            _roomService.Setup(mocked => mocked.CreateOrUpdateEventRoom(_auth, eventRoom)).Returns(newEventRoom);
            var response = _fixture.UpdateEventRoom(123, 456, eventRoom);
            _roomService.VerifyAll();

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<EventRoomDto>>(response);
            var result = (OkNegotiatedContentResult<EventRoomDto>) response;
            Assert.IsNotNull(result.Content);
            Assert.AreSame(newEventRoom, result.Content);
        }

        [Test]
        public void TestGetEventRoomAgesAndGradesWithAuth()
        {
            const int eventId = 123;
            const int roomId = 456;
            var eventRoom = new EventRoomDto
            {
                AssignedGroups = new List<AgeGradeDto>()
            };

            _roomService.Setup(mocked => mocked.GetEventRoomAgesAndGrades(_auth, eventId, roomId)).Returns(eventRoom);

            var result = _fixture.GetEventRoomAgesAndGrades(eventId, roomId);
            _roomService.VerifyAll();
            result.Should().BeOfType<OkNegotiatedContentResult<EventRoomDto>>();
            ((OkNegotiatedContentResult<EventRoomDto>)result).Content.Should().BeSameAs(eventRoom);
        }

        [Test]
        public void TestGetEventRoomAgesAndGradesWithoutAuth()
        {
            const int eventId = 123;
            const int roomId = 456;
            var eventRoom = new EventRoomDto
            {
                AssignedGroups = new List<AgeGradeDto>()
            };

            _roomService.Setup(mocked => mocked.GetEventRoomAgesAndGrades(null, eventId, roomId)).Returns(eventRoom);

            _fixture.RemoveAuthorization();
            var result = _fixture.GetEventRoomAgesAndGrades(eventId, roomId);
            _roomService.VerifyAll();
            result.Should().BeOfType<OkNegotiatedContentResult<EventRoomDto>>();
            ((OkNegotiatedContentResult<EventRoomDto>)result).Content.Should().BeSameAs(eventRoom);
        }

        [Test]
        public void TestGetEvent()
        {
            const int eventId = 123;
            var e = new EventDto();
            _eventService.Setup(mocked => mocked.GetEvent(eventId)).Returns(e);

            var result = _fixture.GetEvent(eventId);
            _eventService.VerifyAll();
            result.Should().BeOfType<OkNegotiatedContentResult<EventDto>>();
            ((OkNegotiatedContentResult<EventDto>)result).Content.Should().BeSameAs(e);
        }

        [Test]
        public void TestImportEventSetup()
        {
            const int destinationEventId = 12345;
            const int sourceEventId = 67890;
            var rooms = new List<EventRoomDto>();
            _eventService.Setup(mocked => mocked.ImportEventSetup(_auth, destinationEventId, sourceEventId)).Returns(rooms);

            var result = _fixture.ImportEventSetup(destinationEventId, sourceEventId);
            _eventService.VerifyAll();
            result.Should().BeOfType<OkNegotiatedContentResult<List<EventRoomDto>>>();
            ((OkNegotiatedContentResult<List<EventRoomDto>>)result).Content.Should().BeSameAs(rooms);
        }

        [Test]
        public void TestResetEventSetup()
        {
            const int eventId = 12345;
            var rooms = new List<EventRoomDto>();
            _eventService.Setup(mocked => mocked.ResetEventSetup(_auth, eventId)).Returns(rooms);

            var result = _fixture.ResetEventSetup(eventId);
            _eventService.VerifyAll();
            result.Should().BeOfType<OkNegotiatedContentResult<List<EventRoomDto>>>();
            ((OkNegotiatedContentResult<List<EventRoomDto>>)result).Content.Should().BeSameAs(rooms);
        }
    }
}
