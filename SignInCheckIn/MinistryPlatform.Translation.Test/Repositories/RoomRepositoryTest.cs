﻿using Crossroads.Utilities.Services.Interfaces;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

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
                "Room_ID_Table.KC_Sort_Order",
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
                "Room_Number",
                "KC_Sort_Order"
            };

            _fixture = new RoomRepository(_apiUserRepository.Object, _ministryPlatformRestRepository.Object, _applicationConfiguration.Object);
        }

        [Test]
        public void TestCreateEventRoomNoExistingEventRoom()
        {
            var token = "auth";
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

            _apiUserRepository.Setup(m => m.GetApiClientToken("CRDS.Service.SignCheckIn")).Returns(token);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken("auth")).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(
                mocked => mocked.Search<MpEventRoomDto>($"Event_Rooms.Event_ID = {eventRoom.EventId} AND Event_Rooms.Room_ID = {eventRoom.RoomId}", It.IsAny<List<string>>(), null, false))
                .Returns(new List<MpEventRoomDto>());
            _ministryPlatformRestRepository.Setup(mocked => mocked.Create(eventRoom, _eventRoomColumns)).Returns(created);
            var result = _fixture.CreateOrUpdateEventRoom(eventRoom);
            _ministryPlatformRestRepository.VerifyAll();

            Assert.IsNotNull(result);
            Assert.AreSame(created, result);
        }

        [Test]
        public void TestCreateEventRoomExistingEventRoomShouldBeUpdated()
        {
            var token = "auth";
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

            _apiUserRepository.Setup(m => m.GetApiClientToken("CRDS.Service.SignCheckIn")).Returns(token);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken("auth")).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(
                mocked => mocked.Search<MpEventRoomDto>($"Event_Rooms.Event_ID = {eventRoom.EventId} AND Event_Rooms.Room_ID = {eventRoom.RoomId}", It.IsAny<List<string>>(), null, false))
                .Returns(new List<MpEventRoomDto>
                {
                    new MpEventRoomDto
                    {
                        EventRoomId = 999
                    }
                });
            _ministryPlatformRestRepository.Setup(mocked => mocked.Update(eventRoom, _eventRoomColumns)).Returns(updated);
            var result = _fixture.CreateOrUpdateEventRoom(eventRoom);
            _ministryPlatformRestRepository.VerifyAll();
            _ministryPlatformRestRepository.Verify(mocked => mocked.Update(It.Is<MpEventRoomDto>(e => e == eventRoom && e.EventRoomId == 999), _eventRoomColumns));

            Assert.IsNotNull(result);
            Assert.AreSame(updated, result);
        }

        [Test]
        public void TestUpdateEventRoom()
        {
            var token = "auth";
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

            _apiUserRepository.Setup(m => m.GetApiClientToken("CRDS.Service.SignCheckIn")).Returns(token);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Update(eventRoom, _eventRoomColumns)).Returns(updated);
            var result = _fixture.CreateOrUpdateEventRoom(eventRoom);
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

            _apiUserRepository.Setup(mocked => mocked.GetApiClientToken("CRDS.Service.SignCheckIn")).Returns(token);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpEventRoomDto>($"Event_Rooms.Event_ID = {eventId} AND Event_Rooms.Room_ID = {roomId}", _eventRoomColumns, null, false))
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

            _apiUserRepository.Setup(mocked => mocked.GetApiClientToken("CRDS.Service.SignCheckIn")).Returns(token);
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
        public void TestGetBumpingRoomsVacancy()
        {
            const string token = "token 123";
            const int eventId = 1837;
            const int fromEventRoomId = 84672817;

            var bumpingRoomsColumns = new List<string>
            {
                "To_Event_Room_ID",
                "To_Event_Room_ID_Table.Room_ID",
                "Priority_Order",
                "Bumping_Rule_Type_ID",
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
                    SignedIn = 1,
                    CheckedIn = 1,
                    BumpingRuleTypeId = 2
                },
                new MpBumpingRoomsDto
                {
                    EventRoomId = 4323,
                    RoomId = 5344,
                    PriorityOrder = 3,
                    AllowSignIn = true,
                    Capacity = 32,
                    RoomName = "Test Room 1",
                    SignedIn = 4,
                    CheckedIn = 13,
                    BumpingRuleTypeId = 2
                },
                new MpBumpingRoomsDto
                {
                    EventRoomId = 1248,
                    RoomId = 3827,
                    PriorityOrder = 1,
                    AllowSignIn = true,
                    Capacity = 10,
                    RoomName = "Test Room 2",
                    SignedIn = 1,
                    CheckedIn = 4,
                    BumpingRuleTypeId = 2
                }
            };

            _applicationConfiguration.Setup(m => m.BumpingRoomTypeVacancyId).Returns(2);
            _apiUserRepository.Setup(mocked => mocked.GetApiClientToken("CRDS.Service.SignCheckIn")).Returns(token);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpBumpingRoomsDto>($"From_Event_Room_ID = {fromEventRoomId}", bumpingRoomsColumns, null, false)).Returns(mpBumpingRooms);

            var result = _fixture.GetBumpingRoomsForEventRoom(eventId, fromEventRoomId);
            _ministryPlatformRestRepository.VerifyAll();
            _apiUserRepository.VerifyAll();

            Assert.AreEqual(mpBumpingRooms.Count, result.Count);
            Assert.AreEqual(mpBumpingRooms[0].EventRoomId, result[0].EventRoomId);
            Assert.AreEqual(mpBumpingRooms[1].EventRoomId, result[2].EventRoomId);
            Assert.AreEqual(mpBumpingRooms[2].EventRoomId, result[1].EventRoomId);
            Assert.AreEqual(mpBumpingRooms[2].RoomId, result[1].RoomId);
            Assert.AreEqual(mpBumpingRooms[2].PriorityOrder, result[1].PriorityOrder);
            Assert.AreEqual(mpBumpingRooms[2].AllowSignIn, result[1].AllowSignIn);
            Assert.AreEqual(mpBumpingRooms[2].Capacity, result[1].Capacity);
            Assert.AreEqual(mpBumpingRooms[2].RoomName, result[1].RoomName);
            Assert.AreEqual(mpBumpingRooms[2].SignedIn, result[1].SignedIn);
            Assert.AreEqual(mpBumpingRooms[2].CheckedIn, result[1].CheckedIn);
        }

        [Test]
        public void TestGetBumpingRoomsPriority()
        {
            const string token = "token 123";
            const int eventId = 1837;
            const int fromEventRoomId = 84672817;

            var bumpingRoomsColumns = new List<string>
            {
                "To_Event_Room_ID",
                "To_Event_Room_ID_Table.Room_ID",
                "Priority_Order",
                "Bumping_Rule_Type_ID",
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
                    SignedIn = 1,
                    CheckedIn = 13,
                    BumpingRuleTypeId = 1
                },
                new MpBumpingRoomsDto
                {
                    EventRoomId = 4323,
                    RoomId = 5344,
                    PriorityOrder = 3,
                    AllowSignIn = true,
                    Capacity = 32,
                    RoomName = "Test Room 1",
                    SignedIn = 1,
                    CheckedIn = 13,
                    BumpingRuleTypeId = 1
                },
                new MpBumpingRoomsDto
                {
                    EventRoomId = 1248,
                    RoomId = 3827,
                    PriorityOrder = 1,
                    AllowSignIn = true,
                    Capacity = 10,
                    RoomName = "Test Room 2",
                    SignedIn = 2,
                    CheckedIn = 1,
                    BumpingRuleTypeId = 1
                }
            };

            _applicationConfiguration.Setup(m => m.BumpingRoomTypeVacancyId).Returns(2);
            _apiUserRepository.Setup(mocked => mocked.GetApiClientToken("CRDS.Service.SignCheckIn")).Returns(token);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpBumpingRoomsDto>($"From_Event_Room_ID = {fromEventRoomId}", bumpingRoomsColumns, null, false)).Returns(mpBumpingRooms);

            var result = _fixture.GetBumpingRoomsForEventRoom(eventId, fromEventRoomId);
            _ministryPlatformRestRepository.VerifyAll();
            _apiUserRepository.VerifyAll();

            Assert.AreEqual(mpBumpingRooms.Count, result.Count);
            Assert.AreEqual(mpBumpingRooms[2].EventRoomId, result[0].EventRoomId);
            Assert.AreEqual(mpBumpingRooms[0].EventRoomId, result[1].EventRoomId);
            Assert.AreEqual(mpBumpingRooms[1].EventRoomId, result[2].EventRoomId);
            Assert.AreEqual(mpBumpingRooms[2].RoomId, result[0].RoomId);
            Assert.AreEqual(mpBumpingRooms[2].PriorityOrder, result[0].PriorityOrder);
            Assert.AreEqual(mpBumpingRooms[2].AllowSignIn, result[0].AllowSignIn);
            Assert.AreEqual(mpBumpingRooms[2].Capacity, result[0].Capacity);
            Assert.AreEqual(mpBumpingRooms[2].RoomName, result[0].RoomName);
            Assert.AreEqual(mpBumpingRooms[2].SignedIn, result[0].SignedIn);
            Assert.AreEqual(mpBumpingRooms[2].CheckedIn, result[0].CheckedIn);
        }

    }
}
