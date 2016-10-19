using System.Web.Http.Results;
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
    }
}
