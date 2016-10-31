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

        private EventService _fixture;

        [SetUp]
        public void SetUp()
        {
            AutoMapperConfig.RegisterMappings();

            _eventRepository = new Mock<IEventRepository>();
            _fixture = new EventService(_eventRepository.Object);
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
    }
}
