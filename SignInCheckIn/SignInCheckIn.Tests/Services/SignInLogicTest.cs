using System;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Crossroads.Utilities.Services.Interfaces;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
                _groupRepository.Object, _roomRepository.Object, _participantRepository.Object);
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
            var result = _fixture.GetSignInEvents(siteId, underThreeSignIn, adventureClubSignIn);

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
            var result = _fixture.GetSignInEvents(siteId, adventureClubSignIn, underThreeSignIn);

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

            var signInTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 00, 00);

            _eventRepository.Setup(r => r.GetEvents(signInTime, signInTime, 1, true)).Returns(GetTestEventSetCurrentEventAc());

            // Act
            var result = _fixture.GetSignInEvents(siteId, adventureClubSignIn, underThreeSignIn);

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

            var signInTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 00, 00);

            _eventRepository.Setup(r => r.GetEvents(signInTime, signInTime, 1, true)).Returns(GetTestEventSetFutureEventAc());

            // Act
            var result = _fixture.GetSignInEvents(siteId, adventureClubSignIn, underThreeSignIn);

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

            var signInTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 00, 00);

            _eventRepository.Setup(r => r.GetEvents(signInTime, signInTime, 1, true)).Returns(GetTestEventSet());

            // Act
            var result = _fixture.GetSignInEvents(siteId, adventureClubSignIn, underThreeSignIn);

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
                    RoomId = 1234
                },
                new MpEventRoomDto
                {
                    Capacity = 10,
                    EventRoomId = 3332211,
                    RoomId = 4321
                }
            };

            return eventRooms;
        }

        [Test]
        public void ShouldAssignNonAcParticipantToRooms()
        {
            // Arrange


            //// Act
            ////var result = _fixture.AssignParticipantToRoomsNonAc(GetEventRoomsData());
            //var result = _fixture.AssignParticipantToRoomsNonAc(GetEventRoomsData(), GetTestEventSet());

            //// Assert
            //Assert.AreEqual(2, result.Count);
        }

        private ParticipantEventMapDto GetEventParticipantMapForAudit()
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
                    GroupId = 5544555,
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

        private List<MpEventParticipantDto> GetMpEventParticipantsForAudit()
        {
            var mpEventParticipantList = new List<MpEventParticipantDto>
            {
                new MpEventParticipantDto
                {
                    GroupId = null,
                    ParticipantId = 9876789,
                    RoomId = null
                },
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
        public void ShouldSetSigninErrorMessages()
        {
            // Arrange
            var participantEventMapDto = GetEventParticipantMapForAudit();
            var mpEventParticipantEventList = GetMpEventParticipantsForAudit();
            var eligibleEvents = GetTestEventSet();
            var eligibleEventIds = eligibleEvents.Select(r => r.EventId).ToList();

            _roomRepository.Setup(r => r.GetEventRoomsByEventRoomIds(eligibleEventIds)).Returns(GetClosedEventRooms());
            _groupRepository.Setup(r => r.GetGroup(null, 7766777, false)).Returns(GetGroupForNoOpenRoomsCheck());

            // Act
            _fixture.AuditSigninIssues(participantEventMapDto, mpEventParticipantEventList, eligibleEvents);

            // Assert
            Assert.AreEqual("Age/Grade Group Not Assigned. Joe is not in a Kids Club Group (DOB: 1/1/2010)", participantEventMapDto.Participants[0].SignInErrorMessage);
            Assert.AreEqual("There are no Kindergarten rooms open for Jane", participantEventMapDto.Participants[1].SignInErrorMessage);
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
    }
}
