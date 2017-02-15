using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Crossroads.Utilities.Services.Interfaces;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private Mock<IApiUserRepository> _apiUserRepository;

        private const int AgesAttributeTypeId = 102;
        private const int BirthMonthsAttributeTypeId = 103;
        private const int GradesAttributeTypeId = 104;
        private const int NurseryAgeAttributeId = 9014;
        private const int NurseryAgesAttributeTypeId = 105;

        private List<MpAttributeDto> _ageList;
        private List<MpAttributeDto> _gradeList;
        private List<MpAttributeDto> _birthMonthList;
        private List<MpAttributeDto> _nurseryMonthList;

        private RoomService _fixture;

        [SetUp]
        public void SetUp()
        {
            AutoMapperConfig.RegisterMappings();

            _eventRepository = new Mock<IEventRepository>();
            _roomRepository = new Mock<IRoomRepository>();
            _attributeRepository = new Mock<IAttributeRepository>(MockBehavior.Strict);
            _groupRepository = new Mock<IGroupRepository>(MockBehavior.Strict);
            _applicationConfiguration = new Mock<IApplicationConfiguration>();
            _apiUserRepository = new Mock<IApiUserRepository>(MockBehavior.Strict);
            _applicationConfiguration.SetupGet(mocked => mocked.AgesAttributeTypeId).Returns(AgesAttributeTypeId);
            _applicationConfiguration.SetupGet(mocked => mocked.BirthMonthsAttributeTypeId).Returns(BirthMonthsAttributeTypeId);
            _applicationConfiguration.SetupGet(mocked => mocked.GradesAttributeTypeId).Returns(GradesAttributeTypeId);
            _applicationConfiguration.SetupGet(mocked => mocked.NurseryAgeAttributeId).Returns(NurseryAgeAttributeId);
            _applicationConfiguration.SetupGet(mocked => mocked.NurseryAgesAttributeTypeId).Returns(NurseryAgesAttributeTypeId);

            _ageList =
                JsonConvert.DeserializeObject<List<MpAttributeDto>>(
                    "[{'Attribute_ID':9014,'Attribute_Name':'Nursery','Sort_Order':1000,'Attribute_Type_ID':102,'Attribute_Type':'KC eCheck Age'},{'Attribute_ID':9015,'Attribute_Name':'First Year','Sort_Order':1,'Attribute_Type_ID':102,'Attribute_Type':'KC eCheck Age'},{'Attribute_ID':9016,'Attribute_Name':'Second Year','Sort_Order':2,'Attribute_Type_ID':102,'Attribute_Type':'KC eCheck Age'},{'Attribute_ID':9017,'Attribute_Name':'Third Year','Sort_Order':3,'Attribute_Type_ID':102,'Attribute_Type':'KC eCheck Age'},{'Attribute_ID':9018,'Attribute_Name':'Fourth Year','Sort_Order':4,'Attribute_Type_ID':102,'Attribute_Type':'KC eCheck Age'},{'Attribute_ID':9019,'Attribute_Name':'Fifth Year','Sort_Order':5,'Attribute_Type_ID':102,'Attribute_Type':'KC eCheck Age'}]");
            _gradeList =
                JsonConvert.DeserializeObject<List<MpAttributeDto>>(
                    "[{'Attribute_ID':9032,'Attribute_Name':'Kindergarten','Sort_Order':1000,'Attribute_Type_ID':104,'Attribute_Type':'KC eCheck Grade'},{'Attribute_ID':9033,'Attribute_Name':'First Grade','Sort_Order':1,'Attribute_Type_ID':104,'Attribute_Type':'KC eCheck Grade'},{'Attribute_ID':9034,'Attribute_Name':'Second Grade','Sort_Order':2,'Attribute_Type_ID':104,'Attribute_Type':'KC eCheck Grade'},{'Attribute_ID':9035,'Attribute_Name':'Third Grade','Sort_Order':3,'Attribute_Type_ID':104,'Attribute_Type':'KC eCheck Grade'},{'Attribute_ID':9036,'Attribute_Name':'Fourth Grade','Sort_Order':4,'Attribute_Type_ID':104,'Attribute_Type':'KC eCheck Grade'},{'Attribute_ID':9037,'Attribute_Name':'Fifth Grade','Sort_Order':5,'Attribute_Type_ID':104,'Attribute_Type':'KC eCheck Grade'},{'Attribute_ID':9038,'Attribute_Name':'Sixth Grade','Sort_Order':6,'Attribute_Type_ID':104,'Attribute_Type':'KC eCheck Grade'},{'Attribute_ID':9039,'Attribute_Name':'CSM','Sort_Order':7,'Attribute_Type_ID':104,'Attribute_Type':'KC eCheck Grade'}]");
            _birthMonthList =
                JsonConvert.DeserializeObject<List<MpAttributeDto>>(
                    "[{'Attribute_ID':9002,'Attribute_Name':'January','Sort_Order':1000,'Attribute_Type_ID':103,'Attribute_Type':'KC eCheck Birth Month'},{'Attribute_ID':9003,'Attribute_Name':'February','Sort_Order':1,'Attribute_Type_ID':103,'Attribute_Type':'KC eCheck Birth Month'},{'Attribute_ID':9004,'Attribute_Name':'March','Sort_Order':2,'Attribute_Type_ID':103,'Attribute_Type':'KC eCheck Birth Month'},{'Attribute_ID':9005,'Attribute_Name':'April','Sort_Order':3,'Attribute_Type_ID':103,'Attribute_Type':'KC eCheck Birth Month'},{'Attribute_ID':9006,'Attribute_Name':'May','Sort_Order':4,'Attribute_Type_ID':103,'Attribute_Type':'KC eCheck Birth Month'},{'Attribute_ID':9007,'Attribute_Name':'June','Sort_Order':5,'Attribute_Type_ID':103,'Attribute_Type':'KC eCheck Birth Month'},{'Attribute_ID':9008,'Attribute_Name':'July','Sort_Order':6,'Attribute_Type_ID':103,'Attribute_Type':'KC eCheck Birth Month'},{'Attribute_ID':9009,'Attribute_Name':'August','Sort_Order':7,'Attribute_Type_ID':103,'Attribute_Type':'KC eCheck Birth Month'},{'Attribute_ID':9010,'Attribute_Name':'September','Sort_Order':8,'Attribute_Type_ID':103,'Attribute_Type':'KC eCheck Birth Month'},{'Attribute_ID':9011,'Attribute_Name':'October','Sort_Order':9,'Attribute_Type_ID':103,'Attribute_Type':'KC eCheck Birth Month'},{'Attribute_ID':9012,'Attribute_Name':'November','Sort_Order':10,'Attribute_Type_ID':103,'Attribute_Type':'KC eCheck Birth Month'},{'Attribute_ID':9013,'Attribute_Name':'December','Sort_Order':11,'Attribute_Type_ID':103,'Attribute_Type':'KC eCheck Birth Month'}]");
            _nurseryMonthList =
                JsonConvert.DeserializeObject<List<MpAttributeDto>>(
                    "[{'Attribute_ID':9020,'Attribute_Name':'0-1','Sort_Order':1000,'Attribute_Type_ID':105,'Attribute_Type':'KC eCheck Nursery Month'},{'Attribute_ID':9021,'Attribute_Name':'1-2','Sort_Order':1,'Attribute_Type_ID':105,'Attribute_Type':'KC eCheck Nursery Month'},{'Attribute_ID':9022,'Attribute_Name':'2-3','Sort_Order':2,'Attribute_Type_ID':105,'Attribute_Type':'KC eCheck Nursery Month'},{'Attribute_ID':9023,'Attribute_Name':'3-4','Sort_Order':3,'Attribute_Type_ID':105,'Attribute_Type':'KC eCheck Nursery Month'},{'Attribute_ID':9024,'Attribute_Name':'4-5','Sort_Order':4,'Attribute_Type_ID':105,'Attribute_Type':'KC eCheck Nursery Month'},{'Attribute_ID':9025,'Attribute_Name':'5-6','Sort_Order':5,'Attribute_Type_ID':105,'Attribute_Type':'KC eCheck Nursery Month'},{'Attribute_ID':9026,'Attribute_Name':'6-7','Sort_Order':6,'Attribute_Type_ID':105,'Attribute_Type':'KC eCheck Nursery Month'},{'Attribute_ID':9027,'Attribute_Name':'7-8','Sort_Order':7,'Attribute_Type_ID':105,'Attribute_Type':'KC eCheck Nursery Month'},{'Attribute_ID':9028,'Attribute_Name':'8-9','Sort_Order':8,'Attribute_Type_ID':105,'Attribute_Type':'KC eCheck Nursery Month'},{'Attribute_ID':9029,'Attribute_Name':'9-10','Sort_Order':9,'Attribute_Type_ID':105,'Attribute_Type':'KC eCheck Nursery Month'},{'Attribute_ID':9030,'Attribute_Name':'10-11','Sort_Order':10,'Attribute_Type_ID':105,'Attribute_Type':'KC eCheck Nursery Month'},{'Attribute_ID':9031,'Attribute_Name':'11-12','Sort_Order':11,'Attribute_Type_ID':105,'Attribute_Type':'KC eCheck Nursery Month'}]");


            _fixture = new RoomService(_eventRepository.Object, _roomRepository.Object, _attributeRepository.Object, _groupRepository.Object, _applicationConfiguration.Object, _apiUserRepository.Object);
        }

        [Test]
        public void ShouldGetEventRooms()
        {
            var token = "abcdefg";

            // Arrange
            var mpEventDto = new MpEventDto
            {
                EventId = 1234567,
                LocationId = 3
            };
            var events = new List<MpEventDto> {mpEventDto};

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

            string eventRoomJson = @"{  'Allow_Checkin': true, 'Capacity': 50, 'Checked_In': 0, 'Event_ID': 4534870, 'Event_Room_ID': 4601093,
                'Room_ID': 1972, 'Room_Name': 'Special Needs (K105)', 'Signed_In': 1, 'Volunteers': 0 }";
            JObject eventRoomResult = JObject.Parse(eventRoomJson);
            List<JObject> eventRoomJsonList = new List<JObject>
            {
                eventRoomResult
            };

            string eventGroupJson = @"{'Event_ID': 4534870, 'Group_ID': 173934, 'Event_Group_ID': 1895918, 'Event_Room_ID': 4601093, 'Room_ID': 1972, 'Signed_In': 0, 'Checked_In': 0}";
            JObject eventGroupResult = JObject.Parse(eventGroupJson);
            List<JObject> eventGroupJsonList = new List<JObject>
            {
                eventGroupResult
            };

            string mpGroupNurseryMonthAttributeJson = @"{'Group_ID': 173934, 'Attribute_Type_ID': 105, 'Attribute_ID': 9031, 'Attribute_Name': '11-12', 'Sort_Order': 11}";
            JObject mpGroupNurseryMonthAttributeResult = JObject.Parse(mpGroupNurseryMonthAttributeJson);
            string mpGroupBirthMonthAttributeJson = @"{'Group_ID': 173934, 'Attribute_Type_ID': 103, 'Attribute_ID': 9030, 'Attribute_Name': 'January', 'Sort_Order': 11}";
            JObject mpGroupBirthMonthAttributeResult = JObject.Parse(mpGroupBirthMonthAttributeJson);
            string mpGroupAgeAttributeJson = @"{'Group_ID': 173934, 'Attribute_Type_ID': 102, 'Attribute_ID': 9029, 'Attribute_Name': 'Nursery Age', 'Sort_Order': 11}";
            JObject mpGroupAgeAttributeResult = JObject.Parse(mpGroupAgeAttributeJson);
            List<JObject> mpGroupAttributeJsonList = new List<JObject>
            {
                mpGroupNurseryMonthAttributeResult,
                mpGroupBirthMonthAttributeResult,
                mpGroupAgeAttributeResult,
            };

            string mpNurseryMonthAttributeJson = @"{'Attribute_ID': 9031, 'Attribute_Name': 'January', 'Description': 'January birth month', 'Attribute_Type_ID': 105, 'Attribute_Type_Name': 'Nursery Month for Kids Club eCheck group' }";
            JObject mpNurseryMonthAttributeResult = JObject.Parse(mpNurseryMonthAttributeJson);
            string mpBirthMonthAttributeJson = @"{'Attribute_ID': 9030, 'Attribute_Name': 'January', 'Description': 'January birth month', 'Attribute_Type_ID': 103, 'Attribute_Type_Name': 'Birth Month for Kids Club eCheck group' }";
            JObject mpBirthMonthAttributeResult = JObject.Parse(mpBirthMonthAttributeJson);
            string mpAgeAttributeJson = @"{'Attribute_ID': 9029, 'Attribute_Name': 'January', 'Description': 'January birth month', 'Attribute_Type_ID': 102, 'Attribute_Type_Name': 'Age for Kids Club eCheck group' }";
            JObject mpAttributeResult = JObject.Parse(mpAgeAttributeJson);
            List<JObject> mpAgeAttributeJsonList = new List<JObject>
            {
                mpNurseryMonthAttributeResult,
                mpBirthMonthAttributeResult,
                mpAttributeResult
            };

            var roomJsonData = new List<List<JObject>>
            {
                eventRoomJsonList,
                eventGroupJsonList,
                mpGroupAttributeJsonList,
                mpAgeAttributeJsonList
            };

            _roomRepository.Setup(m => m.GetManageRoomsListData(mpEventDto.EventId)).Returns(roomJsonData);

            // Act
            var result = _fixture.GetLocationRoomsByEventId(token, 1234567);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result[0].EventRoomId, 4601093);
            Assert.AreEqual(result[0].AssignedGroups.Count, 1);
            _roomRepository.VerifyAll();
            _eventRepository.VerifyAll();
        }
        
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
        
        public void TestCreateOrUpdateEventRoomAndSubEventRoom()
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

            var acEventRoom = new MpEventRoomDto
            {
                AllowSignIn = true,
                Capacity = 1,
                CheckedIn = 2,
                EventId = 32312,
                EventRoomId = 93299,
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

            var acEvents = new List<MpEventDto>
            {
                new MpEventDto
                {
                    EventId = 32312,
                    ParentEventId = 3
                }
            };

            var newEventRoom = Mapper.Map<EventRoomDto>(newMpEventRoom);

            _eventRepository.Setup(mocked => mocked.GetSubeventsForEvents(It.IsAny<List<int>>(), It.IsAny<int>())).Returns(acEvents);
            _roomRepository.Setup(mocked => mocked.GetEventRoom(It.IsAny<int>(), It.IsAny<int>())).Returns(acEventRoom);
            _roomRepository.Setup(mocked => mocked.CreateOrUpdateEventRoom("token", It.IsAny<MpEventRoomDto>())).Returns(newMpEventRoom);
            var result = _fixture.CreateOrUpdateEventRoom("token", eventRoom);
            _roomRepository.VerifyAll();

            Assert.IsNotNull(result);
            result.ShouldBeEquivalentTo(newEventRoom);
        }

        public void TestGetEventRoomAgesAndGradesNoEventGroups()
        {
            const string token = "token 123";
            const int eventId = 12345;
            const int roomId = 67890;
            _attributeRepository.Setup(mocked => mocked.GetAttributesByAttributeTypeId(AgesAttributeTypeId, token)).Returns(_ageList.OrderBy(x => x.SortOrder).ToList());
            _attributeRepository.Setup(mocked => mocked.GetAttributesByAttributeTypeId(BirthMonthsAttributeTypeId, token))
                .Returns(_birthMonthList.OrderBy(x => x.SortOrder).ToList());
            _attributeRepository.Setup(mocked => mocked.GetAttributesByAttributeTypeId(GradesAttributeTypeId, token)).Returns(_gradeList.OrderBy(x => x.SortOrder).ToList());
            _attributeRepository.Setup(mocked => mocked.GetAttributesByAttributeTypeId(NurseryAgesAttributeTypeId, token))
                .Returns(_nurseryMonthList.OrderBy(x => x.SortOrder).ToList());

            _applicationConfiguration.Setup(m => m.AdventureClubEventTypeId).Returns(20);

            MpEventDto mpEventDto = new MpEventDto
            {
                EventId = 12345,
                EventTypeId = 11
            };

            var checkinEvents = new List<MpEventDto>();
            checkinEvents.Add(mpEventDto);
            _eventRepository.Setup(m => m.GetEventAndCheckinSubevents(It.IsAny<string>(), It.IsAny<int>())).Returns(checkinEvents);

            _eventRepository.Setup(m => m.GetEventById(12345)).Returns(mpEventDto);

            var eventRoom = new MpEventRoomDto
            {
                RoomName = "the room"
            };

            List<int> eventIds = new List<int>
            {
                12345
            };
            _roomRepository.Setup(mocked => mocked.GetEventRoomForEventMaps(eventIds, roomId)).Returns(eventRoom);

            var mpRoomDto = new MpRoomDto
            {
                RoomId = 67890,
                RoomName = "the room"
            };

            _roomRepository.Setup(m => m.GetRoom(It.IsAny<int>())).Returns(mpRoomDto);

            _eventRepository.Setup(mocked => mocked.GetEventGroupsForEvent(eventId)).Returns((List<MpEventGroupDto>) null);

            var result = _fixture.GetEventRoomAgesAndGrades(token, eventId, roomId);
            result.Should().NotBeNull();
            result.RoomName.Should().Be("the room");
            var assignedGroups = result.AssignedGroups;
            assignedGroups.Should().NotBeNull();
            assignedGroups.Count.Should().Be(_ageList.Count + _gradeList.Count);
            Assert.IsFalse(assignedGroups.Exists(x => x.Selected));
            assignedGroups.ForEach(x =>
            {
                Assert.IsFalse(x.HasRanges && x.Ranges.Exists(y => y.Selected));
                if (x.Id == NurseryAgeAttributeId)
                {
                    var i = 0;
                    Assert.IsTrue(x.HasRanges);
                    _nurseryMonthList.OrderBy(m => m.SortOrder).ToList().ForEach(m =>
                    {
                        Assert.AreEqual(m.Name, x.Ranges[i].Name);
                        Assert.AreEqual(m.Id, x.Ranges[i].Id);
                        Assert.AreEqual(m.SortOrder, x.Ranges[i].SortOrder);
                        i++;
                    });
                }
                else if (x.TypeId == AgesAttributeTypeId)
                {
                    var i = 0;
                    Assert.IsTrue(x.HasRanges);
                    _birthMonthList.OrderBy(m => m.SortOrder).ToList().ForEach(m =>
                    {
                        Assert.AreEqual(m.Name.Substring(0, 3), x.Ranges[i].Name);
                        Assert.AreEqual(m.Id, x.Ranges[i].Id);
                        Assert.AreEqual(m.SortOrder, x.Ranges[i].SortOrder);
                        i++;
                    });
                }
                else if (x.TypeId == GradesAttributeTypeId)
                {
                    Assert.IsFalse(x.HasRanges);
                }
                else
                {
                    Assert.Fail($"Unexpected age/grade Id: {x.Id}, Name: {x.Name}");
                }
            });
        }
        
        public void TestGetEventRoomAgesAndGradesWithNurseryEventGroups()
        {
            const string token = "token 123";
            const int eventId = 12345;
            const int roomId = 67890;
            _attributeRepository.Setup(mocked => mocked.GetAttributesByAttributeTypeId(AgesAttributeTypeId, token)).Returns(_ageList.OrderBy(x => x.SortOrder).ToList());
            _attributeRepository.Setup(mocked => mocked.GetAttributesByAttributeTypeId(BirthMonthsAttributeTypeId, token))
                .Returns(_birthMonthList.OrderBy(x => x.SortOrder).ToList());
            _attributeRepository.Setup(mocked => mocked.GetAttributesByAttributeTypeId(GradesAttributeTypeId, token)).Returns(_gradeList.OrderBy(x => x.SortOrder).ToList());
            _attributeRepository.Setup(mocked => mocked.GetAttributesByAttributeTypeId(NurseryAgesAttributeTypeId, token))
                .Returns(_nurseryMonthList.OrderBy(x => x.SortOrder).ToList());

            _applicationConfiguration.Setup(m => m.AdventureClubEventTypeId).Returns(20);

            MpEventDto mpEventDto = new MpEventDto
            {
                EventId = 12345,
                EventTypeId = 11
            };

            var checkinEvents = new List<MpEventDto>();
            checkinEvents.Add(mpEventDto);
            //_eventRepository.Setup(m => m.GetEventAndCheckinSubevents(It.IsAny<string>(), It.IsAny<int>())).Returns(checkinEvents);

            _eventRepository.Setup(m => m.GetEventById(12345)).Returns(mpEventDto);

            var events = new List<MpEventGroupDto>
            {
                new MpEventGroupDto
                {
                    Event = new MpEventDto
                    {
                        EventId = eventId
                    },
                    Group = new MpGroupDto
                    {
                        Id = 98765,
                        AgeRange = new MpAttributeDto
                        {
                            Id = NurseryAgeAttributeId,
                            Type = new MpAttributeTypeDto
                            {
                                Id = AgesAttributeTypeId
                            }
                        },
                        NurseryMonth = _nurseryMonthList[0]
                    },
                    RoomReservation = new MpEventRoomDto
                    {
                        RoomId = roomId
                    }
                }
            };

            _eventRepository.Setup(m => m.GetEventAndCheckinSubevents(It.IsAny<string>(), It.IsAny<int>())).Returns(checkinEvents);

            _eventRepository.Setup(mocked => mocked.GetEventGroupsForEvent(eventId)).Returns(events);
            _groupRepository.Setup(mocked => mocked.GetGroups(token, It.IsAny<IEnumerable<int>>(), true)).Returns(new List<MpGroupDto>
            {
                events[0].Group
            });

            var eventRoom = new MpEventRoomDto
            {
                RoomName = "the room"
            };

            List<int> eventIds = new List<int>
            {
                12345
            };
            _roomRepository.Setup(mocked => mocked.GetEventRoomForEventMaps(eventIds, roomId)).Returns(eventRoom);

            var mpRoomDto = new MpRoomDto
            {
                RoomId = 67890,
                RoomName = "the room"
            };

            _roomRepository.Setup(m => m.GetRoom(It.IsAny<int>())).Returns(mpRoomDto);

            var result = _fixture.GetEventRoomAgesAndGrades(token, eventId, roomId);
            result.Should().NotBeNull();
            result.RoomName.Should().Be("the room");
            var assignedGroups = result.AssignedGroups;
            assignedGroups.Should().NotBeNull();

            _groupRepository.Verify(mocked => mocked.GetGroups(token, It.Is<IEnumerable<int>>(x => x.First() == events[0].Group.Id), true));
            assignedGroups.Should().NotBeNull();
            assignedGroups.Count.Should().Be(_ageList.Count + _gradeList.Count);
            Assert.IsFalse(assignedGroups.Exists(x => x.Selected));
            Assert.IsTrue(assignedGroups.Exists(x => x.Id == NurseryAgeAttributeId && x.HasRanges && x.Ranges.Exists(y => y.Selected && y.Id == _nurseryMonthList[0].Id)));
            Assert.IsFalse(assignedGroups.Exists(x => x.Id != NurseryAgeAttributeId && x.HasRanges && x.Ranges.Exists(y => y.Selected)));
        }


        public void TestGetEventRoomAgesAndGradesWithAgeEventGroups()
        {
            const string token = "token 123";
            const int eventId = 12345;
            const int roomId = 67890;
            _attributeRepository.Setup(mocked => mocked.GetAttributesByAttributeTypeId(AgesAttributeTypeId, token)).Returns(_ageList.OrderBy(x => x.SortOrder).ToList());
            _attributeRepository.Setup(mocked => mocked.GetAttributesByAttributeTypeId(BirthMonthsAttributeTypeId, token))
                .Returns(_birthMonthList.OrderBy(x => x.SortOrder).ToList());
            _attributeRepository.Setup(mocked => mocked.GetAttributesByAttributeTypeId(GradesAttributeTypeId, token)).Returns(_gradeList.OrderBy(x => x.SortOrder).ToList());
            _attributeRepository.Setup(mocked => mocked.GetAttributesByAttributeTypeId(NurseryAgesAttributeTypeId, token))
                .Returns(_nurseryMonthList.OrderBy(x => x.SortOrder).ToList());

            MpEventDto mpEventDto = new MpEventDto
            {
                EventId = 12345,
                EventTypeId = 11
            };

            var checkinEvents = new List<MpEventDto>();
            checkinEvents.Add(mpEventDto);

            _eventRepository.Setup(m => m.GetEventAndCheckinSubevents(It.IsAny<string>(), It.IsAny<int>())).Returns(checkinEvents);

            _eventRepository.Setup(m => m.GetEventById(12345)).Returns(mpEventDto);

            var events = new List<MpEventGroupDto>
            {
                new MpEventGroupDto
                {
                    Event = new MpEventDto
                    {
                        EventId = eventId
                    },
                    Group = new MpGroupDto
                    {
                        Id = 98765,
                        AgeRange = new MpAttributeDto
                        {
                            Id = NurseryAgeAttributeId + 1,
                            Type = new MpAttributeTypeDto
                            {
                                Id = AgesAttributeTypeId
                            }
                        },
                        BirthMonth = _birthMonthList[0]
                    },
                    RoomReservation = new MpEventRoomDto
                    {
                        RoomId = roomId
                    }
                }
            };

            _eventRepository.Setup(mocked => mocked.GetEventGroupsForEvent(eventId)).Returns(events);
            _groupRepository.Setup(mocked => mocked.GetGroups(token, It.IsAny<IEnumerable<int>>(), true)).Returns(new List<MpGroupDto>
            {
                events[0].Group
            });

            var eventRoom = new MpEventRoomDto
            {
                RoomName = "the room"
            };
 
            List<int> eventIds = new List<int>
            {
                12345
            };
            _roomRepository.Setup(mocked => mocked.GetEventRoomForEventMaps(eventIds, roomId)).Returns(eventRoom);

            var mpRoomDto = new MpRoomDto
            {
                RoomId = 67890,
                RoomName = "the room"
            };

            _roomRepository.Setup(m => m.GetRoom(It.IsAny<int>())).Returns(mpRoomDto);

            var result = _fixture.GetEventRoomAgesAndGrades(token, eventId, roomId);
            _groupRepository.Verify(mocked => mocked.GetGroups(token, It.Is<IEnumerable<int>>(x => x.First() == events[0].Group.Id), true));
            result.Should().NotBeNull();
            result.RoomName.Should().Be("the room");
            var assignedGroups = result.AssignedGroups;
            assignedGroups.Should().NotBeNull();

            assignedGroups.Count.Should().Be(_ageList.Count + _gradeList.Count);
            Assert.IsFalse(assignedGroups.Exists(x => x.Selected));
            Assert.IsTrue(assignedGroups.Exists(x => x.Id == NurseryAgeAttributeId + 1 && x.HasRanges && x.Ranges.Exists(y => y.Selected && y.Id == _birthMonthList[0].Id)));
            Assert.IsFalse(assignedGroups.Exists(x => x.Id != NurseryAgeAttributeId + 1 && x.HasRanges && x.Ranges.Exists(y => y.Selected)));
        }
        
        public void TestGetEventRoomAgesAndGradesWithGradeEventGroups()
        {
            const string token = "token 123";
            const int eventId = 12345;
            const int roomId = 67890;
            _attributeRepository.Setup(mocked => mocked.GetAttributesByAttributeTypeId(AgesAttributeTypeId, token)).Returns(_ageList.OrderBy(x => x.SortOrder).ToList());
            _attributeRepository.Setup(mocked => mocked.GetAttributesByAttributeTypeId(BirthMonthsAttributeTypeId, token))
                .Returns(_birthMonthList.OrderBy(x => x.SortOrder).ToList());
            _attributeRepository.Setup(mocked => mocked.GetAttributesByAttributeTypeId(GradesAttributeTypeId, token)).Returns(_gradeList.OrderBy(x => x.SortOrder).ToList());
            _attributeRepository.Setup(mocked => mocked.GetAttributesByAttributeTypeId(NurseryAgesAttributeTypeId, token))
                .Returns(_nurseryMonthList.OrderBy(x => x.SortOrder).ToList());

            _eventRepository.Setup(m => m.GetEventAndCheckinSubevents(It.IsAny<string>(), It.IsAny<int>())).Returns(new List<MpEventDto>());

            MpEventDto mpEventDto = new MpEventDto
            {
                EventId = 12345,
                EventTypeId = 11
            };

            _eventRepository.Setup(m => m.GetEventById(12345)).Returns(mpEventDto);

            var checkinEvents = new List<MpEventDto>();
            checkinEvents.Add(mpEventDto);

            _eventRepository.Setup(m => m.GetEventAndCheckinSubevents(It.IsAny<string>(), It.IsAny<int>())).Returns(checkinEvents);

            var events = new List<MpEventGroupDto>
            {
                new MpEventGroupDto
                {
                    Event = new MpEventDto
                    {
                        EventId = eventId
                    },
                    Group = new MpGroupDto
                    {
                        Id = 98765,
                        Grade = new MpAttributeDto
                        {
                            Id = _gradeList[0].Id,
                            Type = new MpAttributeTypeDto
                            {
                                Id = GradesAttributeTypeId
                            }
                        },
                    },
                    RoomReservation = new MpEventRoomDto
                    {
                        RoomId = roomId
                    }
                }
            };

            _eventRepository.Setup(mocked => mocked.GetEventGroupsForEvent(eventId)).Returns(events);
            _groupRepository.Setup(mocked => mocked.GetGroups(token, It.IsAny<IEnumerable<int>>(), true)).Returns(new List<MpGroupDto>
            {
                events[0].Group
            });

            var eventRoom = new MpEventRoomDto
            {
                RoomName = "the room"
            };

            List<int> eventIds = new List<int>
            {
                12345
            };
            _roomRepository.Setup(mocked => mocked.GetEventRoomForEventMaps(eventIds, roomId)).Returns(eventRoom);

            var mpRoomDto = new MpRoomDto
            {
                RoomId = 67890,
                RoomName = "the room"
            };

            _roomRepository.Setup(m => m.GetRoom(It.IsAny<int>())).Returns(mpRoomDto);

            var result = _fixture.GetEventRoomAgesAndGrades(token, eventId, roomId);
            _groupRepository.Verify(mocked => mocked.GetGroups(token, It.Is<IEnumerable<int>>(x => x.First() == events[0].Group.Id), true));
            result.Should().NotBeNull();
            result.RoomName.Should().Be("the room");
            var assignedGroups = result.AssignedGroups;
            assignedGroups.Should().NotBeNull();
            assignedGroups.Count.Should().Be(_ageList.Count + _gradeList.Count);
            var selected = assignedGroups.FindAll(x => x.Selected);
            Assert.IsTrue(selected.Count == 1);
            Assert.AreEqual(_gradeList[0].Id, selected[0].Id);
            Assert.IsFalse(assignedGroups.Exists(x => x.Id != _gradeList[0].Id && x.HasRanges && x.Ranges.Exists(y => y.Selected)));
        }

        [Test]
        public void ShouldGetAvailableRooms()
        {
            // Arrange
            var token = "abcdefg";
            var eventId = 1000;

            var mpEventDto = new MpEventDto
            {
                EventId = eventId,
                LocationId = 1
            };

            var checkinEvents = new List<MpEventDto>();
            checkinEvents.Add(mpEventDto);

            var mpEventRoomFrom = new MpEventRoomDto
            {
                AllowSignIn = true,
                Capacity = 1,
                CheckedIn = 2,
                EventId = eventId,
                EventRoomId = 1000,
                RoomId = 1111,
                RoomName = "name",
                RoomNumber = "number",
                SignedIn = 5,
                Volunteers = 6
            };

            var mpEventRoom = new MpEventRoomDto
            {
                AllowSignIn = true,
                Capacity = 1,
                CheckedIn = 2,
                EventId = eventId,
                EventRoomId = 1111,
                RoomId = 1112,
                RoomName = "name",
                RoomNumber = "number",
                SignedIn = 5,
                Volunteers = 6
            };

            List<MpEventRoomDto> mpEventRoomDtos = new List<MpEventRoomDto>();
            mpEventRoomDtos.Add(mpEventRoom);
            mpEventRoomDtos.Add(mpEventRoomFrom);

            var mpBumpingRuleDtos = new List<MpBumpingRuleDto>
            {
                new MpBumpingRuleDto
                {
                    BumpingRuleId = 2345678,
                    ToEventRoomId = 1111,
                    PriorityOrder = 1
                }
            };

            _eventRepository.Setup(m => m.GetEventAndCheckinSubevents(token, eventId)).Returns(checkinEvents);
            _roomRepository.Setup(m => m.GetEventRoomForEventMaps(It.IsAny<List<int>>(), 1111)).Returns(mpEventRoomFrom);
            _roomRepository.Setup(m => m.GetRoomsForEvent(mpEventDto.EventId, mpEventDto.LocationId)).Returns(mpEventRoomDtos);
            _roomRepository.Setup(m => m.GetBumpingRulesForEventRooms(It.IsAny<List<int?>>(), It.IsAny<int?>())).Returns(mpBumpingRuleDtos);

            List<EventRoomDto> verifiedDtos = new List<EventRoomDto>()
            {
                new EventRoomDto
                {
                    BumpingRuleId = 2345678,
                    BumpingRulePriority = 1
                }
            };

            // Act
            var result = _fixture.GetAvailableRooms(token, 1111, eventId);

            // Assert
            _eventRepository.VerifyAll();
            _roomRepository.VerifyAll();

            Assert.AreEqual(verifiedDtos[0].BumpingRuleId, result[0].BumpingRuleId);
            Assert.AreEqual(verifiedDtos[0].BumpingRulePriority, result[0].BumpingRulePriority);
        }

        [Test]
        public void ShouldGetAvailableRoomsForRoomWithNoEventRoom()
        {
            // Arrange
            var token = "abcdefg";
            var eventId = 1234567;

            var mpEventDto = new MpEventDto
            {
                EventId = eventId,
                LocationId = 1
            };

            var checkinEvents = new List<MpEventDto>();
            checkinEvents.Add(mpEventDto);

            var mpEventRoomFrom = new MpEventRoomDto
            {
                AllowSignIn = true,
                Capacity = 1,
                CheckedIn = 2,
                EventId = 3,
                RoomId = 1111,
                RoomName = "name",
                RoomNumber = "number",
                SignedIn = 5,
                Volunteers = 6
            };

            var mpEventRoom = new MpEventRoomDto
            {
                AllowSignIn = true,
                Capacity = 1,
                CheckedIn = 2,
                EventId = 3,
                EventRoomId = 1111,
                RoomId = 1112,
                RoomName = "name",
                RoomNumber = "number",
                SignedIn = 5,
                Volunteers = 6
            };

            List<MpEventRoomDto> mpEventRoomDtos = new List<MpEventRoomDto>();
            mpEventRoomDtos.Add(mpEventRoom);
            mpEventRoomDtos.Add(mpEventRoomFrom);

            var mpBumpingRuleDtos = new List<MpBumpingRuleDto>
            {
                new MpBumpingRuleDto
                {
                    BumpingRuleId = 2345678,
                    ToEventRoomId = 1111,
                    PriorityOrder = 1
                }
            };

            var returnedNewEventRoom = new MpEventRoomDto
            {
                EventRoomId = 2345678,
                RoomId = 1111,
                EventId = 3,
                RoomName = "name"
            };

            _eventRepository.Setup(m => m.GetEventAndCheckinSubevents(token, eventId)).Returns(checkinEvents);
            _roomRepository.Setup(m => m.GetEventRoomForEventMaps(It.IsAny<List<int>>(), 1111)).Returns((MpEventRoomDto) null);
            _roomRepository.Setup(m => m.GetRoomsForEvent(mpEventDto.EventId, mpEventDto.LocationId)).Returns(mpEventRoomDtos);
            _roomRepository.Setup(m => m.GetBumpingRulesForEventRooms(It.IsAny<List<int?>>(), It.IsAny<int?>())).Returns(mpBumpingRuleDtos);
            _roomRepository.Setup(m => m.CreateOrUpdateEventRoom(null, It.IsAny<MpEventRoomDto>())).Returns(returnedNewEventRoom);

            List<EventRoomDto> verifiedDtos = new List<EventRoomDto>()
            {
                new EventRoomDto
                {
                    BumpingRuleId = 2345678,
                    BumpingRulePriority = 1
                }
            };

            // Act
            var result = _fixture.GetAvailableRooms(token, 1111, 1234567);

            // Assert
            _eventRepository.VerifyAll();
            _roomRepository.VerifyAll();

            Assert.AreEqual(verifiedDtos[0].BumpingRuleId, result[0].BumpingRuleId);
            Assert.AreEqual(verifiedDtos[0].BumpingRulePriority, result[0].BumpingRulePriority);
        }

        [Test]
        public void ShouldUpdateAvailableRooms()
        {
            // Arrange
            var token = "abcdefg";
            var eventId = 1234567;
            var roomId = 1111;

            MpEventDto mpEventDto = new MpEventDto
            {
                EventId = eventId,
                LocationId = 1
            };

            var checkinEvents = new List<MpEventDto>();
            checkinEvents.Add(mpEventDto);

            var mpEventRoomFrom = new MpEventRoomDto
            {
                AllowSignIn = true,
                Capacity = 1,
                CheckedIn = 2,
                EventId = eventId,
                EventRoomId = 1000,
                RoomId = 1111,
                RoomName = "name",
                RoomNumber = "number",
                SignedIn = 5,
                Volunteers = 6
            };


            var mpEventRoom = new MpEventRoomDto
            {
                AllowSignIn = true,
                Capacity = 1,
                CheckedIn = 2,
                EventId = eventId,
                EventRoomId = 1111,
                RoomId = 1112,
                RoomName = "name",
                RoomNumber = "number",
                SignedIn = 5,
                Volunteers = 6
            };

            List<MpEventRoomDto> mpEventRoomDtos = new List<MpEventRoomDto>();
            mpEventRoomDtos.Add(mpEventRoom);
            mpEventRoomDtos.Add(mpEventRoomFrom);

            var mpBumpingRuleDtos = new List<MpBumpingRuleDto>
            {
                new MpBumpingRuleDto
                {
                    BumpingRuleId = 2345678,
                    ToEventRoomId = 1111,
                    PriorityOrder = 1
                }
            };

            List<int?> roomIds = new List<int?>
            {
                111,
                222,
                333
            };

            List<EventRoomDto> eventRoomDtos = new List<EventRoomDto>
            {
                new EventRoomDto
                {
                    EventRoomId = 7891234,
                    EventId = 1234567
                }
            };



            _eventRepository.Setup(m => m.GetEventAndCheckinSubevents(token, eventId)).Returns(checkinEvents);
            _roomRepository.Setup(m => m.GetEventRoomForEventMaps(It.IsAny<List<int>>(), 1111)).Returns((MpEventRoomDto)null);
            _roomRepository.Setup(m => m.CreateOrUpdateEventRoom(null, It.IsAny<MpEventRoomDto>())).Returns(mpEventRoomFrom);
            _roomRepository.Setup(m => m.GetEventRoom(eventId, roomId)).Returns(mpEventRoomFrom);
            _roomRepository.Setup(m => m.GetRoomsForEvent(mpEventDto.EventId, mpEventDto.LocationId)).Returns(mpEventRoomDtos);
            _roomRepository.Setup(m => m.GetBumpingRulesByRoomId(1000)).Returns(mpBumpingRuleDtos);
            _roomRepository.Setup(m => m.GetBumpingRulesForEventRooms(It.IsAny<List<int?>>(), It.IsAny<int?>())).Returns(mpBumpingRuleDtos);
            _roomRepository.Setup(m => m.DeleteBumpingRules(It.IsAny<string>(), It.IsAny<IEnumerable<int>>()));

            // Act
            _fixture.UpdateAvailableRooms(token, eventId, roomId, eventRoomDtos);

            // Assert
            _eventRepository.VerifyAll();
            _roomRepository.VerifyAll();
        }

        [Test]
        public void TestGetGradeAttributes()
        {
            const string token = "token 123";
            _attributeRepository.Setup(mocked => mocked.GetAttributesByAttributeTypeId(GradesAttributeTypeId, token)).Returns(_gradeList.OrderBy(x => x.SortOrder).ToList());
            
            var result = _fixture.GetGradeAttributes(token);
            result.Should().NotBeNull();
            Assert.IsTrue(result.Count == 8);
            Assert.AreEqual(_gradeList[1].Id, result[0].Id);
        }
    }
}
