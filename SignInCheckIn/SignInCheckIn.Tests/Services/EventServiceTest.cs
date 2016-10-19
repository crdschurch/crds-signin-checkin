using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using NUnit.Framework;
using SignInCheckIn.App_Start;
using SignInCheckIn.Services;
using SignInCheckIn.Services.Interfaces;

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
            List<MpEventDto> mpEventDtos = new List<MpEventDto>();

            MpEventDto testMpEventDto = new MpEventDto
            {
                CongregationName = "Oakley",
                EventStartDate = new DateTime(2016, 10, 10),
                EventId = 1234567,
                EventTitle = "Test Event",
                EventType = "Oakley Service"
            };

            mpEventDtos.Add(testMpEventDto);

            DateTime start = new DateTime(2016, 10, 9);
            DateTime end = new DateTime(2016, 10, 12);
            int site = 1;
            _eventRepository.Setup(m => m.GetEvents(start, end, site)).Returns(mpEventDtos);

            // Act
            var result = _fixture.GetCheckinEvents(start, end, site);
            _eventRepository.VerifyAll();

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
