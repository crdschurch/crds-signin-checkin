using System;
using System.Collections.Generic;
using System.Linq;
using Crossroads.Utilities.Services.Interfaces;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using NUnit.Framework;
using Printing.Utilities.Services.Interfaces;
using SignInCheckIn.App_Start;
using SignInCheckIn.Services;
using SignInCheckIn.Services.Interfaces;

namespace SignInCheckIn.Tests.Services
{
    public class ChildSigninServiceTest
    {
        private Mock<IChildSigninRepository> _childSigninRepository;
        private Mock<IEventRepository> _eventRepository;
        private Mock<IGroupRepository> _groupRepository;
        private Mock<IEventService> _eventService;
        private Mock<IPdfEditor> _pdfEditor;
        private Mock<IPrintingService> _printingService;
        private Mock<IContactRepository> _contactRepository;
        private Mock<IKioskRepository> _kioskRepository;

        private ChildSigninService _fixture;

        [SetUp]
        public void SetUp()
        {
            AutoMapperConfig.RegisterMappings();

            _childSigninRepository = new Mock<IChildSigninRepository>();
            _eventRepository = new Mock<IEventRepository>();
            _groupRepository = new Mock<IGroupRepository>();
            _eventService = new Mock<IEventService>();
            _pdfEditor = new Mock<IPdfEditor>();
            _printingService = new Mock<IPrintingService>();
            _contactRepository = new Mock<IContactRepository>();
            _kioskRepository = new Mock<IKioskRepository>();

            _fixture = new ChildSigninService(_childSigninRepository.Object,_eventRepository.Object, 
                _groupRepository.Object, _eventService.Object, _pdfEditor.Object, _printingService.Object,
                _contactRepository.Object, _kioskRepository.Object);
        }

        [Test]
        public void ShouldGetChildrenByPhoneNumber()
        {
            var siteId = 1;
            var phoneNumber = "812-812-8877";
            int? primaryHouseholdId = 123;

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

            List<MpParticipantDto> mpParticipantDto = new List<MpParticipantDto>
            {
                new MpParticipantDto
                {
                    ParticipantId = 12,
                    ContactId = 1443,
                    HouseholdId = primaryHouseholdId.GetValueOrDefault(),
                    HouseholdPositionId = 2,
                    FirstName = "First1",
                    LastName = "Last1",
                    DateOfBirth = new DateTime()
                }
            };

            _childSigninRepository.Setup(m => m.GetChildrenByHouseholdId(It.IsAny<int?>())).Returns(mpParticipantDto);
            _eventRepository.Setup(m => m.GetEvents(It.IsAny<DateTime>(), It.IsAny<DateTime>(), siteId)).Returns(events);
            _childSigninRepository.Setup(m => m.GetChildrenByPhoneNumber(phoneNumber, It.IsAny<MpEventDto>())).Returns(mpParticipantDto);
            var result = _fixture.GetChildrenAndEventByPhoneNumber(phoneNumber, siteId);
            _childSigninRepository.VerifyAll();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(mpParticipantDto[0].ParticipantId, result.Participants[0].ParticipantId);
            Assert.AreEqual(mpParticipantDto[0].ContactId, result.Participants[0].ContactId);
        }

        [Test]
        public void ShouldNotGetChildrenByPhoneNumber()
        {
            var siteId = 1;
            var phoneNumber = "812-812-8877";
            int? householdId = 1234567;

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

            List<MpParticipantDto> mpParticipantDto = new List<MpParticipantDto>();

            _eventRepository.Setup(m => m.GetEvents(It.IsAny<DateTime>(), It.IsAny<DateTime>(), siteId)).Returns(events);
            _childSigninRepository.Setup(m => m.GetHouseholdIdByPhoneNumber(phoneNumber)).Returns(householdId);
            _childSigninRepository.Setup(m => m.GetChildrenByHouseholdId(householdId)).Returns(mpParticipantDto);

            var result = _fixture.GetChildrenAndEventByPhoneNumber(phoneNumber, siteId);
            _childSigninRepository.VerifyAll();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.Participants.Any());
        }
    }
}
