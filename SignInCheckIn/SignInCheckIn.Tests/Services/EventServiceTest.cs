using System;
using System.Collections.Generic;
using AutoMapper;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using NUnit.Framework;
using SignInCheckIn.App_Start;
using SignInCheckIn.Services;
using SignInCheckIn.Models.DTO;

namespace SignInCheckIn.Tests.Services
{
    public class EventServiceTest
    {
        private Mock<IEventRepository> _eventRepository;
        private Mock<IConfigRepository> _configRepository;

        private EventService _fixture;

        [SetUp]
        public void SetUp()
        {
            AutoMapperConfig.RegisterMappings();

            _eventRepository = new Mock<IEventRepository>();
            _configRepository = new Mock<IConfigRepository>();

            MpConfigDto mpConfigDtoEarly = new MpConfigDto
            {
                ApplicationCode = "COMMON",
                ConfigurationSettingId = 1,
                KeyName = "DefaultEarlyCheckIn",
                Value = "60"
            };

            MpConfigDto mpConfigDtoLate = new MpConfigDto
            {
                ApplicationCode = "COMMON",
                ConfigurationSettingId = 1,
                KeyName = "DefaultLateCheckIn",
                Value = "60"
            };

            _configRepository.Setup(m => m.GetMpConfigByKey("DefaultEarlyCheckIn")).Returns(mpConfigDtoEarly);
            _configRepository.Setup(m => m.GetMpConfigByKey("DefaultLateCheckIn")).Returns(mpConfigDtoLate);

            _fixture = new EventService(_eventRepository.Object, _configRepository.Object);
        }

        [Test]
        public void ShouldGetEvents()
        {
            // Arrange
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
            _eventRepository.Setup(m => m.GetEvents(start, end, site)).Returns(mpEventDtos);

            // Act
            var result = _fixture.GetCheckinEvents(start, end, site);
            _eventRepository.VerifyAll();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Oakley", result[0].EventSite);
            Assert.AreEqual(1234567, result[0].EventId);
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
            var siteId = 1;
            List<MpEventDto> events = new List<MpEventDto>
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

            _eventRepository.Setup(m => m.GetEvents(It.IsAny<DateTime>(), It.IsAny<DateTime>(), siteId)).Returns(events);
            var result = _fixture.GetCurrentEventForSite(siteId);
            _eventRepository.VerifyAll();

            Assert.AreEqual(result.EventId, events[0].EventId);
            Assert.AreEqual(result.EarlyCheckinPeriod, events[0].EarlyCheckinPeriod);
            Assert.AreEqual(result.LateCheckinPeriod, events[0].LateCheckinPeriod);
        }

        [Test]
        public void TestCheckEventTimeValidityTrue()
        {
            EventDto eventDto = new EventDto
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
            EventDto eventDto = new EventDto
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
    }
}
