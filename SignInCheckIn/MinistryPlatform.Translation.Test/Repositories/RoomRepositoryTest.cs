using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using NUnit.Framework;

namespace MinistryPlatform.Translation.Test.Repositories
{
    public class RoomRepositoryTest
    {
        private RoomRepository _fixture;
        private Mock<IApiUserRepository> _apiUserRepository;
        private Mock<IMinistryPlatformRestRepository> _ministryPlatformRestRepository;

        private List<string> _eventRoomColumns;
        private List<string> _roomColumns;

        [SetUp]
        public void SetUp()
        {
            _apiUserRepository = new Mock<IApiUserRepository>(MockBehavior.Strict);
            _ministryPlatformRestRepository = new Mock<IMinistryPlatformRestRepository>(MockBehavior.Strict);

            _eventRoomColumns = new List<string>
            {
                "Event_Rooms.Event_Room_ID",
                "Event_Rooms.Event_ID",
                "Event_Rooms.Room_ID",
                "Room_ID_Table.Room_Name",
                "Room_ID_Table.Room_Number",
                "Event_Rooms.Allow_Checkin",
                "Event_Rooms.Volunteers",
                "Event_Rooms.Capacity",
                "Event_Rooms.Label",
                "[dbo].crds_getEventParticipantStatusCount(Event_Rooms.Event_ID, Event_Rooms.Room_ID, 3) AS Signed_In",
                "[dbo].crds_getEventParticipantStatusCount(Event_Rooms.Event_ID, Event_Rooms.Room_ID, 4) AS Checked_In"
            };

            _roomColumns = new List<string>
            {
                "Room_ID",
                "Room_Name",
                "Room_Number"
            };

            _fixture = new RoomRepository(_apiUserRepository.Object, _ministryPlatformRestRepository.Object);
        }

        [Test]
        public void TestCreateEventRoomNoExistingEventRoom()
        {
            var eventRoom = new MpEventRoomDto
            {
                AllowSignIn = true,
                Capacity = 1,
                CheckedIn = 2,
                EventId = 3,
                EventRoomId = null,
                Hidden = true,
                RoomId = 4,
                RoomName = "name",
                RoomNumber = "number",
                SignedIn = 5,
                Volunteers = 6
            };

            var created = new MpEventRoomDto();

            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken("auth")).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(
                mocked => mocked.Search<MpEventRoomDto>($"Event_Rooms.Event_ID = {eventRoom.EventId} AND Event_Rooms.Room_ID = {eventRoom.RoomId}", It.IsAny<List<string>>()))
                .Returns(new List<MpEventRoomDto>());
            //_ministryPlatformRestRepository.Setup(
            //    mocked => mocked.Search<MpEventRoomDto>(It.IsAny<string>(), It.IsAny<List<string>>()))
            //    .Returns(new List<MpEventRoomDto>());
            _ministryPlatformRestRepository.Setup(mocked => mocked.Create(eventRoom, _eventRoomColumns)).Returns(created);
            var result = _fixture.CreateOrUpdateEventRoom("auth", eventRoom);
            _ministryPlatformRestRepository.VerifyAll();

            Assert.IsNotNull(result);
            Assert.AreSame(created, result);
        }

        [Test]
        public void TestCreateEventRoomExistingEventRoomShouldBeUpdated()
        {
            var eventRoom = new MpEventRoomDto
            {
                AllowSignIn = true,
                Capacity = 1,
                CheckedIn = 2,
                EventId = 3,
                EventRoomId = null,
                Hidden = true,
                RoomId = 4,
                RoomName = "name",
                RoomNumber = "number",
                SignedIn = 5,
                Volunteers = 6
            };

            var updated = new MpEventRoomDto();

            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken("auth")).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(
                mocked => mocked.Search<MpEventRoomDto>($"Event_Rooms.Event_ID = {eventRoom.EventId} AND Event_Rooms.Room_ID = {eventRoom.RoomId}", It.IsAny<List<string>>()))
                .Returns(new List<MpEventRoomDto>
                {
                    new MpEventRoomDto
                    {
                        EventRoomId = 999
                    }
                });
            _ministryPlatformRestRepository.Setup(mocked => mocked.Update(eventRoom, _eventRoomColumns)).Returns(updated);
            var result = _fixture.CreateOrUpdateEventRoom("auth", eventRoom);
            _ministryPlatformRestRepository.VerifyAll();
            _ministryPlatformRestRepository.Verify(mocked => mocked.Update(It.Is<MpEventRoomDto>(e => e == eventRoom && e.EventRoomId == 999), _eventRoomColumns));

            Assert.IsNotNull(result);
            Assert.AreSame(updated, result);
        }

        [Test]
        public void TestUpdateEventRoom()
        {
            var eventRoom = new MpEventRoomDto
            {
                AllowSignIn = true,
                Capacity = 1,
                CheckedIn = 2,
                EventId = 3,
                EventRoomId = 999,
                Hidden = true,
                RoomId = 4,
                RoomName = "name",
                RoomNumber = "number",
                SignedIn = 5,
                Volunteers = 6
            };

            var updated = new MpEventRoomDto();

            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken("auth")).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Update(eventRoom, _eventRoomColumns)).Returns(updated);
            var result = _fixture.CreateOrUpdateEventRoom("auth", eventRoom);
            _ministryPlatformRestRepository.VerifyAll();

            Assert.IsNotNull(result);
            Assert.AreSame(updated, result);
        }

        [Test]
        public void TestGetEventRoom()
        {
            const int eventId = 123;
            const int roomId = 456;
            const string token = "token 123";
            var eventRoom = new MpEventRoomDto();

            _apiUserRepository.Setup(mocked => mocked.GetToken()).Returns(token);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            //_ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpEventRoomDto>($"Event_Rooms.Event_ID = {eventId} AND Event_Rooms.Room_ID = {roomId}", _eventRoomColumns))
            //    .Returns(new List<MpEventRoomDto>
            //    {
            //        eventRoom
            //    });
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpEventRoomDto>(It.IsAny<string>(), It.IsAny<List<string>>()))
                .Returns(new List<MpEventRoomDto>
                {
                    eventRoom
                });

            var result = _fixture.GetEventRoom(eventId, roomId);
            _ministryPlatformRestRepository.VerifyAll();
            _apiUserRepository.VerifyAll();

            Assert.AreSame(eventRoom, result);
        }

        public void TestGetRoom()
        {
            const int roomId = 456;
            const string token = "token 123";
            var room = new MpRoomDto();

            _apiUserRepository.Setup(mocked => mocked.GetToken()).Returns(token);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Get<MpRoomDto>(roomId, _roomColumns))
                .Returns(room);

            var result = _fixture.GetRoom(roomId);
            _ministryPlatformRestRepository.VerifyAll();
            _apiUserRepository.VerifyAll();

            Assert.AreSame(room, result);
        }


    }
}
