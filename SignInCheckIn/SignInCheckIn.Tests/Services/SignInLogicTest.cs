using System;
using System.Collections.Generic;
using System.Linq;
using Crossroads.Utilities.Services.Interfaces;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using NUnit.Framework;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Services;

namespace SignInCheckIn.Tests.Services
{
    public class SignInLogicTest
    {
        private Mock<IEventRepository> _eventRepository;
        private Mock<IRoomRepository> _roomRepository;
        private Mock<IAttributeRepository> _attributeRepository;
        private Mock<IGroupRepository> _groupRepository;
        private Mock<IApplicationConfiguration> _applicationConfiguration;
        private Mock<IApiUserRepository> _apiUserRepository;
        private Mock<IConfigRepository> _configRepository;
        private Mock<IParticipantRepository> _participantRepository;
        private Mock<IChildSigninRepository> _childSigninRepository;

        private const int AgesAttributeTypeId = 102;
        private const int BirthMonthsAttributeTypeId = 103;
        private const int GradesAttributeTypeId = 104;
        private const int NurseryAgeAttributeId = 9014;
        private const int NurseryAgesAttributeTypeId = 105;

        private List<MpAttributeDto> _ageList;
        private List<MpAttributeDto> _gradeList;
        private List<MpAttributeDto> _birthMonthList;
        private List<MpAttributeDto> _nurseryMonthList;

        private SignInLogic _fixture;

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
            _configRepository = new Mock<IConfigRepository>();
            _participantRepository = new Mock<IParticipantRepository>();
            _childSigninRepository = new Mock<IChildSigninRepository>();
            _applicationConfiguration.SetupGet(mocked => mocked.AgesAttributeTypeId).Returns(AgesAttributeTypeId);
            _applicationConfiguration.SetupGet(mocked => mocked.BirthMonthsAttributeTypeId).Returns(BirthMonthsAttributeTypeId);
            _applicationConfiguration.SetupGet(mocked => mocked.GradesAttributeTypeId).Returns(GradesAttributeTypeId);
            _applicationConfiguration.SetupGet(mocked => mocked.NurseryAgeAttributeId).Returns(NurseryAgeAttributeId);
            _applicationConfiguration.SetupGet(mocked => mocked.NurseryAgesAttributeTypeId).Returns(NurseryAgesAttributeTypeId);

            MpConfigDto earlyCheckInPeriodConfig = new MpConfigDto
            {
                ApplicationCode = "CHECKIN",
                KeyName = "DefaultEarlyCheckIn",
                ConfigurationSettingId = 1,
                Value = "60"
            };

            _configRepository.Setup(m => m.GetMpConfigByKey("DefaultEarlyCheckIn")).Returns(earlyCheckInPeriodConfig);

            MpConfigDto lateCheckInPeriodConfig = new MpConfigDto
            {
                ApplicationCode = "CHECKIN",
                KeyName = "DefaultLateCheckIn",
                ConfigurationSettingId = 1,
                Value = "30"
            };

            _configRepository.Setup(m => m.GetMpConfigByKey("DefaultLateCheckIn")).Returns(lateCheckInPeriodConfig);

            _fixture = new SignInLogic(_eventRepository.Object, _applicationConfiguration.Object, _configRepository.Object,
                _groupRepository.Object, _roomRepository.Object, _participantRepository.Object, _childSigninRepository.Object);
        }

        [Test]
        public void ShouldGetSingleEventForSignIn()
        {
            // Arrange
            int siteId = 1;
            bool underThreeSignIn = false;
            bool adventureClubSignIn = false;

            var signInTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 00, 00);

            _eventRepository.Setup(r => r.GetEvents(signInTime, signInTime, 1, true)).Returns(GetTestEventSet());

            // Act
            var result = _fixture.EvaluateSignInEvents(siteId, underThreeSignIn, adventureClubSignIn, GetTestEventSet());

            // Assert
            Assert.AreEqual(result[0].EventId, 1234567);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ShouldGetTwoAdventureClubEventsForSignIn()
        {
            // Arrange
            int siteId = 1;
            bool underThreeSignIn = false;
            bool adventureClubSignIn = true;

            var signInTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 00, 00);

            _eventRepository.Setup(r => r.GetEvents(signInTime, signInTime, 1, true)).Returns(GetTestEventSet());

            // Act
            var result = _fixture.EvaluateSignInEvents(siteId, adventureClubSignIn, underThreeSignIn, GetTestEventSet());

            // Assert
            var serviceEventCount = result.Count(r => r.ParentEventId == null);
            var adventureClubEventCount = result.Count(r => r.ParentEventId != null);
            Assert.AreEqual(2, serviceEventCount);
            Assert.AreEqual(2, adventureClubEventCount);
            
            Assert.IsTrue(result.Any(r => r.EventId == 1234567));
            Assert.IsTrue(result.Any(r => r.EventId == 7654321));
            Assert.IsTrue(result.Any(r => r.EventId == 2345678));
            Assert.IsTrue(result.Any(r => r.EventId == 8765432));

            Assert.AreEqual(4, result.Count);
        }

        [Test]
        public void ShouldGetOneAdventureClubEventForSignInCurrentService()
        {
            // Arrange
            int siteId = 1;
            bool underThreeSignIn = false;
            bool adventureClubSignIn = true;

            // Act
            var result = _fixture.EvaluateSignInEvents(siteId, adventureClubSignIn, underThreeSignIn, GetTestEventSetCurrentEventAc());

            // Assert
            var serviceEventCount = result.Count(r => r.ParentEventId == null);
            var adventureClubEventCount = result.Count(r => r.ParentEventId != null);
            Assert.AreEqual(2, serviceEventCount);
            Assert.AreEqual(1, adventureClubEventCount);

            Assert.IsTrue(result.Any(r => r.EventId == 1234567));
            Assert.IsTrue(result.Any(r => r.EventId == 7654321));
            Assert.IsTrue(result.Any(r => r.EventId == 2345678));

            Assert.AreEqual(3, result.Count);
        }

        [Test]
        public void ShouldGetOneAdventureClubEventForSignInFutureService()
        {
            // Arrange
            int siteId = 1;
            bool underThreeSignIn = false;
            bool adventureClubSignIn = true;

            // Act
            var result = _fixture.EvaluateSignInEvents(siteId, adventureClubSignIn, underThreeSignIn, GetTestEventSetFutureEventAc());

            // Assert
            var serviceEventCount = result.Count(r => r.ParentEventId == null);
            var adventureClubEventCount = result.Count(r => r.ParentEventId != null);
            Assert.AreEqual(2, serviceEventCount);
            Assert.AreEqual(1, adventureClubEventCount);

            Assert.IsTrue(result.Any(r => r.EventId == 1234567));
            Assert.IsTrue(result.Any(r => r.EventId == 2345678));
            Assert.IsTrue(result.Any(r => r.EventId == 8765432));

            Assert.AreEqual(3, result.Count);
        }

        [Test]
        public void ShouldGetTwoServiceEventsForUnderThreeSignIn()
        {
            // Arrange
            int siteId = 1;
            bool underThreeSignIn = true;
            bool adventureClubSignIn = true;

            // Act
            var result = _fixture.EvaluateSignInEvents(siteId, adventureClubSignIn, underThreeSignIn, GetTestEventSet());

            // Assert
            var serviceEventCount = result.Count(r => r.ParentEventId == null);
            var adventureClubEventCount = result.Count(r => r.ParentEventId != null);
            Assert.AreEqual(2, serviceEventCount);
            Assert.AreEqual(0, adventureClubEventCount);

            Assert.IsTrue(result.Any(r => r.EventId == 1234567));
            Assert.IsTrue(result.Any(r => r.EventId == 2345678));

            Assert.AreEqual(2, result.Count);
        }

        private List<MpEventDto> GetTestEventSet()
        {
            // we need to set times dynamically on the test data set, to avoid having unit tests
            // break when run at later or earlier hours - this may still be a trouble spot
            var currentStartTime = System.DateTime.Now;
            var futureStartTime = System.DateTime.Now.AddHours(2);
            var invalidFutureStartTime = System.DateTime.Now.AddHours(4);

            List<MpEventDto> mpEventDtos = new List<MpEventDto>
            {
                new MpEventDto
                {
                    EventId = 1234567,
                    EventStartDate = currentStartTime,
                    EventTitle = "First Non Ac Event",
                    Cancelled = false,
                    ParentEventId = null
                },
                new MpEventDto
                {
                    EventId = 7654321,
                    EventStartDate = currentStartTime,
                    EventTitle = "First Ac Event",
                    Cancelled = false,
                    ParentEventId = 1234567
                },
                new MpEventDto
                {
                    EventId = 2345678,
                    EventStartDate = futureStartTime,
                    EventTitle = "Second Non Ac Event",
                    Cancelled = false,
                    ParentEventId = null
                },
                new MpEventDto
                {
                    EventId = 8765432,
                    EventStartDate = futureStartTime,
                    EventTitle = "Second Ac Event",
                    Cancelled = false,
                    ParentEventId = 2345678
                },
                new MpEventDto
                {
                    EventId = 3456789,
                    EventStartDate = invalidFutureStartTime,
                    EventTitle = "Third Non Ac Event",
                    Cancelled = false,
                    ParentEventId = null
                },
                new MpEventDto
                {
                    EventId = 9876543,
                    EventStartDate = invalidFutureStartTime,
                    EventTitle = "Third Ac Event",
                    Cancelled = false,
                    ParentEventId = 3456789
                }
            };

            return mpEventDtos;
        }

        private List<MpEventDto> GetTestEventSetCurrentEventAc()
        {
            // we need to set times dynamically on the test data set, to avoid having unit tests
            // break when run at later or earlier hours - this may still be a trouble spot
            var currentStartTime = System.DateTime.Now;
            var futureStartTime = System.DateTime.Now.AddHours(2);
            var invalidFutureStartTime = System.DateTime.Now.AddHours(4);

            List<MpEventDto> mpEventDtos = new List<MpEventDto>
            {
                new MpEventDto
                {
                    EventId = 1234567,
                    EventStartDate = currentStartTime,
                    EventTitle = "First Non Ac Event",
                    Cancelled = false,
                    ParentEventId = null
                },
                new MpEventDto
                {
                    EventId = 7654321,
                    EventStartDate = currentStartTime,
                    EventTitle = "First Ac Event",
                    Cancelled = false,
                    ParentEventId = 1234567
                },
                new MpEventDto
                {
                    EventId = 2345678,
                    EventStartDate = futureStartTime,
                    EventTitle = "Second Non Ac Event",
                    Cancelled = false,
                    ParentEventId = null
                },
                new MpEventDto
                {
                    EventId = 3456789,
                    EventStartDate = invalidFutureStartTime,
                    EventTitle = "Third Non Ac Event",
                    Cancelled = false,
                    ParentEventId = null
                },
                new MpEventDto
                {
                    EventId = 9876543,
                    EventStartDate = invalidFutureStartTime,
                    EventTitle = "Third Ac Event",
                    Cancelled = false,
                    ParentEventId = 3456789
                }
            };

            return mpEventDtos;
        }

        private List<MpEventDto> GetTestEventSetFutureEventAc()
        {
            // we need to set times dynamically on the test data set, to avoid having unit tests
            // break when run at later or earlier hours - this may still be a trouble spot
            var currentStartTime = System.DateTime.Now;
            var futureStartTime = System.DateTime.Now.AddHours(2);
            var invalidFutureStartTime = System.DateTime.Now.AddHours(4);

            List<MpEventDto> mpEventDtos = new List<MpEventDto>
            {
                new MpEventDto
                {
                    EventId = 1234567,
                    EventStartDate = currentStartTime,
                    EventTitle = "First Non Ac Event",
                    Cancelled = false,
                    ParentEventId = null
                },
                new MpEventDto
                {
                    EventId = 2345678,
                    EventStartDate = futureStartTime,
                    EventTitle = "Second Non Ac Event",
                    Cancelled = false,
                    ParentEventId = null
                },
                new MpEventDto
                {
                    EventId = 8765432,
                    EventStartDate = futureStartTime,
                    EventTitle = "Second Ac Event",
                    Cancelled = false,
                    ParentEventId = 2345678
                },
                new MpEventDto
                {
                    EventId = 3456789,
                    EventStartDate = invalidFutureStartTime,
                    EventTitle = "Third Non Ac Event",
                    Cancelled = false,
                    ParentEventId = null
                },
                new MpEventDto
                {
                    EventId = 9876543,
                    EventStartDate = invalidFutureStartTime,
                    EventTitle = "Third Ac Event",
                    Cancelled = false,
                    ParentEventId = 3456789
                }
            };

            return mpEventDtos;
        }

        [Test]
        public void GetEventRoomsForSignIn()
        {
            // Arrange
            var groupId = 5544555;
            var eventIds = new List<int>
            {
                1234567,
                7654321
            };

            var eventRoomIds = new List<int>
            {
                1122333,
                3332211
            };

            _eventRepository.Setup(m => m.GetEventGroupsByGroupIdAndEventIds(groupId, eventIds)).Returns(GetEventGroupsData());
            _roomRepository.Setup(m => m.GetEventRoomsByEventRoomIds(eventRoomIds)).Returns(GetEventRoomsData());

            // Act
            var result =_fixture.GetSignInEventRooms(groupId, eventIds);

            // Assert
            Assert.AreEqual(2, result.Count);
        }

        private List<MpEventGroupDto> GetEventGroupsData()
        {
            var eventGroups = new List<MpEventGroupDto>
            {
                new MpEventGroupDto
                {
                    EventId = 1234567,
                    RoomReservationId = 1122333,
                    GroupId = 5544555,
                    RoomId = 1234,
                },
                new MpEventGroupDto
                {
                    EventId = 7654321,
                    RoomReservationId = 3332211,
                    GroupId = 5544555,
                    RoomId = 4321
                }
            };

            return eventGroups;
        }

        private List<MpEventRoomDto> GetEventRoomsData()
        {
            var eventRooms = new List<MpEventRoomDto>
            {
                new MpEventRoomDto
                {
                    Capacity = 10,
                    EventRoomId = 1122333,
                    RoomId = 1234,
                    AllowSignIn = true
                },
                new MpEventRoomDto
                {
                    Capacity = 10,
                    EventRoomId = 3332211,
                    RoomId = 4321,
                    AllowSignIn = true
                },
                new MpEventRoomDto
                {
                    Capacity = 10,
                    EventRoomId = 677788,
                    RoomId = 8888,
                    AllowSignIn = false
                }
            };

            return eventRooms;
        }

        private ParticipantEventMapDto GetEventParticipantMapForAuditNoParticipantGroup()
        {
            var kioskEvent = new EventDto
            {
                EventId = 1234567
            };

            List<ParticipantDto> participantDtos = new List<ParticipantDto>
            {
                new ParticipantDto
                {
                    DateOfBirth = new DateTime(2010, 01, 01),
                    GroupId = null,
                    LastName = "UnassignedGroup",
                    Nickname = "Joe",
                    ParticipantId = 9876789
                },
                new ParticipantDto
                {
                    DateOfBirth = new DateTime(2010, 01, 01),
                    GroupId = 7766777,
                    LastName = "NoOpenRoom",
                    Nickname = "Jane",
                    ParticipantId = 8765678
                }
            };

            var participantEventMapDto = new ParticipantEventMapDto
            {
                CurrentEvent = kioskEvent,
                Participants = participantDtos
            };
            
            return participantEventMapDto;
        }

        private ParticipantEventMapDto GetEventParticipantMapForAuditNoOpenRoom()
        {
            var kioskEvent = new EventDto
            {
                EventId = 1234567
            };

            List<ParticipantDto> participantDtos = new List<ParticipantDto>
            {
                new ParticipantDto
                {
                    DateOfBirth = new DateTime(2010, 01, 01),
                    GroupId = 7766777,
                    LastName = "NoOpenRoom",
                    Nickname = "Jane",
                    ParticipantId = 8765678
                }
            };

            var participantEventMapDto = new ParticipantEventMapDto
            {
                CurrentEvent = kioskEvent,
                Participants = participantDtos
            };

            return participantEventMapDto;
        }

        private List<MpEventParticipantDto> GetMpEventParticipantsForAuditNoGroup()
        {
            var mpEventParticipantList = new List<MpEventParticipantDto>
            {
                new MpEventParticipantDto
                {
                    GroupId = null,
                    ParticipantId = 9876789,
                    RoomId = null
                }
            };

            return mpEventParticipantList;
        }

        private List<MpEventParticipantDto> GetMpEventParticipantsForAuditNoGroupRoom()
        {
            var mpEventParticipantList = new List<MpEventParticipantDto>
            {
                new MpEventParticipantDto
                {
                    GroupId = 7766777,
                    ParticipantId = 8765678,
                    RoomId = null
                }
            };

            return mpEventParticipantList;
        }

        [Test]
        public void ShouldSetSigninErrorMessagesNoGroup()
        {
            // Arrange
            var participantEventMapDto = GetEventParticipantMapForAuditNoParticipantGroup();
            var mpEventParticipantEventList = GetMpEventParticipantsForAuditNoGroup();
            var eligibleEvents = GetTestEventSet();
            var eligibleEventIds = eligibleEvents.Select(r => r.EventId).ToList();

            _roomRepository.Setup(r => r.GetRoomsForEvent(eligibleEventIds, It.IsAny<int>())).Returns(GetClosedEventRooms);
            _groupRepository.Setup(r => r.GetGroup(null, 7766777, false)).Returns(GetGroupForNoOpenRoomsCheck());

            // Act
            _fixture.AuditSigninIssues(participantEventMapDto, mpEventParticipantEventList, eligibleEvents, participantEventMapDto.Participants[0]);

            // Assert
            Assert.AreEqual("Age/Grade Group Not Assigned. Joe is not in a Kids Club Group (DOB: 1/1/2010)", participantEventMapDto.Participants[0].SignInErrorMessage);
        }

        [Test]
        public void ShouldSetSigninErrorMessagesNoGroupRooms()
        {
            // Arrange
            var participantEventMapDto = GetEventParticipantMapForAuditNoOpenRoom();
            var mpEventParticipantEventList = GetMpEventParticipantsForAuditNoGroupRoom();
            var eligibleEvents = GetTestEventSet();
            var eligibleEventIds = eligibleEvents.Select(r => r.EventId).ToList();

            _roomRepository.Setup(r => r.GetRoomsForEvent(eligibleEventIds, It.IsAny<int>())).Returns(GetClosedEventRooms);
            _groupRepository.Setup(r => r.GetGroup(null, 7766777, false)).Returns(GetGroupForNoOpenRoomsCheck());

            // Act
            _fixture.AuditSigninIssues(participantEventMapDto, mpEventParticipantEventList, eligibleEvents, participantEventMapDto.Participants[0]);

            // Assert
            Assert.AreEqual("There are no Kindergarten rooms open for Jane", participantEventMapDto.Participants[0].SignInErrorMessage);
        }

        private List<MpEventRoomDto> GetClosedEventRooms()
        {
            return new List<MpEventRoomDto>();
        }

        private MpGroupDto GetGroupForNoOpenRoomsCheck()
        {
            var mpGroupDto = new MpGroupDto
            {
                Name = "Kindergarten"
            };

            return mpGroupDto;
        }

        [Test]
        public void ShouldDetermineEligibleSignInRooms()
        {
            // Arrange
            var eventDtos = GetTwoNonAcEvents();
            var eventRoomDtos = GetTwoNonAcEventRoomsData();

            _roomRepository.Setup(m => m.GetBumpingRoomsForEventRoom(eventDtos[0].EventId, eventRoomDtos[0].EventRoomId.GetValueOrDefault())).Returns(new List<MpBumpingRoomsDto>());
            _roomRepository.Setup(m => m.GetBumpingRoomsForEventRoom(eventDtos[1].EventId, eventRoomDtos[1].EventRoomId.GetValueOrDefault())).Returns(new List<MpBumpingRoomsDto>());

            // Act
            var result = _fixture.GetEligibleRoomsForEvents(eventDtos, eventRoomDtos);

            // Assert
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void ShouldDetermineEligibleBumpingSignInRooms()
        {
            // Arrange
            var eventDtos = GetTwoNonAcEvents();
            var eventRoomDtos = GetTwoNonAcNoCapacityEventRoomsData();

            _roomRepository.Setup(m => m.GetBumpingRoomsForEventRoom(eventDtos[0].EventId, eventRoomDtos[0].EventRoomId.GetValueOrDefault()))
                .Returns(GetNonAcBumpingRoomsDataByEventRoomId(eventRoomDtos[0].EventRoomId.GetValueOrDefault()));
            _roomRepository.Setup(m => m.GetBumpingRoomsForEventRoom(eventDtos[1].EventId, eventRoomDtos[1].EventRoomId.GetValueOrDefault()))
                .Returns(GetNonAcBumpingRoomsDataByEventRoomId(eventRoomDtos[1].EventRoomId.GetValueOrDefault()));

            // Act
            var result = _fixture.GetEligibleRoomsForEvents(eventDtos, eventRoomDtos);

            // Assert
            Assert.AreEqual(2, result.Count);
        }

        private List<MpEventDto> GetTwoNonAcEvents()
        {
            // we need to set times dynamically on the test data set, to avoid having unit tests
            // break when run at later or earlier hours - this may still be a trouble spot
            var currentStartTime = System.DateTime.Now;
            var futureStartTime = System.DateTime.Now.AddHours(2);

            List<MpEventDto> mpEventDtos = new List<MpEventDto>
            {
                new MpEventDto
                {
                    EventId = 1234567,
                    EventStartDate = currentStartTime,
                    EventTitle = "First Non Ac Event",
                    Cancelled = false,
                    ParentEventId = null
                },
                new MpEventDto
                {
                    EventId = 2345678,
                    EventStartDate = futureStartTime,
                    EventTitle = "Second Non Ac Event",
                    Cancelled = false,
                    ParentEventId = null
                }
            };

            return mpEventDtos;
        }

        private List<MpEventRoomDto> GetTwoNonAcEventRoomsData()
        {
            var eventRooms = new List<MpEventRoomDto>
            {
                new MpEventRoomDto
                {
                    Capacity = 10,
                    CheckedIn = 0,
                    EventId = 1234567,
                    EventRoomId = 1122333,
                    RoomId = 1234,
                    SignedIn = 0
                },
                new MpEventRoomDto
                {
                    Capacity = 10,
                    CheckedIn = 0,
                    EventId = 2345678,
                    EventRoomId = 3332211,
                    RoomId = 4321,
                    SignedIn = 0
                }
            };

            return eventRooms;
        }

        private List<MpEventRoomDto> GetTwoNonAcNoCapacityEventRoomsData()
        {
            var eventRooms = new List<MpEventRoomDto>
            {
                new MpEventRoomDto
                {
                    Capacity = 5,
                    EventId = 1234567,
                    EventRoomId = 1122333,
                    RoomId = 1234,
                    SignedIn = 10
                },
                new MpEventRoomDto
                {
                    Capacity = 10,
                    EventId = 2345678,
                    EventRoomId = 3332211,
                    RoomId = 4321,
                    SignedIn = 10
                }
            };

            return eventRooms;
        }

        private List<MpBumpingRoomsDto> GetNonAcBumpingRoomsDataByEventRoomId(int eventRoomId)
        {
            if (eventRoomId == 1122333)
            {
                List<MpBumpingRoomsDto> bumpingRooms = new List<MpBumpingRoomsDto>
                {
                    new MpBumpingRoomsDto
                    {
                        RoomId = 1231231,
                        AllowSignIn = true,
                        Capacity = 10,
                        RoomName = "Bumping Room 1231231"
                    }
                };

                return bumpingRooms;
            }

            if (eventRoomId == 3332211)
            {
                List<MpBumpingRoomsDto> bumpingRooms = new List<MpBumpingRoomsDto>
                {
                    new MpBumpingRoomsDto
                    {
                        RoomId = 3213213,
                        AllowSignIn = true,
                        Capacity = 10,
                        RoomName = "Bumping Room 3213213"
                    }
                };

                return bumpingRooms;
            }

            return null;
        }

        [Test]
        public void ShouldFinalizeAcRoomAssignments()
        {
            // Act
            var result = _fixture.FinalizeAcRoomAssignments(GetAcEventRoomSignInDataTwoRooms(), GetNonAcEventRoomSignInDataTwoRooms());

            // Assert
            Assert.AreEqual(2, result.Count);
        }

        private List<EventRoomSignInData> GetNonAcEventRoomSignInDataTwoRooms()
        {
            var eventRoomSignInData = new List<EventRoomSignInData>
            {
                new EventRoomSignInData
                {
                    EventId = 1234567,
                    EventRoomId = 1122333,
                    ParentEventId = null,
                    RoomId = 1234,
                    RoomName = "First Non Ac Event Room Sign In Data"
                },
                new EventRoomSignInData
                {
                    EventId = 2345678,
                    EventRoomId = 2233444,
                    ParentEventId = null,
                    RoomId = 2345,
                    RoomName = "Second Non Ac Event Room Sign In Data"
                }
            };

            return eventRoomSignInData;
        }

        private List<EventRoomSignInData> GetAcEventRoomSignInDataTwoRooms()
        {
            var eventRoomSignInData = new List<EventRoomSignInData>
            {
                new EventRoomSignInData
                {
                    EventId = 7654321,
                    EventRoomId = 3332211,
                    ParentEventId = 1234567,
                    RoomId = 4321,
                    RoomName = "First Ac Event Room Sign In Data"
                },
                new EventRoomSignInData
                {
                    EventId = 8765432,
                    EventRoomId = 4443322,
                    ParentEventId = 2345678,
                    RoomId = 5432,
                    RoomName = "Second Ac Event Room Sign In Data"
                }
            };

            return eventRoomSignInData;
        }

        [Test]
        public void ShouldSyncInvalidSignins()
        {
            // Arrange
            var participant = new ParticipantDto
            {
                ParticipantId = 5544555,
                AssignedRoomId = 1234,
                AssignedRoomName = "First Room",
                AssignedSecondaryRoomId = 2345,
                AssignedSecondaryRoomName = "Second Room"
            };

            var mpEventParticipantDtos = new List<MpEventParticipantDto>
            {
                new MpEventParticipantDto
                {
                    ParticipantId = 5544555,
                    RoomId = 1234,
                    RoomName = "First Room"
                },
                new MpEventParticipantDto
                {
                    ParticipantId = 5544555,
                    RoomId = null,
                    RoomName = String.Empty
                }
            };

            // Act
            _fixture.SyncInvalidSignins(mpEventParticipantDtos, participant);

            // Assert
            Assert.AreEqual(null, participant.AssignedRoomId);
            Assert.AreEqual(string.Empty, participant.AssignedRoomName);
            Assert.AreEqual(null, participant.AssignedSecondaryRoomId);
            Assert.AreEqual(string.Empty, participant.AssignedSecondaryRoomName);
        }

        [Test]
        public void ShouldSignInTwoYearOldToTwoNonAcEvents()
        {
            // Arrange
            var twoYearOldBirthdate = System.DateTime.Now.AddYears(-1);

            var participant = new ParticipantDto
            {
                DateOfBirth = twoYearOldBirthdate,
                ParticipantId = 5544555,
                GroupId = 1234123
            };

            var participantEventMapDto = new ParticipantEventMapDto
            {
                ServicesAttended = 2,
                CurrentEvent = new EventDto
                {
                    EventSiteId  = 8
                },
                // TODO: consider adding participant data to this list to test audit signin issues
                Participants = new List<ParticipantDto>()
            };

            var eventGroups = new List<MpEventGroupDto>
            {
                new MpEventGroupDto
                {
                    GroupId = 1234123,
                    RoomReservationId = 9988776
                },
                new MpEventGroupDto
                {
                    GroupId = 1234123,
                    RoomReservationId = 8877665
                }
            };

            var eventRooms = new List<MpEventRoomDto>
            {
                new MpEventRoomDto
                {
                    AllowSignIn = true,
                    EventId = 1234567,
                    EventRoomId = 9988776,
                    RoomId = 1234,
                    Capacity = 10,
                    SignedIn = 0,
                    CheckedIn = 0
                },
                new MpEventRoomDto
                {
                    AllowSignIn = true,
                    EventId = 7654321,
                    EventRoomId = 6778899,
                    RoomId = 4321,
                    Capacity = 10,
                    SignedIn = 0,
                    CheckedIn = 0
                },
                new MpEventRoomDto
                {
                    AllowSignIn = true,
                    EventId = 2345678,
                    EventRoomId = 8877665,
                    RoomId = 2345,
                    Capacity = 10,
                    SignedIn = 0,
                    CheckedIn = 0
                },
                new MpEventRoomDto
                {
                    AllowSignIn = true,
                    EventId = 8765432,
                    EventRoomId = 5667788,
                    RoomId = 5432,
                    Capacity = 10,
                    SignedIn = 0,
                    CheckedIn = 0
                }
            };

            var eventList = GetTestEventSet();

            // these mocked service calls are for the GetSignInEventRooms function
            _eventRepository.Setup(r => r.GetEventGroupsByGroupIdAndEventIds(participant.GroupId.GetValueOrDefault(), It.IsAny<List<int>>()))
                .Returns(eventGroups);

            // this part of the test is a little sketchy - in the live code, we're passing down a list of room reservation ids from the
            // list of event group records, which would potentially limit what got returned, as opposed to a static list
            var eventRoomsIds = eventGroups.Select(r => r.RoomReservationId.GetValueOrDefault()).ToList();
            _roomRepository.Setup(r => r.GetEventRoomsByEventRoomIds(eventRoomsIds)).Returns(eventRooms);

            // Act
            var result = _fixture.SignInParticipant(participant, participantEventMapDto, eventList);

            // Assert
            Assert.AreEqual(1234, result[0].RoomId);
            Assert.AreEqual(2345, result[1].RoomId);
        }

        [Test]
        public void ShouldSignInFourYearOldToOneServiceEventAndOneAcEvent()
        {
            // Arrange
            var fourYearOldBirthdate = System.DateTime.Now.AddYears(-5);

            var participant = new ParticipantDto
            {
                DateOfBirth = fourYearOldBirthdate,
                ParticipantId = 5544555,
                GroupId = 1234123
            };

            var participantEventMapDto = new ParticipantEventMapDto
            {
                ServicesAttended = 2,
                CurrentEvent = new EventDto
                {
                    EventSiteId = 8
                },
                // TODO: consider adding participant data to this list to test audit signin issues
                Participants = new List<ParticipantDto>()
            };

            var eventGroups = new List<MpEventGroupDto>
            {
                new MpEventGroupDto
                {
                    GroupId = 1234123,
                    RoomReservationId = 9988776
                },
                new MpEventGroupDto
                {
                    GroupId = 1234123,
                    RoomReservationId = 8877665
                }
            };

            var eventRooms = new List<MpEventRoomDto>
            {
                new MpEventRoomDto
                {
                    AllowSignIn = true,
                    EventId = 1234567,
                    EventRoomId = 9988776,
                    RoomId = 1234,
                    Capacity = 10,
                    SignedIn = 0,
                    CheckedIn = 0
                },
                new MpEventRoomDto
                {
                    AllowSignIn = true,
                    EventId = 7654321,
                    EventRoomId = 6778899,
                    RoomId = 4321,
                    Capacity = 10,
                    SignedIn = 0,
                    CheckedIn = 0
                },
                new MpEventRoomDto
                {
                    AllowSignIn = true,
                    EventId = 2345678,
                    EventRoomId = 8877665,
                    RoomId = 2345,
                    Capacity = 10,
                    SignedIn = 0,
                    CheckedIn = 0
                },
                new MpEventRoomDto
                {
                    AllowSignIn = true,
                    EventId = 8765432,
                    EventRoomId = 5667788,
                    RoomId = 5432,
                    Capacity = 10,
                    SignedIn = 0,
                    CheckedIn = 0
                }
            };

            var eventList = GetTestEventSet();

            // these mocked service calls are for the GetSignInEventRooms function
            _eventRepository.Setup(r => r.GetEventGroupsByGroupIdAndEventIds(participant.GroupId.GetValueOrDefault(), It.IsAny<List<int>>()))
                .Returns(eventGroups);

            // this part of the test is a little sketchy - in the live code, we're passing down a list of room reservation ids from the
            // list of event group records, which would potentially limit what got returned, as opposed to a static list
            var eventRoomsIds = eventGroups.Select(r => r.RoomReservationId.GetValueOrDefault()).ToList();
            _roomRepository.Setup(r => r.GetEventRoomsByEventRoomIds(eventRoomsIds)).Returns(eventRooms);

            // Act
            var result = _fixture.SignInParticipant(participant, participantEventMapDto, eventList);

            // Assert
            Assert.AreEqual(1234, result[0].RoomId);
            Assert.AreEqual(5432, result[1].RoomId);
        }

        [Test]
        public void ShouldSignIntoAcWithAcAsFirstEventOnly()
        {
            // Arrange
            var fourYearOldBirthdate = System.DateTime.Now.AddYears(-5);

            var participant = new ParticipantDto
            {
                DateOfBirth = fourYearOldBirthdate,
                ParticipantId = 5544555,
                GroupId = 1234123
            };

            var participantEventMapDto = new ParticipantEventMapDto
            {
                ServicesAttended = 2,
                CurrentEvent = new EventDto
                {
                    EventSiteId = 8
                },
                // TODO: consider adding participant data to this list to test audit signin issues
                Participants = new List<ParticipantDto>()
            };

            var eventGroups = new List<MpEventGroupDto>
            {
                new MpEventGroupDto
                {
                    GroupId = 1234123,
                    RoomReservationId = 9988776
                },
                new MpEventGroupDto
                {
                    GroupId = 1234123,
                    RoomReservationId = 8877665
                }
            };

            var eventRooms = new List<MpEventRoomDto>
            {
                new MpEventRoomDto
                {
                    AllowSignIn = true,
                    EventId = 1234567,
                    EventRoomId = 9988776,
                    RoomId = 1234,
                    Capacity = 10,
                    SignedIn = 0,
                    CheckedIn = 0
                },
                new MpEventRoomDto
                {
                    AllowSignIn = true,
                    EventId = 7654321,
                    EventRoomId = 6778899,
                    RoomId = 4321,
                    Capacity = 10,
                    SignedIn = 0,
                    CheckedIn = 0
                },
                new MpEventRoomDto
                {
                    AllowSignIn = true,
                    EventId = 2345678,
                    EventRoomId = 8877665,
                    RoomId = 2345,
                    Capacity = 10,
                    SignedIn = 0,
                    CheckedIn = 0
                }
            };

            var eventList = GetTestEventSet();

            // these mocked service calls are for the GetSignInEventRooms function
            _eventRepository.Setup(r => r.GetEventGroupsByGroupIdAndEventIds(participant.GroupId.GetValueOrDefault(), It.IsAny<List<int>>()))
                .Returns(eventGroups);

            // this part of the test is a little sketchy - in the live code, we're passing down a list of room reservation ids from the
            // list of event group records, which would potentially limit what got returned, as opposed to a static list
            var eventRoomsIds = eventGroups.Select(r => r.RoomReservationId.GetValueOrDefault()).ToList();
            _roomRepository.Setup(r => r.GetEventRoomsByEventRoomIds(eventRoomsIds)).Returns(eventRooms);

            // Act
            var result = _fixture.SignInParticipant(participant, participantEventMapDto, eventList);

            // Assert
            Assert.AreEqual(2345, result[0].RoomId);
            Assert.AreEqual(4321, result[1].RoomId);
        }

        [Test]
        public void ShouldSignIntoAcWithAcAsSecondEventOnly()
        {
            // Arrange
            var fourYearOldBirthdate = System.DateTime.Now.AddYears(-5);

            var participant = new ParticipantDto
            {
                DateOfBirth = fourYearOldBirthdate,
                ParticipantId = 5544555,
                GroupId = 1234123
            };

            var participantEventMapDto = new ParticipantEventMapDto
            {
                ServicesAttended = 2,
                CurrentEvent = new EventDto
                {
                    EventSiteId = 8
                },
                // TODO: consider adding participant data to this list to test audit signin issues
                Participants = new List<ParticipantDto>()
            };

            var eventGroups = new List<MpEventGroupDto>
            {
                new MpEventGroupDto
                {
                    GroupId = 1234123,
                    RoomReservationId = 9988776
                },
                new MpEventGroupDto
                {
                    GroupId = 1234123,
                    RoomReservationId = 8877665
                }
            };

            var eventRooms = new List<MpEventRoomDto>
            {
                new MpEventRoomDto
                {
                    AllowSignIn = true,
                    EventId = 1234567,
                    EventRoomId = 9988776,
                    RoomId = 1234,
                    Capacity = 10,
                    SignedIn = 0,
                    CheckedIn = 0
                },
                new MpEventRoomDto
                {
                    AllowSignIn = true,
                    EventId = 2345678,
                    EventRoomId = 8877665,
                    RoomId = 2345,
                    Capacity = 10,
                    SignedIn = 0,
                    CheckedIn = 0
                },
                new MpEventRoomDto
                {
                    AllowSignIn = true,
                    EventId = 8765432,
                    EventRoomId = 5667788,
                    RoomId = 5432,
                    Capacity = 10,
                    SignedIn = 0,
                    CheckedIn = 0
                }
            };

            var eventList = GetTestEventSet();

            // these mocked service calls are for the GetSignInEventRooms function
            _eventRepository.Setup(r => r.GetEventGroupsByGroupIdAndEventIds(participant.GroupId.GetValueOrDefault(), It.IsAny<List<int>>()))
                .Returns(eventGroups);

            // this part of the test is a little sketchy - in the live code, we're passing down a list of room reservation ids from the
            // list of event group records, which would potentially limit what got returned, as opposed to a static list
            var eventRoomsIds = eventGroups.Select(r => r.RoomReservationId.GetValueOrDefault()).ToList();
            _roomRepository.Setup(r => r.GetEventRoomsByEventRoomIds(eventRoomsIds)).Returns(eventRooms);

            // Act
            var result = _fixture.SignInParticipant(participant, participantEventMapDto, eventList);

            // Assert
            Assert.AreEqual(1234, result[0].RoomId);
            Assert.AreEqual(5432, result[1].RoomId);
        }

        [Test]
        public void ShouldNotSignIntoAcWithNoAcRooms()
        {
            // Arrange
            var fourYearOldBirthdate = System.DateTime.Now.AddYears(-5);

            var participant = new ParticipantDto
            {
                DateOfBirth = fourYearOldBirthdate,
                ParticipantId = 5544555,
                GroupId = 1234123
            };

            var participantEventMapDto = new ParticipantEventMapDto
            {
                ServicesAttended = 2,
                CurrentEvent = new EventDto
                {
                    EventSiteId = 8
                },
                // TODO: consider adding participant data to this list to test audit signin issues
                Participants = new List<ParticipantDto>()
            };

            var eventGroups = new List<MpEventGroupDto>
            {
                new MpEventGroupDto
                {
                    GroupId = 1234123,
                    RoomReservationId = 9988776
                },
                new MpEventGroupDto
                {
                    GroupId = 1234123,
                    RoomReservationId = 8877665
                }
            };

            var eventRooms = new List<MpEventRoomDto>
            {
                new MpEventRoomDto
                {
                    AllowSignIn = true,
                    EventId = 1234567,
                    EventRoomId = 9988776,
                    RoomId = 1234,
                    Capacity = 10,
                    SignedIn = 0,
                    CheckedIn = 0
                },
                new MpEventRoomDto
                {
                    AllowSignIn = true,
                    EventId = 2345678,
                    EventRoomId = 8877665,
                    RoomId = 2345,
                    Capacity = 10,
                    SignedIn = 0,
                    CheckedIn = 0
                }
            };

            var eventList = GetTestEventSet();

            // these mocked service calls are for the GetSignInEventRooms function
            _eventRepository.Setup(r => r.GetEventGroupsByGroupIdAndEventIds(participant.GroupId.GetValueOrDefault(), It.IsAny<List<int>>()))
                .Returns(eventGroups);

            _roomRepository.Setup(r => r.GetRoomsForEvent(It.IsAny<List<int>>(), It.IsAny<int>())).Returns(eventRooms);

            // this part of the test is a little sketchy - in the live code, we're passing down a list of room reservation ids from the
            // list of event group records, which would potentially limit what got returned, as opposed to a static list
            var eventRoomsIds = eventGroups.Select(r => r.RoomReservationId.GetValueOrDefault()).ToList();
            _roomRepository.Setup(r => r.GetEventRoomsByEventRoomIds(eventRoomsIds)).Returns(eventRooms);

            // Act
            var result = _fixture.SignInParticipant(participant, participantEventMapDto, eventList);

            // Assert
            Assert.AreEqual(null, result[0].RoomId);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ShouldNotSignIntoAcWithClosedAcRooms()
        {
            // Arrange
            var fourYearOldBirthdate = System.DateTime.Now.AddYears(-5);

            var participant = new ParticipantDto
            {
                DateOfBirth = fourYearOldBirthdate,
                ParticipantId = 5544555,
                GroupId = 1234123
            };

            var participantEventMapDto = new ParticipantEventMapDto
            {
                ServicesAttended = 2,
                CurrentEvent = new EventDto
                {
                    EventSiteId = 8
                },
                // TODO: consider adding participant data to this list to test audit signin issues
                Participants = new List<ParticipantDto>()
            };

            var eventGroups = new List<MpEventGroupDto>
            {
                new MpEventGroupDto
                {
                    GroupId = 1234123,
                    RoomReservationId = 9988776
                },
                new MpEventGroupDto
                {
                    GroupId = 1234123,
                    RoomReservationId = 8877665
                }
            };

            var eventRooms = new List<MpEventRoomDto>
            {
                new MpEventRoomDto
                {
                    AllowSignIn = true,
                    EventId = 1234567,
                    EventRoomId = 9988776,
                    RoomId = 1234,
                    Capacity = 10,
                    SignedIn = 0,
                    CheckedIn = 0
                },
                new MpEventRoomDto
                {
                    AllowSignIn = false,
                    EventId = 7654321,
                    EventRoomId = 6778899,
                    RoomId = 4321,
                    Capacity = 10,
                    SignedIn = 0,
                    CheckedIn = 0
                },
                new MpEventRoomDto
                {
                    AllowSignIn = true,
                    EventId = 2345678,
                    EventRoomId = 8877665,
                    RoomId = 2345,
                    Capacity = 10,
                    SignedIn = 0,
                    CheckedIn = 0
                },
                new MpEventRoomDto
                {
                    AllowSignIn = false,
                    EventId = 8765432,
                    EventRoomId = 5667788,
                    RoomId = 5432,
                    Capacity = 10,
                    SignedIn = 0,
                    CheckedIn = 0
                }
            };

            var eventList = GetTestEventSet();

            // these mocked service calls are for the GetSignInEventRooms function
            _eventRepository.Setup(r => r.GetEventGroupsByGroupIdAndEventIds(participant.GroupId.GetValueOrDefault(), It.IsAny<List<int>>()))
                .Returns(eventGroups);

            _roomRepository.Setup(r => r.GetRoomsForEvent(It.IsAny<List<int>>(), It.IsAny<int>())).Returns(eventRooms);

            // this part of the test is a little sketchy - in the live code, we're passing down a list of room reservation ids from the
            // list of event group records, which would potentially limit what got returned, as opposed to a static list
            var eventRoomsIds = eventGroups.Select(r => r.RoomReservationId.GetValueOrDefault()).ToList();
            _roomRepository.Setup(r => r.GetEventRoomsByEventRoomIds(eventRoomsIds)).Returns(eventRooms);

            // Act
            var result = _fixture.SignInParticipant(participant, participantEventMapDto, eventList);

            // Assert
            Assert.AreEqual(null, result[0].RoomId);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ShouldSignIntoAcWithSecondClosedAcRoom()
        {
            // Arrange
            var fourYearOldBirthdate = System.DateTime.Now.AddYears(-5);

            var participant = new ParticipantDto
            {
                DateOfBirth = fourYearOldBirthdate,
                ParticipantId = 5544555,
                GroupId = 1234123
            };

            var participantEventMapDto = new ParticipantEventMapDto
            {
                ServicesAttended = 2,
                CurrentEvent = new EventDto
                {
                    EventSiteId = 8
                },
                // TODO: consider adding participant data to this list to test audit signin issues
                Participants = new List<ParticipantDto>()
            };

            var eventGroups = new List<MpEventGroupDto>
            {
                new MpEventGroupDto
                {
                    GroupId = 1234123,
                    RoomReservationId = 9988776
                },
                new MpEventGroupDto
                {
                    GroupId = 1234123,
                    RoomReservationId = 8877665
                }
            };

            var eventRooms = new List<MpEventRoomDto>
            {
                new MpEventRoomDto
                {
                    AllowSignIn = true,
                    EventId = 1234567,
                    EventRoomId = 9988776,
                    RoomId = 1234,
                    Capacity = 10,
                    SignedIn = 0,
                    CheckedIn = 0
                },
                new MpEventRoomDto
                {
                    AllowSignIn = true,
                    EventId = 7654321,
                    EventRoomId = 6778899,
                    RoomId = 4321,
                    Capacity = 10,
                    SignedIn = 0,
                    CheckedIn = 0
                },
                new MpEventRoomDto
                {
                    AllowSignIn = true,
                    EventId = 2345678,
                    EventRoomId = 8877665,
                    RoomId = 2345,
                    Capacity = 10,
                    SignedIn = 0,
                    CheckedIn = 0
                },
                new MpEventRoomDto
                {
                    AllowSignIn = false,
                    EventId = 8765432,
                    EventRoomId = 5667788,
                    RoomId = 5432,
                    Capacity = 10,
                    SignedIn = 0,
                    CheckedIn = 0
                }
            };

            var eventList = GetTestEventSet();

            // these mocked service calls are for the GetSignInEventRooms function
            _eventRepository.Setup(r => r.GetEventGroupsByGroupIdAndEventIds(participant.GroupId.GetValueOrDefault(), It.IsAny<List<int>>()))
                .Returns(eventGroups);

            // this part of the test is a little sketchy - in the live code, we're passing down a list of room reservation ids from the
            // list of event group records, which would potentially limit what got returned, as opposed to a static list
            var eventRoomsIds = eventGroups.Select(r => r.RoomReservationId.GetValueOrDefault()).ToList();
            _roomRepository.Setup(r => r.GetEventRoomsByEventRoomIds(eventRoomsIds)).Returns(eventRooms);

            // Act
            var result = _fixture.SignInParticipant(participant, participantEventMapDto, eventList);

            // Assert
            Assert.AreEqual(2345, result[0].RoomId);
            Assert.AreEqual(4321, result[1].RoomId);
        }

        [Test]
        public void ShouldSignIntoAcWithFirstClosedAcRoom()
        {
            // Arrange
            var fourYearOldBirthdate = System.DateTime.Now.AddYears(-5);

            var participant = new ParticipantDto
            {
                DateOfBirth = fourYearOldBirthdate,
                ParticipantId = 5544555,
                GroupId = 1234123
            };

            var participantEventMapDto = new ParticipantEventMapDto
            {
                ServicesAttended = 2,
                CurrentEvent = new EventDto
                {
                    EventSiteId = 8
                },
                // TODO: consider adding participant data to this list to test audit signin issues
                Participants = new List<ParticipantDto>()
            };

            var eventGroups = new List<MpEventGroupDto>
            {
                new MpEventGroupDto
                {
                    GroupId = 1234123,
                    RoomReservationId = 9988776
                },
                new MpEventGroupDto
                {
                    GroupId = 1234123,
                    RoomReservationId = 8877665
                }
            };

            var eventRooms = new List<MpEventRoomDto>
            {
                new MpEventRoomDto
                {
                    AllowSignIn = true,
                    EventId = 1234567,
                    EventRoomId = 9988776,
                    RoomId = 1234,
                    Capacity = 10,
                    SignedIn = 0,
                    CheckedIn = 0
                },
                new MpEventRoomDto
                {
                    AllowSignIn = false,
                    EventId = 7654321,
                    EventRoomId = 6778899,
                    RoomId = 4321,
                    Capacity = 10,
                    SignedIn = 0,
                    CheckedIn = 0
                },
                new MpEventRoomDto
                {
                    AllowSignIn = true,
                    EventId = 2345678,
                    EventRoomId = 8877665,
                    RoomId = 2345,
                    Capacity = 10,
                    SignedIn = 0,
                    CheckedIn = 0
                },
                new MpEventRoomDto
                {
                    AllowSignIn = true,
                    EventId = 8765432,
                    EventRoomId = 5667788,
                    RoomId = 5432,
                    Capacity = 10,
                    SignedIn = 0,
                    CheckedIn = 0
                }
            };

            var eventList = GetTestEventSet();

            // these mocked service calls are for the GetSignInEventRooms function
            _eventRepository.Setup(r => r.GetEventGroupsByGroupIdAndEventIds(participant.GroupId.GetValueOrDefault(), It.IsAny<List<int>>()))
                .Returns(eventGroups);

            // this part of the test is a little sketchy - in the live code, we're passing down a list of room reservation ids from the
            // list of event group records, which would potentially limit what got returned, as opposed to a static list
            var eventRoomsIds = eventGroups.Select(r => r.RoomReservationId.GetValueOrDefault()).ToList();
            _roomRepository.Setup(r => r.GetEventRoomsByEventRoomIds(eventRoomsIds)).Returns(eventRooms);

            // Act
            var result = _fixture.SignInParticipant(participant, participantEventMapDto, eventList);

            // Assert
            Assert.AreEqual(1234, result[0].RoomId);
            Assert.AreEqual(5432, result[1].RoomId);
        }

        // JPC - this test is mothballed for the moment. The intent was to test the SignInLogic class from end to end, using multiple participants, etc.
        // However, the call we make down to the participant repository, when inserting the event participants and getting back these objects, would have
        // to be mocked for the specific data going in, as this would have to be defined multiple times in the test, once for each participant. The problem then
        // is that we have date time data on the objet being inserted, which would also have to be set. I believe there's a way to do some logic via a 
        // LINQ query on the mock params - using ItIs instead of ItIsAny - but I didn't pursue that, instead looking at how to use Moq callbacks to get the 
        // data being passed to the mocked object. Either way, I think the unit tests we have in place around the signin logic now are solid enough that 
        // getting this test to work correctly is probably of limited value

        //[Test]
        //public void ShouldSignInMixedParticipantsOneIntoAcOneNotIntoAc()
        //{
        //    // Arrange
        //    var twoYearOldBirthdate = System.DateTime.Now.AddYears(-1);
        //    var fourYearOldBirthdate = System.DateTime.Now.AddYears(-4);

        //    var twoYearOldParticipant = new ParticipantDto
        //    {
        //        DateOfBirth = twoYearOldBirthdate,
        //        ParticipantId = 6655666,
        //        GroupId = 2345234,
        //        DuplicateSignIn = false,
        //        Selected = true
        //    };

        //    var fourYearOldParticipant = new ParticipantDto
        //    {
        //        DateOfBirth = fourYearOldBirthdate,
        //        ParticipantId = 5544555,
        //        GroupId = 1234123,
        //        DuplicateSignIn = false,
        //        Selected = true
        //    };

        //    var participantEventMapDto = new ParticipantEventMapDto
        //    {
        //        ServicesAttended = 2,
        //        CurrentEvent = new EventDto
        //        {
        //            EventSiteId = 8
        //        },
        //        Participants = new List<ParticipantDto>
        //        {
        //            twoYearOldParticipant,
        //            fourYearOldParticipant
        //        }
        //    };

        //    var twoYearOldEventGroups = new List<MpEventGroupDto>
        //    {
        //        new MpEventGroupDto
        //        {
        //            GroupId = 2345234,
        //            RoomReservationId = 1726354
        //        },
        //        new MpEventGroupDto
        //        {
        //            GroupId = 2345234,
        //            RoomReservationId = 2837465
        //        }
        //    };

        //    var twoYearOldEventRooms = new List<MpEventRoomDto>
        //    {
        //        new MpEventRoomDto
        //        {
        //            AllowSignIn = true,
        //            EventId = 1234567,
        //            EventRoomId = 1726354,
        //            RoomId = 4567,
        //            Capacity = 10,
        //            SignedIn = 0,
        //            CheckedIn = 0
        //        },
        //        new MpEventRoomDto
        //        {
        //            AllowSignIn = true,
        //            EventId = 2345678,
        //            EventRoomId = 2837465,
        //            RoomId = 5678,
        //            Capacity = 10,
        //            SignedIn = 0,
        //            CheckedIn = 0
        //        }
        //    };

        //    var fourYearOldEventGroups = new List<MpEventGroupDto>
        //    {
        //        new MpEventGroupDto
        //        {
        //            GroupId = 1234123,
        //            RoomReservationId = 9988776
        //        },
        //        new MpEventGroupDto
        //        {
        //            GroupId = 1234123,
        //            RoomReservationId = 8877665
        //        }
        //    };

        //    var fourYearOldEventRooms = new List<MpEventRoomDto>
        //    {
        //        new MpEventRoomDto
        //        {
        //            AllowSignIn = true,
        //            EventId = 1234567,
        //            EventRoomId = 9988776,
        //            RoomId = 1234,
        //            Capacity = 10,
        //            SignedIn = 0,
        //            CheckedIn = 0
        //        },
        //        new MpEventRoomDto
        //        {
        //            AllowSignIn = true,
        //            EventId = 7654321,
        //            EventRoomId = 6778899,
        //            RoomId = 4321,
        //            Capacity = 10,
        //            SignedIn = 0,
        //            CheckedIn = 0
        //        },
        //        new MpEventRoomDto
        //        {
        //            AllowSignIn = true,
        //            EventId = 2345678,
        //            EventRoomId = 8877665,
        //            RoomId = 2345,
        //            Capacity = 10,
        //            SignedIn = 0,
        //            CheckedIn = 0
        //        },
        //        new MpEventRoomDto
        //        {
        //            AllowSignIn = true,
        //            EventId = 8765432,
        //            EventRoomId = 5667788,
        //            RoomId = 5432,
        //            Capacity = 10,
        //            SignedIn = 0,
        //            CheckedIn = 0
        //        }
        //    };

        //    var eventList = GetTestEventSet();

        //    // these mocked service calls are for the GetSignInEventRooms function
        //    _eventRepository.Setup(r => r.GetEventGroupsByGroupIdAndEventIds(2345234, It.IsAny<List<int>>()))
        //        .Returns(twoYearOldEventGroups);

        //    _eventRepository.Setup(r => r.GetEventGroupsByGroupIdAndEventIds(1234123, It.IsAny<List<int>>()))
        //        .Returns(fourYearOldEventGroups);

        //    // this part of the test is a little sketchy - in the live code, we're passing down a list of room reservation ids from the
        //    // list of event group records, which would potentially limit what got returned, as opposed to a static list
        //    var twoYearOldEventRoomsIds = twoYearOldEventGroups.Select(r => r.RoomReservationId.GetValueOrDefault()).ToList();
        //    _roomRepository.Setup(r => r.GetEventRoomsByEventRoomIds(twoYearOldEventRoomsIds)).Returns(twoYearOldEventRooms);

        //    var fourYearOldEventRoomsIds = fourYearOldEventGroups.Select(r => r.RoomReservationId.GetValueOrDefault()).ToList();
        //    _roomRepository.Setup(r => r.GetEventRoomsByEventRoomIds(fourYearOldEventRoomsIds)).Returns(fourYearOldEventRooms);

        //    // this is mocked out here to avoid a null exception error in the audit signin code - if we want to run a whole test 
        //    // and test the no room conditions as part of that test, we'll need to mock this with something more meaningful
        //    _roomRepository.Setup(r => r.GetRoomsForEvent(It.IsAny<List<int>>(), It.IsAny<int>())).Returns(new List<MpEventRoomDto>());

        //    // this needs to have good data going in and coming out, as it will be used to build the participant list to get the test results
        //    _childSigninRepository.Setup(r => r.CreateEventParticipants(It.IsAny<List<MpEventParticipantDto>>())).Returns(new List<MpEventParticipantDto>());

        //    // these two lines were approaches to try to make the test match the data correctly
        //    _childSigninRepository.Setup(r => r.CreateEventParticipants(It.Is<List<MpEventParticipantDto>>(s => s.First().RoomId == 2))).Returns(new List<MpEventParticipantDto>());
        //    _childSigninRepository.Setup(r => r.CreateEventParticipants(It.IsAny<List<MpEventParticipantDto>>())).Callback<List<MpEventParticipantDto>>(r => expectedResult = r);

        //    // Act
        //    var result = _fixture.SignInParticipants(participantEventMapDto, eventList);

        //    //// Assert
        //    //Assert.AreEqual(1234, result[0].RoomId);
        //    //Assert.AreEqual(5432, result[1].RoomId);
        //}
    }
}
