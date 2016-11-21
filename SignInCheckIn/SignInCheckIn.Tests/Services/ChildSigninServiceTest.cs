using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Components.DictionaryAdapter;
using Crossroads.Utilities.Services.Interfaces;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using NUnit.Framework;
using Printing.Utilities.Models;
using Printing.Utilities.Services.Interfaces;
using SignInCheckIn.App_Start;
using SignInCheckIn.Models.DTO;
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

            _childSigninRepository = new Mock<IChildSigninRepository>(MockBehavior.Strict);
            _eventRepository = new Mock<IEventRepository>(MockBehavior.Strict);
            _groupRepository = new Mock<IGroupRepository>(MockBehavior.Strict);
            _eventService = new Mock<IEventService>(MockBehavior.Strict);
            _pdfEditor = new Mock<IPdfEditor>(MockBehavior.Strict);
            _printingService = new Mock<IPrintingService>(MockBehavior.Strict);
            _contactRepository = new Mock<IContactRepository>(MockBehavior.Strict);
            _kioskRepository = new Mock<IKioskRepository>(MockBehavior.Strict);

            _fixture = new ChildSigninService(_childSigninRepository.Object,_eventRepository.Object, 
                _groupRepository.Object, _eventService.Object, _pdfEditor.Object, _printingService.Object,
                _contactRepository.Object, _kioskRepository.Object);
        }

        [Test]
        public void ShouldGetChildrenByPhoneNumber()
        {
            const int siteId = 1;
            const string phoneNumber = "812-812-8877";
            int? primaryHouseholdId = 123;

            var eventDto = new EventDto();
 
            var mpParticipantDto = new List<MpParticipantDto>
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

            List<MpContactDto> contactDtos = new List<MpContactDto>();
    
            _childSigninRepository.Setup(mocked => mocked.GetHouseholdIdByPhoneNumber(phoneNumber)).Returns(primaryHouseholdId.Value);
            _childSigninRepository.Setup(m => m.GetChildrenByHouseholdId(It.IsAny<int?>(), It.IsAny<MpEventDto>())).Returns(mpParticipantDto);
            _contactRepository.Setup(m => m.GetHeadsOfHouseholdByHouseholdId(It.IsAny<int>())).Returns(contactDtos);
            _eventService.Setup(m => m.GetCurrentEventForSite(siteId)).Returns(eventDto);
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
            const int siteId = 1;
            const string phoneNumber = "812-812-8877";
            int? householdId = 1234567;

            var mpParticipantDto = new List<MpParticipantDto>();
            var eventDto = new EventDto();
            List<MpContactDto> contactDtos = new List<MpContactDto>();

            _childSigninRepository.Setup(m => m.GetHouseholdIdByPhoneNumber(phoneNumber)).Returns(householdId);
            _childSigninRepository.Setup(m => m.GetChildrenByHouseholdId(householdId, It.IsAny<MpEventDto>())).Returns(mpParticipantDto);
            _contactRepository.Setup(m => m.GetHeadsOfHouseholdByHouseholdId(It.IsAny<int>())).Returns(contactDtos);
            _eventService.Setup(m => m.GetCurrentEventForSite(siteId)).Returns(eventDto);
            var result = _fixture.GetChildrenAndEventByPhoneNumber(phoneNumber, siteId);
            _childSigninRepository.VerifyAll();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.Participants.Any());
        }

        [Test]
        public void ShouldPrintLabelsForAllParticipants()
        {
            // Arrange
            Guid kioskId = Guid.Parse("1a11a1a1-a11a-1a1a-11a1-a111a111a11a");

            MpKioskConfigDto mpKioskConfigDto = new MpKioskConfigDto
            {
                KioskIdentifier = kioskId,
                CongregationId = 1,
                PrinterMapId = 1111111
            };

            _kioskRepository.Setup(m => m.GetMpKioskConfigByIdentifier(It.IsAny<Guid>())).Returns(mpKioskConfigDto);

            MpPrinterMapDto mpPrinterMapDto = new MpPrinterMapDto
            {
                PrinterMapId = 1111111
            };

            _kioskRepository.Setup(m => m.GetPrinterMapById(mpKioskConfigDto.PrinterMapId.GetValueOrDefault())).Returns(mpPrinterMapDto);

            List<ParticipantDto> participantDtos = new List<ParticipantDto>
            {
                new ParticipantDto
                {
                    FirstName = "Child1First",
                    AssignedRoomId = 1234567,
                    AssignedRoomName = "TestRoom",
                    AssignedSecondaryRoomId = 2345678,
                    AssignedSecondaryRoomName = "TestSecondaryRoom",
                    ParticipantId = 111,
                    Selected = true
                }
            };

            List<ContactDto> contactDtos = new List<ContactDto>
            {
                new ContactDto
                {
                    ContactId = 1234567,
                    LastName = "TestLast",
                    Nickname = "TestNickname"
                }
            };

            EventDto eventDto = new EventDto
            {
                EventTitle = "test event"
            };

            ParticipantEventMapDto participantEventMapDto = new ParticipantEventMapDto();
            participantEventMapDto.Participants = participantDtos;
            participantEventMapDto.Contacts = contactDtos;
            participantEventMapDto.CurrentEvent = eventDto;

            string base64pdf = "aaa";
            _pdfEditor.Setup(m => m.PopulatePdfMergeFields(It.IsAny<Byte[]>(), It.IsAny<Dictionary<string, string>>())).Returns(base64pdf);

            _printingService.Setup(m => m.SendPrintRequest(It.IsAny<PrintRequestDto>())).Returns(1234567);

            // Act
            _fixture.PrintParticipants(participantEventMapDto, kioskId.ToString());

            // Assert
            _kioskRepository.VerifyAll();
            _pdfEditor.VerifyAll();
            _printingService.VerifyAll();
        }

        [Test]
        public void ShouldPrintLabelsForNoParticipants()
        {
            // Arrange
            Guid kioskId = Guid.Parse("1a11a1a1-a11a-1a1a-11a1-a111a111a11a");

            MpKioskConfigDto mpKioskConfigDto = new MpKioskConfigDto
            {
                KioskIdentifier = kioskId,
                CongregationId = 1,
                PrinterMapId = 1111111
            };

            _kioskRepository.Setup(m => m.GetMpKioskConfigByIdentifier(It.IsAny<Guid>())).Returns(mpKioskConfigDto);

            MpPrinterMapDto mpPrinterMapDto = new MpPrinterMapDto
            {
                PrinterMapId = 1111111
            };

            _kioskRepository.Setup(m => m.GetPrinterMapById(mpKioskConfigDto.PrinterMapId.GetValueOrDefault())).Returns(mpPrinterMapDto);

            List<ParticipantDto> participantDtos = new List<ParticipantDto>
            {
                new ParticipantDto
                {
                    FirstName = "Child1First",
                    AssignedRoomId = 1234567,
                    AssignedRoomName = "TestRoom",
                    AssignedSecondaryRoomId = 2345678,
                    AssignedSecondaryRoomName = "TestSecondaryRoom",
                    ParticipantId = 111,
                    Selected = false
                }
            };

            List<ContactDto> contactDtos = new List<ContactDto>
            {
                new ContactDto
                {
                    ContactId = 1234567,
                    LastName = "TestLast",
                    Nickname = "TestNickname"
                }
            };

            EventDto eventDto = new EventDto
            {
                EventTitle = "test event"
            };

            ParticipantEventMapDto participantEventMapDto = new ParticipantEventMapDto();
            participantEventMapDto.Participants = participantDtos;
            participantEventMapDto.Contacts = contactDtos;
            participantEventMapDto.CurrentEvent = eventDto;

            // Act
            _fixture.PrintParticipants(participantEventMapDto, kioskId.ToString());

            // Assert
            _kioskRepository.VerifyAll();
            _pdfEditor.VerifyAll();
            _printingService.VerifyAll();
        }

        [Test]
        public void ShouldPrintLabelsForSomeParticipants()
        {
            // Arrange
            Guid kioskId = Guid.Parse("1a11a1a1-a11a-1a1a-11a1-a111a111a11a");

            MpKioskConfigDto mpKioskConfigDto = new MpKioskConfigDto
            {
                KioskIdentifier = kioskId,
                CongregationId = 1,
                PrinterMapId = 1111111
            };

            _kioskRepository.Setup(m => m.GetMpKioskConfigByIdentifier(It.IsAny<Guid>())).Returns(mpKioskConfigDto);

            MpPrinterMapDto mpPrinterMapDto = new MpPrinterMapDto
            {
                PrinterMapId = 1111111
            };

            _kioskRepository.Setup(m => m.GetPrinterMapById(mpKioskConfigDto.PrinterMapId.GetValueOrDefault())).Returns(mpPrinterMapDto);

            List<ParticipantDto> participantDtos = new List<ParticipantDto>
            {
                new ParticipantDto
                {
                    FirstName = "Child1First",
                    AssignedRoomId = 1234567,
                    AssignedRoomName = "TestRoom",
                    AssignedSecondaryRoomId = 2345678,
                    AssignedSecondaryRoomName = "TestSecondaryRoom",
                    ParticipantId = 111,
                    Selected = true
                },
                new ParticipantDto
                {
                    FirstName = "Child2First",
                    AssignedRoomId = null,
                    AssignedSecondaryRoomId = 2345678,
                    AssignedSecondaryRoomName = "TestSecondaryRoom",
                    ParticipantId = 222,
                    Selected = true
                },
                new ParticipantDto
                {
                    FirstName = "Child3First",
                    AssignedRoomId = null,
                    AssignedSecondaryRoomId = 2345678,
                    AssignedSecondaryRoomName = "TestSecondaryRoom",
                    ParticipantId = 333,
                    Selected = false
                }
            };

            List<ContactDto> contactDtos = new List<ContactDto>
            {
                new ContactDto
                {
                    ContactId = 1234567,
                    LastName = "TestLast",
                    Nickname = "TestNickname"
                }
            };

            EventDto eventDto = new EventDto
            {
                EventTitle = "test event"
            };

            ParticipantEventMapDto participantEventMapDto = new ParticipantEventMapDto();
            participantEventMapDto.Participants = participantDtos;
            participantEventMapDto.Contacts = contactDtos;
            participantEventMapDto.CurrentEvent = eventDto;

            string successLabel = "aaa";
            _pdfEditor.Setup(m => m.PopulatePdfMergeFields(It.IsAny<Byte[]>(), It.IsAny<Dictionary<string, string>>())).Returns(successLabel);

            _printingService.Setup(m => m.SendPrintRequest(It.IsAny<PrintRequestDto>())).Returns(1234567);

            // Act
            _fixture.PrintParticipants(participantEventMapDto, kioskId.ToString());

            // Assert
            _kioskRepository.VerifyAll();
            _pdfEditor.VerifyAll();
            _printingService.VerifyAll();
        } 
    }
}
