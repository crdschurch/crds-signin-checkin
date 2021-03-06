﻿using Crossroads.Utilities.Services.Interfaces;
using FluentAssertions;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using NUnit.Framework;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Services;
using System;
using System.Collections.Generic;

namespace SignInCheckIn.Tests.Services
{
    public class EventServiceTest
    {
        private Mock<IEventRepository> _eventRepository;
        private Mock<IConfigRepository> _configRepository;
        private Mock<IRoomRepository> _roomRepository;
        private Mock<IApplicationConfiguration> _applicationConfiguation;
        private Mock<IParticipantRepository> _participantRepository;
        private Mock<IKioskRepository> _kioskRepository;

        private const int CheckinKioskTypeId = 2;
        private const int StudentMinistrySignInKioskTypeId = 4;
        private const int BigEventTypeId = 369;
        private const int MiddleSchoolSmEventTypeId = 402;
        private const int HighSchoolSmEventTypeId = 403;

        private EventService _fixture;

        [SetUp]
        public void SetUp()
        {
            AutoMapperConfig.RegisterMappings();

            _eventRepository = new Mock<IEventRepository>();
            _configRepository = new Mock<IConfigRepository>();
            _roomRepository = new Mock<IRoomRepository>(MockBehavior.Strict);
            _applicationConfiguation = new Mock<IApplicationConfiguration>(MockBehavior.Strict);
            _participantRepository = new Mock<IParticipantRepository>(MockBehavior.Strict);
            _kioskRepository = new Mock<IKioskRepository>();

            var mpConfigDtoEarly = new MpConfigDto
            {
                ApplicationCode = "COMMON",
                ConfigurationSettingId = 1,
                KeyName = "DefaultEarlyCheckIn",
                Value = "60"
            };

            var mpConfigDtoLate = new MpConfigDto
            {
                ApplicationCode = "COMMON",
                ConfigurationSettingId = 1,
                KeyName = "DefaultLateCheckIn",
                Value = "60"
            };

            _configRepository.Setup(m => m.GetMpConfigByKey("DefaultEarlyCheckIn")).Returns(mpConfigDtoEarly);
            _configRepository.Setup(m => m.GetMpConfigByKey("DefaultLateCheckIn")).Returns(mpConfigDtoLate);
            _applicationConfiguation.Setup(ac => ac.AdventureClubEventTypeId).Returns(20);
            _applicationConfiguation.Setup(ac => ac.CheckinKioskTypeId).Returns(CheckinKioskTypeId);
            _applicationConfiguation.Setup(ac => ac.StudentMinistryKioskTypeId).Returns(StudentMinistrySignInKioskTypeId);
            _applicationConfiguation.Setup(ac => ac.StudentMinistryGradesSixToEightEventTypeId).Returns(MiddleSchoolSmEventTypeId);
            _applicationConfiguation.Setup(ac => ac.StudentMinistryGradesNineToTwelveEventTypeId).Returns(HighSchoolSmEventTypeId);
            _applicationConfiguation.Setup(ac => ac.BigEventTypeId).Returns(BigEventTypeId);

            _fixture = new EventService(_eventRepository.Object, _configRepository.Object, _roomRepository.Object, _applicationConfiguation.Object, _participantRepository.Object,
                _kioskRepository.Object);
        }

        [Test]
        public void ShouldGetEvents()
        {
            // Arrange
            var kioskConfig = new MpKioskConfigDto
            {
                KioskTypeId = 1
            };

            var mpEventDtos = new List<MpEventDto>();

            var testMpEventDto = new MpEventDto
            {
                CongregationName = "Oakley",
                EventStartDate = new DateTime(2016, 10, 10),
                EventId = 1234567,
                EventTitle = "Test Event",
                EventType = "Oakley Service"
            };

            mpEventDtos.Add(testMpEventDto);

            var start = new DateTime(2016, 10, 9);
            var end = new DateTime(2016, 10, 12);
            const int site = 1;
            _kioskRepository.Setup(m => m.GetMpKioskConfigByIdentifier(It.IsAny<Guid>())).Returns(kioskConfig);
            _eventRepository.Setup(m => m.GetEvents(start, end, site, false, It.IsAny<List<int>>(), true)).Returns(mpEventDtos);

            // Act
            var result = _fixture.GetCheckinEvents(start, end, site, Guid.NewGuid().ToString());
            _eventRepository.VerifyAll();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Oakley", result[0].EventSite);
            Assert.AreEqual(1234567, result[0].EventId);
            Assert.IsNull(result[0].Template);
        }

        [Test]
        public void ShouldGetEventTemplates()
        {
            // Arrange
            var kioskConfig = new MpKioskConfigDto
            {
                KioskTypeId = 1
            };

            var mpEventDtos = new List<MpEventDto>();

            var testMpEventDto = new MpEventDto
            {
                CongregationName = "Oakley",
                EventStartDate = new DateTime(2016, 10, 10),
                EventId = 1234567,
                EventTitle = "Test Event",
                EventType = "Oakley Service",
                Template = true
            };

            mpEventDtos.Add(testMpEventDto);

            const int site = 1;
            _eventRepository.Setup(m => m.GetEventTemplates(site)).Returns(mpEventDtos);

            // Act
            var result = _fixture.GetCheckinEventTemplates(site);
            _eventRepository.VerifyAll();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Oakley", result[0].EventSite);
            Assert.AreEqual(1234567, result[0].EventId);
            Assert.AreEqual(true, result[0].Template);
        }

        [Test]
        public void ShouldOnlyGetEventsEligibleForCheckin()
        {
            // Arrange
            var kioskConfig = new MpKioskConfigDto
            {
                KioskTypeId = 2
            };

            var mpEventDtos = new List<MpEventDto>
            {
                new MpEventDto
                {
                    CongregationName = "Oakley",
                    EventStartDate = new DateTime(2016, 10, 10),
                    EventId = 1234567,
                    EventTitle = "Test Event",
                    EventType = "Oakley Service",
                    EventTypeId = 123
                },
                new MpEventDto
                {
                    CongregationName = "Oakley",
                    EventStartDate = new DateTime(2016, 10, 10),
                    EventId = 2345678,
                    EventTitle = "Test Big Event",
                    EventType = "Big Event (MSM and HSM Combined)",
                    EventTypeId = 369
                },
                new MpEventDto
                {
                    CongregationName = "Oakley",
                    EventStartDate = new DateTime(2016, 10, 10),
                    EventId = 3456789,
                    EventTitle = "Test MSM Event",
                    EventType = "Student Ministry 6 to 8",
                    EventTypeId = 402
                },
                new MpEventDto
                {
                    CongregationName = "Oakley",
                    EventStartDate = new DateTime(2016, 10, 10),
                    EventId = 4567890,
                    EventTitle = "Test HSM Event",
                    EventType = "Student Ministry 9 to 12",
                    EventTypeId = 403
                }
            };

            var start = new DateTime(2016, 10, 9);
            var end = new DateTime(2016, 10, 12);
            const int site = 1;
            _kioskRepository.Setup(m => m.GetMpKioskConfigByIdentifier(It.IsAny<Guid>())).Returns(kioskConfig);
            _eventRepository.Setup(m => m.GetEvents(start, end, site, false, It.IsAny<List<int>>(), true)).Returns(mpEventDtos);

            // Act
            var result = _fixture.GetCheckinEvents(start, end, site, Guid.NewGuid().ToString());

            // Assert
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ShouldGetEventsForAdmin()
        {
            // Arrange
            var kioskConfig = new MpKioskConfigDto
            {
                KioskTypeId = 3
            };

            var mpEventDtos = new List<MpEventDto>
            {
                new MpEventDto
                {
                    CongregationName = "Oakley",
                    EventStartDate = new DateTime(2016, 10, 10),
                    EventId = 1234567,
                    EventTitle = "Test Event",
                    EventType = "Oakley Service",
                    EventTypeId = 123
                },
                new MpEventDto
                {
                    CongregationName = "Oakley",
                    EventStartDate = new DateTime(2016, 10, 10),
                    EventId = 2345678,
                    EventTitle = "Test Big Event",
                    EventType = "Big Event (MSM and HSM Combined)",
                    EventTypeId = 369
                },
                new MpEventDto
                {
                    CongregationName = "Oakley",
                    EventStartDate = new DateTime(2016, 10, 10),
                    EventId = 3456789,
                    EventTitle = "Test MSM Event",
                    EventType = "Student Ministry 6 to 8",
                    EventTypeId = 402
                },
                new MpEventDto
                {
                    CongregationName = "Oakley",
                    EventStartDate = new DateTime(2016, 10, 10),
                    EventId = 4567890,
                    EventTitle = "Test HSM Event",
                    EventType = "Student Ministry 9 to 12",
                    EventTypeId = 403
                }
            };

            var start = new DateTime(2016, 10, 9);
            var end = new DateTime(2016, 10, 12);
            const int site = 1;
            _kioskRepository.Setup(m => m.GetMpKioskConfigByIdentifier(It.IsAny<Guid>())).Returns(kioskConfig);
            _eventRepository.Setup(m => m.GetEvents(start, end, site, false, It.IsAny<List<int>>(), true)).Returns(mpEventDtos);

            // Act
            var result = _fixture.GetCheckinEvents(start, end, site, Guid.NewGuid().ToString());

            // Assert
            Assert.AreEqual(4, result.Count);
        }

        [Test]
        public void TestGetEvent()
        {
            const int eventId = 123;
            var e = new MpEventDto
            {
                EventId = 999
            };

            _eventRepository.Setup(mocked => mocked.GetEventById(eventId)).Returns(e);
            var result = _fixture.GetEvent(eventId);
            _eventRepository.VerifyAll();
            Assert.AreEqual(e.EventId, result.EventId);
        }

        [Test]
        public void TestGetCurrentEventForSite()
        {
            const int siteId = 1;
            const string kioskId = "504c73c1-d664-4ccd-964e-e008e7ce2635";

            var kioskConfig = new MpKioskConfigDto
            {
                KioskTypeId = 4
            };

            var events = new List<MpEventDto>
            {
                new MpEventDto
                {
                    CongregationId = 1,
                    CongregationName = "Oakley",
                    EarlyCheckinPeriod = 30,
                    EventEndDate = DateTime.Now.AddDays(1),
                    EventId = 1234567,
                    EventStartDate = DateTime.Now,
                    EventTitle = "test event",
                    EventType = "type test",
                    LateCheckinPeriod = 30,
                    LocationId = 3
                }
            };

            _kioskRepository.Setup(m => m.GetMpKioskConfigByIdentifier(Guid.Parse(kioskId))).Returns(kioskConfig);

            _eventRepository.Setup(m => m.GetEvents(It.IsAny<DateTime>(), It.IsAny<DateTime>(), siteId, false, It.IsAny<List<int>>(), false)).Returns(events);
            var result = _fixture.GetCurrentEventForSite(siteId, kioskId);
            _eventRepository.VerifyAll();

            Assert.AreEqual(result.EventId, events[0].EventId);
            Assert.AreEqual(result.EarlyCheckinPeriod, events[0].EarlyCheckinPeriod);
            Assert.AreEqual(result.LateCheckinPeriod, events[0].LateCheckinPeriod);
        }

        [Test]
        public void TestCheckEventTimeValidityTrue()
        {
            var eventDto = new EventDto
            {
                EarlyCheckinPeriod = 30,
                EventEndDate = DateTime.Now.AddDays(1),
                EventId = 1234567,
                EventStartDate = DateTime.Now,
                EventTitle = "test event",
                EventType = "type test",
                LateCheckinPeriod = 30
            };

            var result = _fixture.CheckEventTimeValidity(eventDto);
            Assert.AreEqual(result, true);
        }

        [Test]
        public void TestCheckEventTimeValidityFalse()
        {
            var eventDto = new EventDto
            {
                EarlyCheckinPeriod = 30,
                EventEndDate = DateTime.Now.AddDays(2),
                EventId = 1234567,
                EventStartDate = DateTime.Now.AddDays(1),
                EventTitle = "test event",
                EventType = "type test",
                LateCheckinPeriod = 30
            };

            var result = _fixture.CheckEventTimeValidity(eventDto);
            Assert.AreEqual(result, false);
        }

        [Test]
        public void TestImportEventSetup()
        {
            const int destinationEventId = 12345;
            const int sourceEventId = 67890;
            const int locationId = 937;
            const string token = "tok123";
            var destinationEvent = new MpEventDto
            {
                EventId = destinationEventId,
                LocationId = locationId
            };

            var eventRooms = new List<MpEventRoomDto>
            {
                new MpEventRoomDto(),
                new MpEventRoomDto()
            };

            _eventRepository.Setup(mocked => mocked.GetEventById(destinationEventId)).Returns(destinationEvent);
            _eventRepository.Setup(mocked => mocked.ResetEventSetup(destinationEventId));
            _eventRepository.Setup(mocked => mocked.ImportEventSetup(destinationEventId, sourceEventId));
            _roomRepository.Setup(mocked => mocked.GetRoomsForEvent(destinationEventId, locationId)).Returns(eventRooms);

            var response = _fixture.ImportEventSetup(destinationEventId, sourceEventId);
            _eventRepository.VerifyAll();
            _roomRepository.VerifyAll();
            response.Should().NotBeNull();
            response.Count.Should().Be(eventRooms.Count);
        }

        [Test]
        public void TestResetEventSetup()
        {
            const int eventId = 12345;
            const int locationId = 937;
            const string token = "tok123";
            var destinationEvent = new MpEventDto
            {
                EventId = eventId,
                LocationId = locationId
            };

            var eventRooms = new List<MpEventRoomDto>
            {
                new MpEventRoomDto(),
                new MpEventRoomDto()
            };

            _eventRepository.Setup(mocked => mocked.GetEventById(eventId)).Returns(destinationEvent);
            _eventRepository.Setup(mocked => mocked.ResetEventSetup(eventId));
            _roomRepository.Setup(mocked => mocked.GetRoomsForEvent(eventId, locationId)).Returns(eventRooms);

            var response = _fixture.ResetEventSetup(eventId);
            _eventRepository.VerifyAll();
            _roomRepository.VerifyAll();
            response.Should().NotBeNull();
            response.Count.Should().Be(eventRooms.Count);
        }

        [Test]
        public void ItShouldCreateSubevent()
        {
            // Arrange
            var token = "123abc";
            var eventId = 1234567;

            List<MpEventDto> events = new List<MpEventDto>();

            MpEventDto parentEvent = new MpEventDto
            {
                EventId = 1234567
            };

            events.Add(parentEvent);

            MpEventDto childEvent = new MpEventDto
            {
                EventId = 7654321,
                ParentEventId = 234567,
                EventTypeId = 20
            };

            _eventRepository.Setup(m => m.GetEventAndCheckinSubevents(eventId, true)).Returns(events);
            _eventRepository.Setup(m => m.CreateSubEvent(It.IsAny<MpEventDto>())).Returns(childEvent);
            _applicationConfiguation.Setup(m => m.AdventureClubEventTypeId).Returns(20);

            // Act
            var result = _fixture.GetEventMaps(eventId);

            // Assert
            _eventRepository.VerifyAll();
            Assert.AreEqual(result.Count, 2);
        }

        [Test]
        public void ItShouldGetSubevent()
        {
            // Arrange
            var token = "123abc";
            var eventId = 1234567;

            List<MpEventDto> events = new List<MpEventDto>();

            MpEventDto parentEvent = new MpEventDto
            {
                EventId = 1234567
            };

            events.Add(parentEvent);

            MpEventDto childEvent = new MpEventDto
            {
                EventId = 7654321,
                ParentEventId = 1234567,
                EventTypeId = 20
            };

            events.Add(childEvent);

            _eventRepository.Setup(m => m.GetEventAndCheckinSubevents(eventId, true)).Returns(events);
            _applicationConfiguation.Setup(m => m.AdventureClubEventTypeId).Returns(20);

            // Act
            var result = _fixture.GetEventMaps(eventId);

            // Assert
            _eventRepository.VerifyAll();
            Assert.AreEqual(result.Count, 2);
        }

        [Test]
        public void ItShouldGetGetListOfChildrenForEvent()
        {// Arrange
            var token = "123abc";
            var eventId = 1234567;

            var events = new List<MpEventDto>();

            var parentEvent = new MpEventDto
            {
                EventId = 1234567
            };
            events.Add(parentEvent);

            var childEvent = new MpEventDto
            {
                EventId = 7654321,
                ParentEventId = 1234567,
                EventTypeId = 20
            };
            events.Add(childEvent);

            var children = new List<MpEventParticipantDto>
            {
                new MpEventParticipantDto
                {
                    EventId = 1231,
                    ParticipantId = 1,
                    ParticipantStatusId = 1,
                    FirstName = "FirstName1",
                    LastName = "LastName1",
                    CallNumber = "1123",
                    RoomId = 1,
                    RoomName = "Room1",
                    TimeIn = DateTime.Now,
                    HouseholdId = 1,
                    HeadsOfHousehold = new List<MpContactDto>
                    {
                        new MpContactDto
                        {
                            HouseholdId = 1,
                            FirstName = "FirstName3",
                            LastName = "LastName3"
                        },
                        new MpContactDto
                        {
                            HouseholdId = 1,
                            FirstName = "FirstName4",
                            LastName = "LastName4"
                        }
                    }
                },
                new MpEventParticipantDto
                {
                    EventId = 1231,
                    ParticipantId = 2,
                    ParticipantStatusId = 1,
                    FirstName = "FirstName2",
                    LastName = "LastName2",
                    CallNumber = "1124",
                    RoomId = 1,
                    RoomName = "Room1",
                    TimeIn = DateTime.Now,
                    HouseholdId = 2,
                    HeadsOfHousehold = new List<MpContactDto>
                    {
                        new MpContactDto
                        {
                            HouseholdId = 2,
                            FirstName = "FirstName5",
                            LastName = "LastName5"
                        }
                    }
                }
            };
            var searchString = "bob";
            _participantRepository.Setup(m => m.GetChildParticipantsByEvent(It.IsAny<int>(), searchString)).Returns(children);

            var result = _fixture.GetListOfChildrenForEvent(eventId, searchString);

            // Assert
            _eventRepository.VerifyAll();
            _participantRepository.VerifyAll();

            Assert.AreEqual(result.Count, 2);
            Assert.AreEqual(result[0].FirstName, children[0].FirstName);
            Assert.AreEqual(result[0].LastName, children[0].LastName);
            Assert.AreEqual(result[0].HeadsOfHousehold.Count, children[0].HeadsOfHousehold.Count);
            Assert.AreEqual(result[0].HeadsOfHousehold[0].FirstName, children[0].HeadsOfHousehold[0].FirstName);
            Assert.AreEqual(result[0].HeadsOfHousehold[1].FirstName, children[0].HeadsOfHousehold[1].FirstName);

            Assert.AreEqual(result[1].FirstName, children[1].FirstName);
            Assert.AreEqual(result[1].LastName, children[1].LastName);
            Assert.AreEqual(result[1].HeadsOfHousehold.Count, children[1].HeadsOfHousehold.Count);
            Assert.AreEqual(result[1].HeadsOfHousehold[0].FirstName, children[1].HeadsOfHousehold[0].FirstName);

        }

        [Test]
        public void ItShouldGetFamiliesBySearch()
        {
            // Arrange
            var token = "123abc";
            var search = "dust";

            var contacts = new List<MpContactDto>
            {
                new MpContactDto
                {
                    FirstName = "FirstName1",
                    LastName = "LastName1",
                    HouseholdId = 1
                },
                new MpContactDto
                {
                    FirstName = "FirstName2",
                    LastName = "LastName2",
                    HouseholdId = 2
                }
            };

            _participantRepository.Setup(m => m.GetFamiliesForSearch(search)).Returns(contacts);

            var result = _fixture.GetFamiliesForSearch(search);

            // Assert
            _participantRepository.VerifyAll();

            Assert.AreEqual(result.Count, 2);
            Assert.AreEqual(result[0].FirstName, contacts[0].FirstName);
            Assert.AreEqual(result[0].LastName, contacts[0].LastName);
            Assert.AreEqual(result[0].HouseholdId, contacts[0].HouseholdId);

            Assert.AreEqual(result[1].FirstName, contacts[1].FirstName);
            Assert.AreEqual(result[1].LastName, contacts[1].LastName);
            Assert.AreEqual(result[1].HouseholdId, contacts[1].HouseholdId);
        }

        [Test]
        public void ItShouldGetHouseholdByHouseholdId()
        {
            // Arrange
            var token = "123abc";
            var householdId = 123;

            var household = new MpHouseholdDto()
            {
                HouseholdId = 123,
                HouseholdName = "Test"
            };

            _participantRepository.Setup(m => m.GetHouseholdByHouseholdId(householdId)).Returns(household);

            var result = _fixture.GetHouseholdByHouseholdId(householdId);

            // Assert
            Assert.AreEqual(result.HouseholdId, household.HouseholdId);
            Assert.AreEqual(result.HouseholdName, household.HouseholdName);
        }

        [Test]
        public void ItShouldUpdateHouseholdInformation()
        {
            // Arrange
            var token = "123abc";

            var householdDto = new HouseholdDto
            {
                HouseholdId = 123,
                HouseholdName = "TestUser1"
            };

            _participantRepository.Setup(m => m.UpdateHouseholdInformation(It.IsAny<MpHouseholdDto>()));

            _fixture.UpdateHouseholdInformation(householdDto);

            // Assert
            _participantRepository.VerifyAll();
        }

        [Test]
        public void ShouldGetCapacityBySite()
        {
            // Arrange
            var siteId = 1;

            var eventsList = new List<MpEventDto>
            {
                new MpEventDto
                {
                    EventId = 1234567,
                    LocationId = 1,
                    EventStartDate = System.DateTime.Now
                }
            };

            var capacitiesList = new List<MpCapacityDto>
            {
                new MpCapacityDto
                {
                    CapacityKey = "NURSERY",
                    CurrentParticipants = 5,
                    MaxCapacity = 20
                }
            };



            _eventRepository.Setup(m => m.GetEvents(It.IsAny<DateTime>(), It.IsAny<DateTime>(), siteId, false, It.IsAny<List<int>>(), true)).Returns(eventsList);
            _eventRepository.Setup(m => m.GetCapacitiesForEvent(1234567)).Returns(capacitiesList);

            // Act
            var result = _fixture.GetCapacityBySite(siteId);

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
