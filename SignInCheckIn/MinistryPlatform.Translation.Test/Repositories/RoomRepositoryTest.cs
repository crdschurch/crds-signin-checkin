using System.Collections.Generic;
using Crossroads.Utilities.Services.Interfaces;
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
        private Mock<IApplicationConfiguration> _applicationConfiguration;

        private List<string> _eventRoomColumns;
        private List<string> _roomColumns;

        [SetUp]
        public void SetUp()
        {
            _apiUserRepository = new Mock<IApiUserRepository>(MockBehavior.Strict);
            _ministryPlatformRestRepository = new Mock<IMinistryPlatformRestRepository>(MockBehavior.Strict);
            _applicationConfiguration = new Mock<IApplicationConfiguration>(MockBehavior.Strict);

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

            _fixture = new RoomRepository(_apiUserRepository.Object, _ministryPlatformRestRepository.Object, _applicationConfiguration.Object);
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
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpEventRoomDto>($"Event_Rooms.Event_ID = {eventId} AND Event_Rooms.Room_ID = {roomId}", _eventRoomColumns))
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

        [Test]
        public void TestRoomUsageTypeKidsClub()
        {
            _applicationConfiguration.Setup(m => m.RoomUsageTypeKidsClub).Returns(6);
        }


        [Test]
        public void TestGetBumpingRooms()
        {
            const string token = "token 123";
            const int eventId = 1837;
            const int fromEventRoomId = 84672817;

            var bumpingRoomsColumns = new List<string>
            {
                "To_Event_Room_ID",
                "To_Event_Room_ID_Table.Room_ID",
                "Priority_Order",
                "To_Event_Room_ID_Table.Capacity",
                "To_Event_Room_ID_Table.Allow_Checkin",
                "To_Event_Room_ID_Table_Room_ID_Table.Room_Name",
                $"[dbo].crds_getEventParticipantStatusCount({eventId}, To_Event_Room_ID_Table.Room_Id, 3) AS Signed_In",
                $"[dbo].crds_getEventParticipantStatusCount({eventId}, To_Event_Room_ID_Table.Room_Id, 4) AS Checked_In"
            };

            var mpBumpingRooms = new List<MpBumpingRoomsDto>
            {
                new MpBumpingRoomsDto
                {
                    EventRoomId = 5134,
                    RoomId = 161641,
                    PriorityOrder = 2,
                    AllowSignIn = true,
                    Capacity = 32,
                    RoomName = "Test Room 1",
                    SignedIn = 93,
                    CheckedIn = 12
                },
                new MpBumpingRoomsDto
                {
                    EventRoomId = 1248,
                    RoomId = 3827,
                    PriorityOrder = 1,
                    AllowSignIn = true,
                    Capacity = 10,
                    RoomName = "Test Room 2",
                    SignedIn = 9,
                    CheckedIn = 1
                }
            };


            _apiUserRepository.Setup(mocked => mocked.GetToken()).Returns(token);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpBumpingRoomsDto>($"From_Event_Room_ID = {fromEventRoomId}", bumpingRoomsColumns)).Returns(mpBumpingRooms);

            var result = _fixture.GetBumpingRoomsForEventRoom(eventId, fromEventRoomId);
            _ministryPlatformRestRepository.VerifyAll();
            _apiUserRepository.VerifyAll();

            Assert.AreEqual(mpBumpingRooms.Count, result.Count);
            Assert.AreEqual(mpBumpingRooms[1].EventRoomId, result[0].EventRoomId);
            Assert.AreEqual(mpBumpingRooms[1].RoomId, result[0].RoomId);
            Assert.AreEqual(mpBumpingRooms[1].PriorityOrder, result[0].PriorityOrder);
            Assert.AreEqual(mpBumpingRooms[1].AllowSignIn, result[0].AllowSignIn);
            Assert.AreEqual(mpBumpingRooms[1].Capacity, result[0].Capacity);
            Assert.AreEqual(mpBumpingRooms[1].RoomName, result[0].RoomName);
            Assert.AreEqual(mpBumpingRooms[1].SignedIn, result[0].SignedIn);
            Assert.AreEqual(mpBumpingRooms[1].CheckedIn, result[0].CheckedIn);
        }

    }
}
