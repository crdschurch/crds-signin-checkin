using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
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
        private Mock<IParticipantRepository> _participantRepository;
        private Mock<IApplicationConfiguration> _applicationConfiguration;
        private Mock<IGroupLookupRepository> _groupLookupRepository;
        private Mock<IRoomRepository> _roomRepository;
        private Mock<IConfigRepository> _configRepository;

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
            _participantRepository = new Mock<IParticipantRepository>(MockBehavior.Strict);
            _applicationConfiguration = new Mock<IApplicationConfiguration>();
            _groupLookupRepository = new Mock<IGroupLookupRepository>();
            _roomRepository = new Mock<IRoomRepository>();
            _configRepository = new Mock<IConfigRepository>();

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

            _fixture = new ChildSigninService(_childSigninRepository.Object,_eventRepository.Object, 
                _groupRepository.Object, _eventService.Object, _pdfEditor.Object, _printingService.Object,
                _contactRepository.Object, _kioskRepository.Object, _participantRepository.Object,
                _applicationConfiguration.Object, _groupLookupRepository.Object, _roomRepository.Object,
                _configRepository.Object);
        }

        [Test]
        public void ShouldGetChildrenByPhoneNumber()
        {
            const int siteId = 1;
            const string phoneNumber = "812-812-8877";
            int? primaryHouseholdId = 123;

            var eventDto = new EventDto();

            var mpHouseholdAndParticipants = new MpHouseholdParticipantsDto
            {
                HouseholdId = primaryHouseholdId.GetValueOrDefault(),
                Participants = new List<MpParticipantDto>
                {
                    new MpParticipantDto
                    {
                        ParticipantId = 12,
                        ContactId = 1443,
                        HouseholdId = primaryHouseholdId.GetValueOrDefault(),
                        HouseholdPositionId = 2,
                        FirstName = "First1",
                        LastName = "Last1",
                        DateOfBirth = new DateTime(),
                        PrimaryHousehold = true
                    }
                }
            };
 
            var contactDtos = new List<MpContactDto>();

            _childSigninRepository.Setup(mocked => mocked.GetChildrenByPhoneNumber(phoneNumber, true)).Returns(mpHouseholdAndParticipants);
            _contactRepository.Setup(m => m.GetHeadsOfHouseholdByHouseholdId(It.IsAny<int>())).Returns(contactDtos);
            _eventService.Setup(m => m.GetCurrentEventForSite(siteId)).Returns(eventDto);
            var result = _fixture.GetChildrenAndEventByPhoneNumber(phoneNumber, siteId, null);
            _childSigninRepository.VerifyAll();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(mpHouseholdAndParticipants.Participants[0].ParticipantId, result.Participants[0].ParticipantId);
            Assert.AreEqual(mpHouseholdAndParticipants.Participants[0].ContactId, result.Participants[0].ContactId);
        }

        [Test]
        public void GetChildrenByPhoneNumberShouldReturnNoParticipants()
        {
            const int siteId = 1;
            const string phoneNumber = "812-812-8877";
            int? primaryHouseholdId = 123;

            var mpHouseholdAndParticipants = new MpHouseholdParticipantsDto
            {
                HouseholdId = primaryHouseholdId.GetValueOrDefault(),
            };
            var eventDto = new EventDto();
            var contactDtos = new List<MpContactDto>();

            _childSigninRepository.Setup(mocked => mocked.GetChildrenByPhoneNumber(phoneNumber, true)).Returns(mpHouseholdAndParticipants);
            _contactRepository.Setup(m => m.GetHeadsOfHouseholdByHouseholdId(primaryHouseholdId.Value)).Returns(contactDtos);
            _eventService.Setup(m => m.GetCurrentEventForSite(siteId)).Returns(eventDto);
            var result = _fixture.GetChildrenAndEventByPhoneNumber(phoneNumber, siteId, null);
            _childSigninRepository.VerifyAll();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.Participants.Any());
        }

        public void GetChildrenByPhoneNumberShouldThrowExceptionIfHouseholdNotFound()
        {
            const int siteId = 1;
            const string phoneNumber = "812-812-8877";

            var mpHouseholdAndParticipants = new MpHouseholdParticipantsDto();

            _childSigninRepository.Setup(mocked => mocked.GetChildrenByPhoneNumber(phoneNumber, true)).Returns(mpHouseholdAndParticipants);
            try
            {
                _fixture.GetChildrenAndEventByPhoneNumber(phoneNumber, siteId, null);
                Assert.Fail("Expected exception was not thrown");
            }
            catch (ApplicationException e)
            {
                _childSigninRepository.VerifyAll();
                Assert.AreEqual($"Could not locate household for phone number {phoneNumber}", e.Message);
            }
        }

        [Test]
        public void ShouldSignInParticipants()
        {
            // Arrange
            var participantDtos = new List<ParticipantDto>
            {
                new ParticipantDto
                {
                    FirstName = "Child1First",
                    AssignedRoomId = 1234567,
                    AssignedRoomName = "TestRoom",
                    AssignedSecondaryRoomId = 2345678,
                    AssignedSecondaryRoomName = "TestSecondaryRoom",
                    ParticipantId = 111,
                    Selected = true,
                    DuplicateSignIn = false
                }
            };

            var contactDtos = new List<ContactDto>
            {
                new ContactDto
                {
                    ContactId = 1234567,
                    LastName = "TestLast",
                    Nickname = "TestNickname"
                }
            };

            var eventDto = new EventDto
            {
                EventTitle = "test event",
                EventId = 321,
                EventSiteId = 8
            };

            var mpEventGroupDtos = new List<MpEventGroupDto>
            {
                new MpEventGroupDto
                {
                    RoomReservation = new MpEventRoomDto
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
                    }
                }
            };

            var mpEventParticipantDtos = new List<MpEventParticipantDto>
            {
                new MpEventParticipantDto
                {
                    GroupId = 432,
                    RoomId = 4
                }
            };

            MpEventDto currentMpServiceEventDto = new MpEventDto
            {
                EventId = 321,
                ParentEventId = null,
                CongregationId = 8,
                EventTypeId = 123,
                EventStartDate = DateTime.Now
            };

            // current service event, current ac event, trailing service event
            List<MpEventDto> eventDtosBySite = new List<MpEventDto>()
            {
                currentMpServiceEventDto
            };

            _eventRepository.Setup(m => m.GetEvents(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), true)).Returns(eventDtosBySite);

            var participantEventMapDto = new ParticipantEventMapDto();
            participantEventMapDto.Participants = participantDtos;
            participantEventMapDto.Contacts = contactDtos;
            participantEventMapDto.CurrentEvent = eventDto;

            _eventService.Setup(m => m.GetEvent(eventDto.EventId)).Returns(participantEventMapDto.CurrentEvent);
            _eventService.Setup(m => m.CheckEventTimeValidity(participantEventMapDto.CurrentEvent)).Returns(true);
            _eventRepository.Setup(m => m.GetEventGroupsForEvent(participantEventMapDto.CurrentEvent.EventId)).Returns(mpEventGroupDtos);
            _groupRepository.Setup(m => m.GetGroup(null, 2, false)).Returns((MpGroupDto)null);
            _childSigninRepository.Setup(m => m.CreateEventParticipants(It.IsAny<List<MpEventParticipantDto>>())).Returns(mpEventParticipantDtos);
            _participantRepository.Setup(m => m.UpdateEventParticipants(It.IsAny<List<MpEventParticipantDto>>()));
            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(It.IsAny<int>(), It.IsAny<List<int>>())).Returns(new List<MpEventParticipantDto>());

            // Act
            var response = _fixture.SigninParticipants(participantEventMapDto);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsNull(response.Participants[0].SignInErrorMessage);
        }
        
        [Test]
        public void ShouldSignInParticipantsViaBumpingRules()
        {
            // Arrange
            var participantDtos = new List<ParticipantDto>
            {
                new ParticipantDto
                {
                    FirstName = "Child1First",
                    ParticipantId = 111,
                    Selected = true,
                    GroupId = 432
                }
            };

            var contactDtos = new List<ContactDto>
            {
                new ContactDto
                {
                    ContactId = 1234567,
                    LastName = "TestLast",
                    Nickname = "TestNickname"
                }
            };

            var eventDto = new EventDto
            {
                EventTitle = "test event",
                EventId = 321
            };

            var mpEventGroupDtos = new List<MpEventGroupDto>
            {
                new MpEventGroupDto
                {
                    GroupId = 432,
                    RoomReservation = new MpEventRoomDto
                    {
                        AllowSignIn = true,
                        Capacity = 11,
                        CheckedIn = 9,
                        EventId = 321,
                        EventRoomId = 153234,
                        Hidden = true,
                        RoomId = 4,
                        RoomName = "name",
                        RoomNumber = "number",
                        SignedIn = 2,
                        Volunteers = 6
                    }
                }
            };

            var mpEventParticipantDtos = new List<MpEventParticipantDto>
            {
                new MpEventParticipantDto
                {
                    GroupId = 432,
                    RoomId = 3827
                }
            };

            var participantEventMapDto = new ParticipantEventMapDto
            {
                Participants = participantDtos,
                Contacts = contactDtos,
                CurrentEvent = eventDto
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
                    SignedIn = 93,
                    CheckedIn = 12
                },
                new MpBumpingRoomsDto
                {
                    EventRoomId = 1248,
                    RoomId = 3877727,
                    PriorityOrder = 1,
                    AllowSignIn = false,
                    Capacity = 10,
                    RoomName = "Test Room 2",
                    SignedIn = 9,
                    CheckedIn = 0
                },
                new MpBumpingRoomsDto
                {
                    EventRoomId = 1248,
                    RoomId = 511,
                    PriorityOrder = 4,
                    AllowSignIn = false,
                    Capacity = 10,
                    RoomName = "Test Room 2",
                    SignedIn = 9,
                    CheckedIn = 0
                },
                new MpBumpingRoomsDto
                {
                    EventRoomId = 1248,
                    RoomId = 3827,
                    PriorityOrder = 3,
                    AllowSignIn = true,
                    Capacity = 10,
                    RoomName = "Test Room 2",
                    SignedIn = 9,
                    CheckedIn = 0
                }
            };

            //DateTime currentMpEventDtoStartTime = new DateTime(2016, 12, 1, 9, 0, 0);

            MpEventDto currentMpServiceEventDto = new MpEventDto
            {
                EventId = 321,
                ParentEventId = null,
                CongregationId = 8,
                EventTypeId = 123,
                EventStartDate = DateTime.Now
            };

            // current service event, current ac event, trailing service event
            List<MpEventDto> eventDtosBySite = new List<MpEventDto>()
            {
                currentMpServiceEventDto
            };

            _eventRepository.Setup(m => m.GetEvents(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), true)).Returns(eventDtosBySite);

            _eventService.Setup(m => m.GetEvent(eventDto.EventId)).Returns(participantEventMapDto.CurrentEvent);
            _eventService.Setup(m => m.CheckEventTimeValidity(participantEventMapDto.CurrentEvent)).Returns(true);
            _eventRepository.Setup(m => m.GetEventGroupsForEvent(participantEventMapDto.CurrentEvent.EventId)).Returns(mpEventGroupDtos);
            _groupRepository.Setup(m => m.GetGroup(null, 2, false)).Returns((MpGroupDto)null);
            _roomRepository.Setup(m => m.GetBumpingRoomsForEventRoom(321, 153234)).Returns(mpBumpingRooms);
            _childSigninRepository.Setup(m => m.CreateEventParticipants(It.IsAny<List<MpEventParticipantDto>>())).Returns(mpEventParticipantDtos);
            _participantRepository.Setup(m => m.UpdateEventParticipants(It.IsAny<List<MpEventParticipantDto>>()));
            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(It.IsAny<int>(), It.IsAny<List<int>>())).Returns(new List<MpEventParticipantDto>());

            // Act
            var response = _fixture.SigninParticipants(participantEventMapDto);

            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(response.Participants[0].AssignedRoomId, 3827);
        }

        [Test]
        public void ShouldNotSignInParticipantsDueToCapacity()
        {
            // Arrange
            var participantDtos = new List<ParticipantDto>
            {
                new ParticipantDto
                {
                    FirstName = "Child1First",
                    ParticipantId = 111,
                    Selected = true,
                    GroupId = 432
                }
            };

            var contactDtos = new List<ContactDto>
            {
                new ContactDto
                {
                    ContactId = 1234567,
                    LastName = "TestLast",
                    Nickname = "TestNickname"
                }
            };

            var eventDto = new EventDto
            {
                EventTitle = "test event",
                EventId = 321
            };

            var mpEventGroupDtos = new List<MpEventGroupDto>
            {
                new MpEventGroupDto
                {
                    GroupId = 432,
                    RoomReservation = new MpEventRoomDto
                    {
                        AllowSignIn = true,
                        Capacity = 11,
                        CheckedIn = 9,
                        EventId = 321,
                        EventRoomId = 153234,
                        Hidden = true,
                        RoomId = 4,
                        RoomName = "name",
                        RoomNumber = "number",
                        SignedIn = 2,
                        Volunteers = 6
                    }
                }
            };

            var mpEventParticipantDtos = new List<MpEventParticipantDto>();

            var participantEventMapDto = new ParticipantEventMapDto
            {
                Participants = participantDtos,
                Contacts = contactDtos,
                CurrentEvent = eventDto
            };

            //DateTime currentMpEventDtoStartTime = new DateTime(2016, 12, 1, 9, 0, 0);

            MpEventDto currentMpServiceEventDto = new MpEventDto
            {
                EventId = 321,
                ParentEventId = null,
                CongregationId = 8,
                EventTypeId = 123,
                EventStartDate = DateTime.Now
            };

            // current service event, current ac event, trailing service event
            List<MpEventDto> eventDtosBySite = new List<MpEventDto>()
            {
                currentMpServiceEventDto
            };

            _eventRepository.Setup(m => m.GetEvents(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), true)).Returns(eventDtosBySite);

            _eventService.Setup(m => m.GetEvent(eventDto.EventId)).Returns(participantEventMapDto.CurrentEvent);
            _eventService.Setup(m => m.CheckEventTimeValidity(participantEventMapDto.CurrentEvent)).Returns(true);
            _eventRepository.Setup(m => m.GetEventGroupsForEvent(participantEventMapDto.CurrentEvent.EventId)).Returns(mpEventGroupDtos);
            _groupRepository.Setup(m => m.GetGroup(null, 2, false)).Returns((MpGroupDto)null);
            _roomRepository.Setup(m => m.GetBumpingRoomsForEventRoom(321, 153234)).Returns(new List<MpBumpingRoomsDto>());
            _childSigninRepository.Setup(m => m.CreateEventParticipants(It.IsAny<List<MpEventParticipantDto>>())).Returns(mpEventParticipantDtos);
            _participantRepository.Setup(m => m.UpdateEventParticipants(It.IsAny<List<MpEventParticipantDto>>()));
            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(It.IsAny<int>(), It.IsAny<List<int>>())).Returns(new List<MpEventParticipantDto>());

            // Act
            var response = _fixture.SigninParticipants(participantEventMapDto);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsNull(response.Participants[0].AssignedRoomId);
        }

        [Test]
        public void ShouldPrintLabelsForAllParticipants()
        {
            // Arrange
            var kioskId = Guid.Parse("1a11a1a1-a11a-1a1a-11a1-a111a111a11a");

            var mpKioskConfigDto = new MpKioskConfigDto
            {
                KioskIdentifier = kioskId,
                CongregationId = 1,
                PrinterMapId = 1111111
            };

            _kioskRepository.Setup(m => m.GetMpKioskConfigByIdentifier(It.IsAny<Guid>())).Returns(mpKioskConfigDto);

            var mpPrinterMapDto = new MpPrinterMapDto
            {
                PrinterMapId = 1111111
            };

            _kioskRepository.Setup(m => m.GetPrinterMapById(mpKioskConfigDto.PrinterMapId.GetValueOrDefault())).Returns(mpPrinterMapDto);

            var participantDtos = new List<ParticipantDto>
            {
                new ParticipantDto
                {
                    FirstName = "Child1First",
                    AssignedRoomId = 1234567,
                    AssignedRoomName = "TestRoom",
                    AssignedSecondaryRoomId = 2345678,
                    AssignedSecondaryRoomName = "TestSecondaryRoom",
                    ParticipantId = 111,
                    Selected = true,
                    CallNumber = "1234"
                }
            };

            var contactDtos = new List<ContactDto>
            {
                new ContactDto
                {
                    ContactId = 1234567,
                    LastName = "TestLast",
                    Nickname = "TestNickname"
                }
            };

            var eventDto = new EventDto
            {
                EventTitle = "test event"
            };

            var participantEventMapDto = new ParticipantEventMapDto();
            participantEventMapDto.Participants = participantDtos;
            participantEventMapDto.Contacts = contactDtos;
            participantEventMapDto.CurrentEvent = eventDto;

            const string base64Pdf = "aaa";
            _pdfEditor.Setup(m => m.PopulatePdfMergeFields(It.IsAny<Byte[]>(), It.IsAny<Dictionary<string, string>>())).Returns(base64Pdf);

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
            var kioskId = Guid.Parse("1a11a1a1-a11a-1a1a-11a1-a111a111a11a");

            var mpKioskConfigDto = new MpKioskConfigDto
            {
                KioskIdentifier = kioskId,
                CongregationId = 1,
                PrinterMapId = 1111111
            };

            _kioskRepository.Setup(m => m.GetMpKioskConfigByIdentifier(It.IsAny<Guid>())).Returns(mpKioskConfigDto);

            var mpPrinterMapDto = new MpPrinterMapDto
            {
                PrinterMapId = 1111111
            };

            _kioskRepository.Setup(m => m.GetPrinterMapById(mpKioskConfigDto.PrinterMapId.GetValueOrDefault())).Returns(mpPrinterMapDto);

            var participantDtos = new List<ParticipantDto>
            {
                new ParticipantDto
                {
                    FirstName = "Child1First",
                    AssignedRoomId = 1234567,
                    AssignedRoomName = "TestRoom",
                    AssignedSecondaryRoomId = 2345678,
                    AssignedSecondaryRoomName = "TestSecondaryRoom",
                    ParticipantId = 111,
                    Selected = false,
                    CallNumber = "1234"
                }
            };

            var contactDtos = new List<ContactDto>
            {
                new ContactDto
                {
                    ContactId = 1234567,
                    LastName = "TestLast",
                    Nickname = "TestNickname"
                }
            };

            var eventDto = new EventDto
            {
                EventTitle = "test event"
            };

            var participantEventMapDto = new ParticipantEventMapDto
            {
                Participants = participantDtos,
                Contacts = contactDtos,
                CurrentEvent = eventDto
            };

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
            var kioskId = Guid.Parse("1a11a1a1-a11a-1a1a-11a1-a111a111a11a");

            var mpKioskConfigDto = new MpKioskConfigDto
            {
                KioskIdentifier = kioskId,
                CongregationId = 1,
                PrinterMapId = 1111111
            };

            _kioskRepository.Setup(m => m.GetMpKioskConfigByIdentifier(It.IsAny<Guid>())).Returns(mpKioskConfigDto);

            var mpPrinterMapDto = new MpPrinterMapDto
            {
                PrinterMapId = 1111111
            };

            _kioskRepository.Setup(m => m.GetPrinterMapById(mpKioskConfigDto.PrinterMapId.GetValueOrDefault())).Returns(mpPrinterMapDto);

            var participantDtos = new List<ParticipantDto>
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
                },
                new ParticipantDto
                {
                    FirstName = "Child4First",
                    AssignedRoomId = null,
                    AssignedSecondaryRoomId = 2345678,
                    AssignedSecondaryRoomName = "TestSecondaryRoom",
                    ParticipantId = 333,
                    Selected = true,
                    SignInErrorMessage = "testerror"
                }
            };

            var contactDtos = new List<ContactDto>
            {
                new ContactDto
                {
                    ContactId = 1234567,
                    LastName = "TestLast",
                    Nickname = "TestNickname"
                }
            };

            var eventDto = new EventDto
            {
                EventTitle = "test event"
            };

            var participantEventMapDto = new ParticipantEventMapDto
            {
                Participants = participantDtos,
                Contacts = contactDtos,
                CurrentEvent = eventDto
            };

            const string successLabel = "aaa";
            _pdfEditor.Setup(m => m.PopulatePdfMergeFields(It.IsAny<byte[]>(), It.IsAny<Dictionary<string, string>>())).Returns(successLabel);
            _printingService.Setup(m => m.SendPrintRequest(It.IsAny<PrintRequestDto>())).Returns(1234567);

            // Act
            _fixture.PrintParticipants(participantEventMapDto, kioskId.ToString());

            // Assert

            // Verify specific calls, to make sure we populated the right PDFs for the right participants
            _pdfEditor.Verify(
                mocked =>
                    mocked.PopulatePdfMergeFields(Properties.Resources.Checkin_KC_Label, It.Is<Dictionary<string, string>>(d => d["ChildName"].Equals(participantDtos[0].FirstName))));
            _pdfEditor.Verify(
                mocked =>
                    mocked.PopulatePdfMergeFields(Properties.Resources.Activity_Kit_Label, It.Is<Dictionary<string, string>>(d => d["ChildName"].Equals(participantDtos[1].FirstName))));
            _pdfEditor.Verify(
                mocked =>
                    mocked.PopulatePdfMergeFields(Properties.Resources.Error_Label, It.Is<Dictionary<string, string>>(d => d["ChildName"].Equals(participantDtos[3].FirstName))));
            _pdfEditor.VerifyAll();

            // Verify that we called the print service for each expected participant
            _printingService.Verify(mocked => mocked.SendPrintRequest(It.Is<PrintRequestDto>(p => p.title.Contains(participantDtos[0].FirstName))));
            _printingService.Verify(mocked => mocked.SendPrintRequest(It.Is<PrintRequestDto>(p => p.title.Contains(participantDtos[1].FirstName))));
            _printingService.Verify(mocked => mocked.SendPrintRequest(It.Is<PrintRequestDto>(p => p.title.Contains(participantDtos[3].FirstName))));
            _printingService.VerifyAll();

            _kioskRepository.VerifyAll();
        }

        [Test]
        public void ItShouldSaveNewFamilyData()
        {
            // Arrange
            string token = "123abc";

            EventDto eventDto = new EventDto
            {
                EventSiteId = 1
            };

            NewParentDto newParentDto = new NewParentDto
            {
                FirstName = "TestParentFirst",
                LastName = "TestParentLast",
                PhoneNumber = "123-456-7890"
            };

            List<NewChildDto> newChildDtos = new List<NewChildDto>
            {
                new NewChildDto
                {
                    DateOfBirth = new DateTime(2016, 12, 1, 00, 00, 00),
                    FirstName = "TestChildFirst",
                    LastName = "TestChildLast",
                    YearGrade = 1
                }
            };

            NewFamilyDto newFamilyDto = new NewFamilyDto
            {
                EventDto = eventDto,
                ParentContactDto = newParentDto,
                ChildContactDtos = newChildDtos
            };

            MpHouseholdDto mpHouseholdDto = new MpHouseholdDto();
            MpNewParticipantDto newParticipantDto = new MpNewParticipantDto();

            _contactRepository.Setup(m => m.CreateHousehold(token, It.IsAny<MpHouseholdDto>())).Returns(mpHouseholdDto);
            _participantRepository.Setup(m => m.CreateParticipantWithContact(It.IsAny<string>(), It.IsAny<MpNewParticipantDto>())).Returns(newParticipantDto);

            // Act
            var result = _fixture.SaveNewFamilyData(token, newFamilyDto);

            // Assert
            _contactRepository.VerifyAll();
            _participantRepository.VerifyAll();
            Assert.IsNotNull(result);
        }

        [Test]
        public void CheckAcEventDuringCurrentService()
        {
            // Arrange
            _applicationConfiguration.Setup(m => m.AdventureClubEventTypeId).Returns(20);

            var currentMpEventDtoStartTime = DateTime.Now;
            var futureMpEventDtoStartTime = DateTime.Now.AddHours(2);

            var currentMpServiceEventDto = new MpEventDto
            {
                EventId = 1234567,
                ParentEventId = null,
                CongregationId = 8,
                EventTypeId = 123,
                EventStartDate = currentMpEventDtoStartTime
            };

            var currentMpAdventureClubEventDto = new MpEventDto
            {
                EventId = 7654321,
                ParentEventId = 1234567,
                CongregationId = 8,
                EventTypeId = 20,
                EventStartDate = currentMpEventDtoStartTime
            };

            var futureMpServiceEventDto = new MpEventDto
            {
                EventId = 2345678,
                ParentEventId = null,
                CongregationId = 8,
                EventTypeId = 20,
                EventStartDate = futureMpEventDtoStartTime
            };

            // current service event, current ac event, trailing service event
            var eventDtosBySite = new List<MpEventDto>()
            {
                currentMpServiceEventDto,
                currentMpAdventureClubEventDto,
                futureMpServiceEventDto
            };

            _eventRepository.Setup(m => m.GetEvents(It.IsAny<DateTime>(), It.IsAny<DateTime>(), 8, true)).Returns(eventDtosBySite);

            var signingInEventDto = new EventDto
            {
                EventId = 1234567,
                EventSiteId = 8
            };

            var participantEventMapDto = new ParticipantEventMapDto
            {
                CurrentEvent = signingInEventDto,
                ServicesAttended = 2
            };

            // Act
            var result = _fixture.GetEventsForSignin(participantEventMapDto);

            // Assert

            // we expect the child to be signed into the current ac event and future service event
            Assert.AreEqual(result[0].EventId, currentMpAdventureClubEventDto.EventId);
            Assert.AreEqual(result[1].EventId, futureMpServiceEventDto.EventId);
        }

        [Test]
        public void CheckAcEventAfterCurrentService()
        {
            // Arrange
            _applicationConfiguration.Setup(m => m.AdventureClubEventTypeId).Returns(20);

            var currentMpEventDtoStartTime = DateTime.Now;
            var futureMpEventDtoStartTime = DateTime.Now.AddHours(2);

            var currentMpServiceEventDto = new MpEventDto
            {
                EventId = 1234567,
                ParentEventId = null,
                CongregationId = 8,
                EventTypeId = 123,
                EventStartDate = currentMpEventDtoStartTime
            };

            var futureMpServiceEventDto = new MpEventDto
            {
                EventId = 2345678,
                ParentEventId = null,
                CongregationId = 8,
                EventTypeId = 20,
                EventStartDate = futureMpEventDtoStartTime
            };

            var futureMpAdventureClubEventDto = new MpEventDto
            {
                EventId = 7654321,
                ParentEventId = 2345678,
                CongregationId = 8,
                EventTypeId = 20,
                EventStartDate = futureMpEventDtoStartTime
            };

            // current service event, current ac event, trailing service event
            var eventDtosBySite = new List<MpEventDto>()
            {
                currentMpServiceEventDto,
                futureMpServiceEventDto,
                futureMpAdventureClubEventDto
            };

            _eventRepository.Setup(m => m.GetEvents(It.IsAny<DateTime>(), It.IsAny<DateTime>(), 8, true)).Returns(eventDtosBySite);

            var signingInEventDto = new EventDto
            {
                EventId = 1234567,
                EventSiteId = 8
            };

            var participantEventMapDto = new ParticipantEventMapDto
            {
                CurrentEvent = signingInEventDto,
                ServicesAttended = 2
            };

            // Act
            var result = _fixture.GetEventsForSignin(participantEventMapDto);

            // Assert

            // we expect the child to be signed into the current ac event and future service event
            Assert.AreEqual(result[0].EventId, currentMpServiceEventDto.EventId);
            Assert.AreEqual(result[1].EventId, futureMpAdventureClubEventDto.EventId);
        }

        [Test]
        public void CheckAcEventNoAcEvent()
        {
            // Arrange
            _applicationConfiguration.Setup(m => m.AdventureClubEventTypeId).Returns(20);

            var currentMpEventDtoStartTime = DateTime.Now;
            var futureMpEventDtoStartTime = DateTime.Now.AddHours(2);

            MpEventDto currentMpServiceEventDto = new MpEventDto
            {
                EventId = 1234567,
                ParentEventId = null,
                CongregationId = 8,
                EventTypeId = 123,
                EventStartDate = currentMpEventDtoStartTime
            };

            MpEventDto futureMpAdventureClubEventDto = new MpEventDto
            {
                EventId = 7654321,
                ParentEventId = 1234567,
                CongregationId = 8,
                EventTypeId = 20,
                EventStartDate = futureMpEventDtoStartTime
            };

            MpEventDto futureMpServiceEventDto = new MpEventDto
            {
                EventId = 2345678,
                ParentEventId = null,
                CongregationId = 8,
                EventTypeId = 20,
                EventStartDate = futureMpEventDtoStartTime
            };

            // current service event, future service event
            List<MpEventDto> eventDtosBySite = new List<MpEventDto>()
            {
                currentMpServiceEventDto,
                futureMpServiceEventDto
            };

            _eventRepository.Setup(m => m.GetEvents(It.IsAny<DateTime>(), It.IsAny<DateTime>(), 8, true)).Returns(eventDtosBySite);

            EventDto signingInEventDto = new EventDto
            {
                EventId = 1234567,
                EventSiteId = 8
            };

            ParticipantEventMapDto participantEventMapDto = new ParticipantEventMapDto
            {
                CurrentEvent = signingInEventDto,
                ServicesAttended = 2
            };

            // Act
            var result = _fixture.GetEventsForSignin(participantEventMapDto);

            // Assert

            // we expect the child to be signed into the current ac event and future service event
            Assert.AreEqual(result[0].EventId, currentMpServiceEventDto.EventId);
            Assert.AreEqual(result.Count, 1);
        }

        [Test]
        public void CheckAcEventCurrentService()
        {
            // Arrange
            _applicationConfiguration.Setup(m => m.AdventureClubEventTypeId).Returns(20);

            var currentMpEventDtoStartTime = DateTime.Now;

            MpEventDto currentMpServiceEventDto = new MpEventDto
            {
                EventId = 1234567,
                ParentEventId = null,
                CongregationId = 8,
                EventTypeId = 123,
                EventStartDate = currentMpEventDtoStartTime
            };

            MpEventDto currentMpAdventureClubEventDto = new MpEventDto
            {
                EventId = 7654321,
                ParentEventId = 1234567,
                CongregationId = 8,
                EventTypeId = 20,
                EventStartDate = currentMpEventDtoStartTime
            };

            // current service event, future service event
            List<MpEventDto> eventDtosBySite = new List<MpEventDto>()
            {
                currentMpServiceEventDto,
                currentMpAdventureClubEventDto
            };

            _eventRepository.Setup(m => m.GetEvents(It.IsAny<DateTime>(), It.IsAny<DateTime>(), 8, true)).Returns(eventDtosBySite);

            EventDto signingInEventDto = new EventDto
            {
                EventId = 1234567,
                EventSiteId = 8
            };

            ParticipantEventMapDto participantEventMapDto = new ParticipantEventMapDto
            {
                CurrentEvent = signingInEventDto,
                ServicesAttended = 2
            };

            // Act
            var result = _fixture.GetEventsForSignin(participantEventMapDto);

            // Assert

            // we expect the child to be signed into the current ac event and future service event
            Assert.AreEqual(result[0].EventId, currentMpServiceEventDto.EventId);
            Assert.AreEqual(result.Count, 1);
        }

        [Test]
        public void ShouldProcessGuestSignIns()
        {
            // Arrange
            const int groupId = 1000000;
            const int participantId = 5544555;

            ParticipantDto guestParticipantDto = new ParticipantDto
            {
                FirstName = "TestFirst",
                LastName = "TestLast",
                DateOfBirth = new DateTime(2008, 10, 10),
                YearGrade = 0,
                GuestSignin = true
            };

            ParticipantEventMapDto participantEventMapDto = new ParticipantEventMapDto
            {
                Participants = new List<ParticipantDto>
                {
                    guestParticipantDto
                }
            };

            _applicationConfiguration.Setup(m => m.GuestHouseholdId).Returns(5771805);

            MpNewParticipantDto mpNewParticipantDto = new MpNewParticipantDto
            {
                ParticipantId = participantId
            };

            _participantRepository.Setup(m => m.CreateParticipantWithContact(It.IsAny<string>(), It.IsAny<MpNewParticipantDto>())).Returns(mpNewParticipantDto);

            _groupLookupRepository.Setup(m => m.GetGroupId(new DateTime(2008, 10, 10), null)).Returns(groupId);

            MpGroupParticipantDto mpGroupParticipantDto = new MpGroupParticipantDto
            {
                GroupId = groupId,
                ParticipantId = participantId
            };

            List<MpGroupParticipantDto> mpGroupParticipantDtos = new List<MpGroupParticipantDto>
            {
                mpGroupParticipantDto
            };

            _participantRepository.Setup(m => m.CreateGroupParticipants(It.IsAny<string>(), It.IsAny<List<MpGroupParticipantDto>>())).Returns(mpGroupParticipantDtos);

            // Act
            _fixture.ProcessGuestSignins(participantEventMapDto);

            // Assert - Testing to make sure that these fields are being set correctly on the guest participant
            Assert.AreEqual(participantEventMapDto.Participants[0].GroupId, groupId);
            Assert.AreEqual(participantEventMapDto.Participants[0].ParticipantId, participantId);
        }

        [Test]
        public void ShouldProcessMixOfGuestSignIns()
        {
            // Arrange
            const int groupId = 1000000;
            const int participantId = 5544555;

            const int nonGuestGroupId = 2000000;
            const int nonGuestParticipantId = 3322333;

            ParticipantDto guestParticipantDto = new ParticipantDto
            {
                FirstName = "TestFirst",
                LastName = "TestLast",
                DateOfBirth = new DateTime(2008, 10, 10),
                YearGrade = 0,
                GuestSignin = true
            };

            ParticipantDto nonGuestParticipantDto = new ParticipantDto
            {
                FirstName = "NonguestFirst",
                LastName = "NonguestLast",
                DateOfBirth = new DateTime(2009, 10, 10),
                GroupId = nonGuestGroupId,
                ParticipantId = nonGuestParticipantId,
                GuestSignin = false
            };

            ParticipantEventMapDto participantEventMapDto = new ParticipantEventMapDto
            {
                Participants = new List<ParticipantDto>
                {
                    guestParticipantDto,
                    nonGuestParticipantDto
                }
            };

            _applicationConfiguration.Setup(m => m.GuestHouseholdId).Returns(5771805);

            MpNewParticipantDto mpNewParticipantDto = new MpNewParticipantDto
            {
                ParticipantId = participantId
            };

            _participantRepository.Setup(m => m.CreateParticipantWithContact(It.IsAny<string>(), It.IsAny<MpNewParticipantDto>())).Returns(mpNewParticipantDto);

            _groupLookupRepository.Setup(m => m.GetGroupId(new DateTime(2008, 10, 10), null)).Returns(groupId);

            MpGroupParticipantDto mpGroupParticipantDto = new MpGroupParticipantDto
            {
                GroupId = groupId,
                ParticipantId = participantId
            };

            List<MpGroupParticipantDto> mpGroupParticipantDtos = new List<MpGroupParticipantDto>
            {
                mpGroupParticipantDto
            };

            _participantRepository.Setup(m => m.CreateGroupParticipants(It.IsAny<string>(), It.IsAny<List<MpGroupParticipantDto>>())).Returns(mpGroupParticipantDtos);

            // Act
            _fixture.ProcessGuestSignins(participantEventMapDto);

            // Assert

            //Testing to make sure that these fields are being set correctly on the guest participant
            Assert.AreEqual(participantEventMapDto.Participants[0].GroupId, groupId);
            Assert.AreEqual(participantEventMapDto.Participants[0].ParticipantId, participantId);

            // Testing to make sure that the fields were not set against on the non-guest participant
            Assert.AreEqual(participantEventMapDto.Participants[1].GroupId, nonGuestGroupId);
            Assert.AreEqual(participantEventMapDto.Participants[1].ParticipantId, nonGuestParticipantId);
        }

        [Test]
        public void ShouldReverseSignin_ParticipantNotCheckedIn()
        {
            // Arrange
            string token = "ABC123";
            int eventParticipantId = 1234567;
            int attendedParticipantStatusId = 3;
            int checkedInParticipantStatusId = 4;
            int cancelledParticipantStatusId = 5;

            MpEventParticipantDto mpEventParticipantDto = new MpEventParticipantDto
            {
                EventParticipantId = 1234567,
                ParticipantStatusId = attendedParticipantStatusId
            };

            _participantRepository.Setup(r => r.GetEventParticipantByEventParticipantId(token, eventParticipantId))
                .Returns(mpEventParticipantDto);
            _participantRepository.Setup(r => r.UpdateEventParticipants(It.IsAny<List<MpEventParticipantDto>>()));
            _applicationConfiguration.Setup(r => r.AttendeeParticipantType).Returns(attendedParticipantStatusId);
            _applicationConfiguration.Setup(r => r.CheckedInParticipationStatusId).Returns(checkedInParticipantStatusId);
            _applicationConfiguration.Setup(r => r.CancelledParticipationStatusId).Returns(cancelledParticipantStatusId);

            // Act
            _fixture.ReverseSignin(token, eventParticipantId);

            // Assert
            Assert.AreNotEqual(mpEventParticipantDto.EndDate, null);
            Assert.AreEqual(mpEventParticipantDto.ParticipantStatusId, cancelledParticipantStatusId);
            _participantRepository.VerifyAll();
        }

        [Test]
        public void ShouldNotReverseSignin_ParticipantCheckedIn()
        {
            // Arrange
            string token = "ABC123";
            int eventParticipantId = 1234567;
            int attendedParticipantStatusId = 3;
            int checkedInParticipantStatusId = 4;
            int cancelledParticipantStatusId = 5;

            MpEventParticipantDto mpEventParticipantDto = new MpEventParticipantDto
            {
                EventParticipantId = 1234567,
                ParticipantStatusId = checkedInParticipantStatusId
            };

            _participantRepository.Setup(r => r.GetEventParticipantByEventParticipantId(token, eventParticipantId))
                .Returns(mpEventParticipantDto);
            _participantRepository.Setup(r => r.UpdateEventParticipants(It.IsAny<List<MpEventParticipantDto>>()));
            _applicationConfiguration.Setup(r => r.AttendeeParticipantType).Returns(attendedParticipantStatusId);
            _applicationConfiguration.Setup(r => r.CheckedInParticipationStatusId).Returns(checkedInParticipantStatusId);
            _applicationConfiguration.Setup(r => r.CancelledParticipationStatusId).Returns(cancelledParticipantStatusId);

            // Act
            var result =_fixture.ReverseSignin(token, eventParticipantId);

            // Assert
            Assert.AreEqual(result, false);
            Assert.AreEqual(mpEventParticipantDto.ParticipantStatusId, checkedInParticipantStatusId);
        }

        [Test]
        public void ShouldReprintEventParticipantTag()
        {
            var token = "abcd";
            var kioskId = "1a11a1a1-a11a-1a1a-11a1-a111a111a11a";

            var participant = new MpEventParticipantDto
            {
                EventId = 123,
                EventParticipantId = 765,
                FirstName = "Test",
                LastName = "User",
                CheckinHouseholdId = 4
            };

            var contacts = new List<MpContactDto>
            {
                new MpContactDto
                {
                    HouseholdId = 4,
                    FirstName = "Dad",
                    LastName = "Hello"
                },
                new MpContactDto
                {
                    HouseholdId = 4,
                    FirstName = "Mom",
                    LastName = "Hello"
                }
            };

            var currentEvent = new EventDto
            {
                EventId = 123,
                EventTitle = "this test"
            };

            var mpKioskConfigDto = new MpKioskConfigDto
            {
                KioskIdentifier = Guid.Parse("1a11a1a1-a11a-1a1a-11a1-a111a111a11a"),
                CongregationId = 1,
                PrinterMapId = 1111111
            };

            var mpPrinterMapDto = new MpPrinterMapDto
            {
                PrinterMapId = 1111111
            };

            _participantRepository.Setup(m => m.GetEventParticipantByEventParticipantId(token, 765)).Returns(participant);
            _eventService.Setup(m => m.GetEvent(123)).Returns(currentEvent);
            _contactRepository.Setup(m => m.GetHeadsOfHouseholdByHouseholdId(4)).Returns(contacts);
            _kioskRepository.Setup(m => m.GetMpKioskConfigByIdentifier(It.IsAny<Guid>())).Returns(mpKioskConfigDto);
            _kioskRepository.Setup(m => m.GetPrinterMapById(mpKioskConfigDto.PrinterMapId.GetValueOrDefault())).Returns(mpPrinterMapDto);
            _pdfEditor.Setup(m => m.PopulatePdfMergeFields(It.IsAny<byte[]>(), It.IsAny<Dictionary<string, string>>())).Returns("");
            _printingService.Setup(m => m.SendPrintRequest(It.IsAny<PrintRequestDto>())).Returns(1);


            var participantEventMapDto = _fixture.PrintParticipant(765, "1a11a1a1-a11a-1a1a-11a1-a111a111a11a", "abcd");

            Assert.AreEqual(participantEventMapDto.Participants.Count, 1);
            Assert.AreEqual(participantEventMapDto.Participants[0].EventId, participant.EventId);
            Assert.AreEqual(participantEventMapDto.Participants[0].EventParticipantId, participant.EventParticipantId);
            Assert.AreEqual(participantEventMapDto.Participants[0].FirstName, participant.FirstName);
            Assert.AreEqual(participantEventMapDto.Participants[0].LastName, participant.LastName);
            Assert.AreEqual(participantEventMapDto.Participants[0].CheckinHouseholdId, participant.CheckinHouseholdId);

            Assert.AreEqual(participantEventMapDto.Contacts.Count, 2);
            Assert.AreEqual(participantEventMapDto.Contacts[0].HouseholdId, contacts[0].HouseholdId);
            Assert.AreEqual(participantEventMapDto.Contacts[0].FirstName, contacts[0].FirstName);
            Assert.AreEqual(participantEventMapDto.Contacts[0].LastName, contacts[0].LastName);
            Assert.AreEqual(participantEventMapDto.Contacts[1].HouseholdId, contacts[1].HouseholdId);
            Assert.AreEqual(participantEventMapDto.Contacts[1].FirstName, contacts[1].FirstName);
            Assert.AreEqual(participantEventMapDto.Contacts[1].LastName, contacts[1].LastName);

            Assert.AreEqual(participantEventMapDto.CurrentEvent.EventId, currentEvent.EventId);
            Assert.AreEqual(participantEventMapDto.CurrentEvent.EventTitle, currentEvent.EventTitle);
        }

        [Test]
        public void ShouldMarkDuplicateSigninsSingleEvent()
        {
            // Arrange
            List<MpEventDto> eventDtos = new List<MpEventDto>
            {
                new MpEventDto
                {
                    EventId = 1234567
                }
            };

            ParticipantEventMapDto participantEventMapDto = new ParticipantEventMapDto
            {
                Participants = new List<ParticipantDto>
                {
                    new ParticipantDto
                    {
                        ParticipantId = 4455544,
                        DuplicateSignIn = false
                    }
                }
            };

            List<MpEventParticipantDto> mpEventParticipantDtos = new List<MpEventParticipantDto>
            {
                new MpEventParticipantDto
                {
                    EventId = 1234567,
                    ParticipantId = 4455544
                }
            };

            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(eventDtos[0].EventId, It.IsAny<List<int>>())).Returns(mpEventParticipantDtos);
            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(0, It.IsAny<List<int>>())).Returns(new List<MpEventParticipantDto>());

            // Act
            _fixture.CheckForDuplicateSignIns(eventDtos, participantEventMapDto);

            // Assert
            Assert.AreEqual(participantEventMapDto.Participants[0].DuplicateSignIn, true);
        }

        [Test]
        public void ShouldNotMarkDuplicateSigninsSingleEvent()
        {
            // Arrange
            List<MpEventDto> eventDtos = new List<MpEventDto>
            {
                new MpEventDto
                {
                    EventId = 1234567
                }
            };

            ParticipantEventMapDto participantEventMapDto = new ParticipantEventMapDto
            {
                Participants = new List<ParticipantDto>
                {
                    new ParticipantDto
                    {
                        ParticipantId = 4455544,
                        DuplicateSignIn = false
                    }
                }
            };

            List<MpEventParticipantDto> mpEventParticipantDtos = new List<MpEventParticipantDto>();

            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(eventDtos[0].EventId, It.IsAny<List<int>>())).Returns(mpEventParticipantDtos);
            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(0, It.IsAny<List<int>>())).Returns(mpEventParticipantDtos);

            // Act
            _fixture.CheckForDuplicateSignIns(eventDtos, participantEventMapDto);

            // Assert
            Assert.AreEqual(participantEventMapDto.Participants[0].DuplicateSignIn, false);
        }

        [Test]
        public void ShouldMarkDuplicateSigninsACEvent()
        {
            // Arrange
            List<MpEventDto> eventDtos = new List<MpEventDto>
            {
                new MpEventDto
                {
                    EventId = 1234567
                },
                new MpEventDto
                {
                    ParentEventId = 1234567,
                    EventId = 7654321
                },
                new MpEventDto
                {
                    EventId = 2345678
                }
            };

            ParticipantEventMapDto participantEventMapDto = new ParticipantEventMapDto
            {
                Participants = new List<ParticipantDto>
                {
                    new ParticipantDto
                    {
                        ParticipantId = 4455544,
                        DuplicateSignIn = false
                    }
                }
            };

            List<MpEventParticipantDto> mpEventParticipantDtos = new List<MpEventParticipantDto>
            {
                new MpEventParticipantDto
                {
                    EventId = 7654321,
                    ParticipantId = 4455544
                }
            };

            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(eventDtos[0].EventId, It.IsAny<List<int>>())).Returns(new List<MpEventParticipantDto>());
            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(eventDtos[1].EventId, It.IsAny<List<int>>())).Returns(mpEventParticipantDtos);
            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(eventDtos[2].EventId, It.IsAny<List<int>>())).Returns(new List<MpEventParticipantDto>());
            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(0, It.IsAny<List<int>>())).Returns(new List<MpEventParticipantDto>());

            // Act
            _fixture.CheckForDuplicateSignIns(eventDtos, participantEventMapDto);

            // Assert
            Assert.AreEqual(participantEventMapDto.Participants[0].DuplicateSignIn, true);
        }

        [Test]
        public void ShouldNotMarkDuplicateSigninsACEvent()
        {
            // Arrange
            List<MpEventDto> eventDtos = new List<MpEventDto>
            {
                new MpEventDto
                {
                    EventId = 1234567
                },
                new MpEventDto
                {
                    ParentEventId = 1234567,
                    EventId = 7654321
                },
                new MpEventDto
                {
                    EventId = 2345678
                }
            };

            ParticipantEventMapDto participantEventMapDto = new ParticipantEventMapDto
            {
                Participants = new List<ParticipantDto>
                {
                    new ParticipantDto
                    {
                        ParticipantId = 4455544,
                        DuplicateSignIn = false
                    }
                }
            };

            List<MpEventParticipantDto> mpEventParticipantDtos = new List<MpEventParticipantDto>
            {
                new MpEventParticipantDto
                {
                    EventId = 7654321,
                    ParticipantId = 4455544
                }
            };

            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(eventDtos[0].EventId, It.IsAny<List<int>>())).Returns(new List<MpEventParticipantDto>());
            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(eventDtos[1].EventId, It.IsAny<List<int>>())).Returns(new List<MpEventParticipantDto>());
            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(eventDtos[2].EventId, It.IsAny<List<int>>())).Returns(new List<MpEventParticipantDto>());
            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(0, It.IsAny<List<int>>())).Returns(new List<MpEventParticipantDto>());

            // Act
            _fixture.CheckForDuplicateSignIns(eventDtos, participantEventMapDto);

            // Assert
            Assert.AreEqual(participantEventMapDto.Participants[0].DuplicateSignIn, false);
        }

        [Test]
        public void ShouldMarkDuplicateSigninsFirstServiceEvent()
        {
            // Arrange
            List<MpEventDto> eventDtos = new List<MpEventDto>
            {
                new MpEventDto
                {
                    ParentEventId = 1234567,
                    EventId = 7654321
                },
                new MpEventDto
                {
                    EventId = 2345678
                }
            };

            ParticipantEventMapDto participantEventMapDto = new ParticipantEventMapDto
            {
                Participants = new List<ParticipantDto>
                {
                    new ParticipantDto
                    {
                        ParticipantId = 4455544,
                        DuplicateSignIn = false
                    }
                }
            };

            List<MpEventParticipantDto> mpEventParticipantDtos = new List<MpEventParticipantDto>
            {
                new MpEventParticipantDto
                {
                    EventId = 1234567,
                    ParticipantId = 4455544
                }
            };

            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(1234567, It.IsAny<List<int>>())).Returns(mpEventParticipantDtos);
            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(7654321, It.IsAny<List<int>>())).Returns(mpEventParticipantDtos);
            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(eventDtos[1].EventId, It.IsAny<List<int>>())).Returns(new List<MpEventParticipantDto>());
            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(0, It.IsAny<List<int>>())).Returns(new List<MpEventParticipantDto>());

            // Act
            _fixture.CheckForDuplicateSignIns(eventDtos, participantEventMapDto);

            // Assert
            Assert.AreEqual(participantEventMapDto.Participants[0].DuplicateSignIn, true);
        }

        [Test]
        public void ShouldMarkDuplicateSigninsSecondServiceEvent()
        {
            // Arrange
            List<MpEventDto> eventDtos = new List<MpEventDto>
            {
                new MpEventDto
                {
                    EventId = 1234567
                },
                new MpEventDto
                {
                    ParentEventId = 1234567,
                    EventId = 7654321
                },
                new MpEventDto
                {
                    EventId = 2345678
                }
            };

            ParticipantEventMapDto participantEventMapDto = new ParticipantEventMapDto
            {
                Participants = new List<ParticipantDto>
                {
                    new ParticipantDto
                    {
                        ParticipantId = 4455544,
                        DuplicateSignIn = false
                    }
                }
            };

            List<MpEventParticipantDto> mpEventParticipantDtos = new List<MpEventParticipantDto>
            {
                new MpEventParticipantDto
                {
                    EventId = 2345678,
                    ParticipantId = 4455544
                }
            };

            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(eventDtos[0].EventId, It.IsAny<List<int>>())).Returns(new List<MpEventParticipantDto>());
            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(eventDtos[1].EventId, It.IsAny<List<int>>())).Returns(new List<MpEventParticipantDto>());
            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(eventDtos[2].EventId, It.IsAny<List<int>>())).Returns(mpEventParticipantDtos);
            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(0, It.IsAny<List<int>>())).Returns(new List<MpEventParticipantDto>());

            // Act
            _fixture.CheckForDuplicateSignIns(eventDtos, participantEventMapDto);

            // Assert
            Assert.AreEqual(participantEventMapDto.Participants[0].DuplicateSignIn, true);
        }
    }
}
