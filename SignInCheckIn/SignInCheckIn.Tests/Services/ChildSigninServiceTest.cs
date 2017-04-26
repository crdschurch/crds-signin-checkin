using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using Crossroads.Utilities.Services.Interfaces;
using FluentAssertions.Common;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Printing.Utilities.Models;
using Printing.Utilities.Services.Interfaces;
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
        private Mock<IAttributeRepository> _attributeRepository;
        private Mock<ISignInLogic> _signInLogic;

        private ChildSigninService _fixture;

        private static int ChildcareEventTypeId = 243;
        private static int ChildcareGroupTypeId = 27;
        private static int BigEventTypeId = 369;
        private static int MiddleSchoolEventTypeId = 402;
        private static int HighSchoolEventTypeId = 403;
        private static int KidsClubEventTypeId = 410; // huh?

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
            _attributeRepository = new Mock<IAttributeRepository>();
            _signInLogic = new Mock<ISignInLogic>();

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

            _applicationConfiguration.SetupGet(m => m.ChildcareEventTypeId).Returns(ChildcareEventTypeId);
            _applicationConfiguration.SetupGet(m => m.ChildcareEventTypeId).Returns(ChildcareGroupTypeId);
            _applicationConfiguration.SetupGet(m => m.StudentMinistryGradesSixToEightEventTypeId).Returns(MiddleSchoolEventTypeId);
            _applicationConfiguration.SetupGet(m => m.StudentMinistryGradesNineToTwelveEventTypeId).Returns(HighSchoolEventTypeId);
            _applicationConfiguration.SetupGet(m => m.BigEventTypeId).Returns(BigEventTypeId);

            _fixture = new ChildSigninService(_childSigninRepository.Object,_eventRepository.Object, 
                _groupRepository.Object, _eventService.Object, _pdfEditor.Object, _printingService.Object,
                _contactRepository.Object, _kioskRepository.Object, _participantRepository.Object,
                _applicationConfiguration.Object, _groupLookupRepository.Object, _roomRepository.Object,
                _configRepository.Object, _attributeRepository.Object, _signInLogic.Object);
        }

        [Test]
        public void ShouldGetChildrenAndEventByHouseholdId()
        {
            const int householdId = 654;
            const int siteId = 16;
            const string kioskId = "504c73c1-d664-4ccd-964e-e008e7ce2635";

                  var eventDto = new EventDto()
            {
                EventId = 468
            };
            
            var children = new List<MpParticipantDto>
            {
                new MpParticipantDto
                {
                    ParticipantId = 534
                }
            };
            var headsOfHousehold = new List<MpContactDto>
            {
                new MpContactDto
                {
                    ContactId = 331
                }
            };
            _eventService.Setup(m => m.GetEvent(eventDto.EventId)).Returns(eventDto);
            _childSigninRepository.Setup(m => m.GetChildrenByHouseholdId(householdId, eventDto.EventId)).Returns(children);
            _contactRepository.Setup(m => m.GetHeadsOfHouseholdByHouseholdId(householdId)).Returns(headsOfHousehold);

            var result = _fixture.GetChildrenAndEventByHouseholdId(householdId, eventDto.EventId, siteId, kioskId);

            Assert.AreEqual(result.Contacts[0].ContactId, headsOfHousehold[0].ContactId);
            Assert.AreEqual(result.Participants[0].ParticipantId, children[0].ParticipantId);
            Assert.AreEqual(result.CurrentEvent.EventId, eventDto.EventId);

        }

        [Test]
        public void ShouldGetChildrenByPhoneNumber()
        {
            const int siteId = 1;
            const string phoneNumber = "812-812-8877";
            int? primaryHouseholdId = 123;
            const string kioskId = "504c73c1-d664-4ccd-964e-e008e7ce2635";

            var eventDto = new EventDto
            {
                EventTypeId = 1
            };

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
            _eventService.Setup(m => m.GetCurrentEventForSite(siteId, kioskId)).Returns(eventDto);
            var result = _fixture.GetChildrenAndEventByPhoneNumber(phoneNumber, siteId, eventDto);
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
            const string kioskId = "504c73c1-d664-4ccd-964e-e008e7ce2635";

            var mpHouseholdAndParticipants = new MpHouseholdParticipantsDto
            {
                HouseholdId = primaryHouseholdId.GetValueOrDefault(),
            };

            var eventDto = new EventDto
            {
                EventTypeId = 1
            };

            var contactDtos = new List<MpContactDto>();

            _childSigninRepository.Setup(mocked => mocked.GetChildrenByPhoneNumber(phoneNumber, true)).Returns(mpHouseholdAndParticipants);
            _contactRepository.Setup(m => m.GetHeadsOfHouseholdByHouseholdId(primaryHouseholdId.Value)).Returns(contactDtos);
            _eventService.Setup(m => m.GetCurrentEventForSite(siteId, kioskId)).Returns(eventDto);
            var result = _fixture.GetChildrenAndEventByPhoneNumber(phoneNumber, siteId, eventDto);
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

            _eventRepository.Setup(m => m.GetEvents(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), true, It.IsAny<List<int>>(), true)).Returns(eventDtosBySite);

            var participantEventMapDto = new ParticipantEventMapDto();
            participantEventMapDto.Participants = participantDtos;
            participantEventMapDto.Contacts = contactDtos;
            participantEventMapDto.CurrentEvent = eventDto;

            _applicationConfiguration.Setup(m => m.AdventureClubEventTypeId).Returns(1);
            _eventRepository.Setup(m => m.GetSubeventByParentEventId(eventDto.EventId, 1)).Returns((MpEventDto)null);

            _eventService.Setup(m => m.GetEvent(eventDto.EventId)).Returns(participantEventMapDto.CurrentEvent);
            _eventService.Setup(m => m.CheckEventTimeValidity(participantEventMapDto.CurrentEvent)).Returns(true);
            _eventRepository.Setup(m => m.GetEventGroupsForEvent(participantEventMapDto.CurrentEvent.EventId)).Returns(mpEventGroupDtos);
            _groupRepository.Setup(m => m.GetGroup(null, 2, false)).Returns((MpGroupDto)null);
            _childSigninRepository.Setup(m => m.CreateEventParticipants(It.IsAny<List<MpEventParticipantDto>>())).Returns(mpEventParticipantDtos);
            _participantRepository.Setup(m => m.UpdateEventParticipants(It.IsAny<List<MpEventParticipantDto>>()));
            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(It.IsAny<int>(), It.IsAny<List<int>>())).Returns(new List<MpEventParticipantDto>());

            // TODO: Determine what this should return from a unit test?
            _signInLogic.Setup(m => m.SignInParticipants(participantEventMapDto, eventDtosBySite)).Returns(participantDtos);

            // Act
            var response = _fixture.SigninParticipants(participantEventMapDto);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsNull(response.Participants[0].SignInErrorMessage);
        }
        
        [Obsolete]
        //[Test]
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

            _eventRepository.Setup(m => m.GetEvents(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), true, It.IsAny<List<int>>(), true)).Returns(eventDtosBySite);

            _applicationConfiguration.Setup(m => m.AdventureClubEventTypeId).Returns(1);
            _eventRepository.Setup(m => m.GetSubeventByParentEventId(eventDto.EventId, 1)).Returns((MpEventDto)null);

            _eventService.Setup(m => m.GetEvent(eventDto.EventId)).Returns(participantEventMapDto.CurrentEvent);
            _eventService.Setup(m => m.CheckEventTimeValidity(participantEventMapDto.CurrentEvent)).Returns(true);
            _eventRepository.Setup(m => m.GetEventGroupsForEvent(participantEventMapDto.CurrentEvent.EventId)).Returns(mpEventGroupDtos);
            _groupRepository.Setup(m => m.GetGroup(null, 2, false)).Returns((MpGroupDto)null);
            _roomRepository.Setup(m => m.GetBumpingRoomsForEventRoom(321, 153234)).Returns(mpBumpingRooms);
            _childSigninRepository.Setup(m => m.CreateEventParticipants(It.IsAny<List<MpEventParticipantDto>>())).Returns(mpEventParticipantDtos);
            _participantRepository.Setup(m => m.UpdateEventParticipants(It.IsAny<List<MpEventParticipantDto>>()));
            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(It.IsAny<int>(), It.IsAny<List<int>>())).Returns(new List<MpEventParticipantDto>());

            // TODO: Determine what this should return from a unit test?
            _signInLogic.Setup(m => m.SignInParticipants(participantEventMapDto, eventDtosBySite)).Returns(participantDtos);

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

            _eventRepository.Setup(m => m.GetEvents(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), true, It.IsAny<List<int>>(), true)).Returns(eventDtosBySite);

            _applicationConfiguration.Setup(m => m.AdventureClubEventTypeId).Returns(1);
            _eventRepository.Setup(m => m.GetSubeventByParentEventId(eventDto.EventId, 1)).Returns((MpEventDto)null);

            _eventService.Setup(m => m.GetEvent(eventDto.EventId)).Returns(participantEventMapDto.CurrentEvent);
            _eventService.Setup(m => m.CheckEventTimeValidity(participantEventMapDto.CurrentEvent)).Returns(true);
            _eventRepository.Setup(m => m.GetEventGroupsForEvent(participantEventMapDto.CurrentEvent.EventId)).Returns(mpEventGroupDtos);
            _groupRepository.Setup(m => m.GetGroup(null, 2, false)).Returns((MpGroupDto)null);
            _roomRepository.Setup(m => m.GetBumpingRoomsForEventRoom(321, 153234)).Returns(new List<MpBumpingRoomsDto>());
            _childSigninRepository.Setup(m => m.CreateEventParticipants(It.IsAny<List<MpEventParticipantDto>>())).Returns(mpEventParticipantDtos);
            _participantRepository.Setup(m => m.UpdateEventParticipants(It.IsAny<List<MpEventParticipantDto>>()));
            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(It.IsAny<int>(), It.IsAny<List<int>>())).Returns(new List<MpEventParticipantDto>());

            // TODO: Determine what this should return from a unit test?
            _signInLogic.Setup(m => m.SignInParticipants(participantEventMapDto, eventDtosBySite)).Returns(participantDtos);

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
                EventTitle = "test event",
                EventTypeId = ChildcareEventTypeId
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
        public void ShouldNotPrintLabelsForAllParticipants()
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
                EventTitle = "test event",
                EventTypeId = MiddleSchoolEventTypeId
            };

            var participantEventMapDto = new ParticipantEventMapDto();
            participantEventMapDto.Participants = participantDtos;
            participantEventMapDto.Contacts = contactDtos;
            participantEventMapDto.CurrentEvent = eventDto;

            // Act
            var result = _fixture.PrintParticipants(participantEventMapDto, kioskId.ToString());

            // Assert
            _kioskRepository.VerifyAll();
            Assert.Null(result);
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
                    Nickname = "Child1Nickname",
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
                    Nickname = "Child2Nickname",
                    AssignedRoomId = null,
                    AssignedSecondaryRoomId = 2345678,
                    AssignedSecondaryRoomName = "TestSecondaryRoom",
                    ParticipantId = 222,
                    Selected = true
                },
                new ParticipantDto
                {
                    FirstName = "Child3First",
                    Nickname = "Child3Nickname",
                    AssignedRoomId = null,
                    AssignedSecondaryRoomId = 2345678,
                    AssignedSecondaryRoomName = "TestSecondaryRoom",
                    ParticipantId = 333,
                    Selected = false
                },
                new ParticipantDto
                {
                    FirstName = "Child4First",
                    Nickname = "Child4Nickname",
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
                EventTitle = "test event",
                EventTypeId = ChildcareEventTypeId
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
                    mocked.PopulatePdfMergeFields(Properties.Resources.Checkin_KC_Label, It.Is<Dictionary<string, string>>(d => d["ChildName"].Equals(participantDtos[0].Nickname))));
            _pdfEditor.Verify(
                mocked =>
                    mocked.PopulatePdfMergeFields(Properties.Resources.Activity_Kit_Label, It.Is<Dictionary<string, string>>(d => d["ChildName"].Equals(participantDtos[1].Nickname))));
            _pdfEditor.Verify(
                mocked =>
                    mocked.PopulatePdfMergeFields(Properties.Resources.Error_Label, It.Is<Dictionary<string, string>>(d => d["ChildName"].Equals(participantDtos[3].Nickname))));
            _pdfEditor.VerifyAll();

            // Verify that we called the print service for each expected participant
            _printingService.Verify(mocked => mocked.SendPrintRequest(It.Is<PrintRequestDto>(p => p.title.Contains(participantDtos[0].Nickname))));
            _printingService.Verify(mocked => mocked.SendPrintRequest(It.Is<PrintRequestDto>(p => p.title.Contains(participantDtos[1].Nickname))));
            _printingService.Verify(mocked => mocked.SendPrintRequest(It.Is<PrintRequestDto>(p => p.title.Contains(participantDtos[3].Nickname))));
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
            _participantRepository.Setup(m => m.CreateParticipantWithContact(It.IsAny<MpNewParticipantDto>(), It.IsAny<string>())).Returns(newParticipantDto);

            // Act
            var result = _fixture.SaveNewFamilyData(token, newFamilyDto);

            // Assert
            _contactRepository.VerifyAll();
            _participantRepository.VerifyAll();
            Assert.IsNotNull(result);
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

            _participantRepository.Setup(m => m.CreateParticipantWithContact(It.IsAny<MpNewParticipantDto>(), It.IsAny<string>())).Returns(mpNewParticipantDto);

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

            _participantRepository.Setup(m => m.CreateParticipantWithContact(It.IsAny<MpNewParticipantDto>(), null)).Returns(mpNewParticipantDto);

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
                EventTitle = "this test",
                EventTypeId = ChildcareEventTypeId
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

            _applicationConfiguration.Setup(m => m.AdventureClubEventTypeId).Returns(1);
            _eventRepository.Setup(m => m.GetSubeventByParentEventId(eventDtos[0].EventId, 1)).Returns((MpEventDto)null);
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

            _applicationConfiguration.Setup(m => m.AdventureClubEventTypeId).Returns(1);
            _eventRepository.Setup(m => m.GetSubeventByParentEventId(eventDtos[0].EventId, 1)).Returns((MpEventDto)null);
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
                    ParentEventId = 1234567,
                    EventId = 7654321
                },
                new MpEventDto
                {
                    EventId = 2345678
                }
            };

            var parentEvent = new MpEventDto
            {
                EventId = 1234567
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

            _applicationConfiguration.Setup(m => m.AdventureClubEventTypeId).Returns(1);
            _eventRepository.Setup(m => m.GetSubeventByParentEventId(eventDtos[1].EventId, 1)).Returns(parentEvent);
            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(eventDtos[0].EventId, It.IsAny<List<int>>())).Returns(new List<MpEventParticipantDto>());
            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(eventDtos[1].EventId, It.IsAny<List<int>>())).Returns(mpEventParticipantDtos);
            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(parentEvent.EventId, It.IsAny<List<int>>())).Returns(new List<MpEventParticipantDto>());
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

            _applicationConfiguration.Setup(m => m.AdventureClubEventTypeId).Returns(1);
            _eventRepository.Setup(m => m.GetSubeventByParentEventId(eventDtos[0].EventId, 1)).Returns(eventDtos[0]);
            _eventRepository.Setup(m => m.GetSubeventByParentEventId(eventDtos[2].EventId, 1)).Returns((MpEventDto)null);
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

            _applicationConfiguration.Setup(m => m.AdventureClubEventTypeId).Returns(1);
            _eventRepository.Setup(m => m.GetSubeventByParentEventId(eventDtos[1].EventId, 1)).Returns((MpEventDto) null);
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

            _applicationConfiguration.Setup(m => m.AdventureClubEventTypeId).Returns(1);
            _eventRepository.Setup(m => m.GetSubeventByParentEventId(eventDtos[2].EventId, 1)).Returns((MpEventDto)null);
            _eventRepository.Setup(m => m.GetSubeventByParentEventId(eventDtos[0].EventId, 1)).Returns(eventDtos[1]);
            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(eventDtos[0].EventId, It.IsAny<List<int>>())).Returns(new List<MpEventParticipantDto>());
            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(eventDtos[1].EventId, It.IsAny<List<int>>())).Returns(new List<MpEventParticipantDto>());
            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(eventDtos[2].EventId, It.IsAny<List<int>>())).Returns(mpEventParticipantDtos);
            _participantRepository.Setup(m => m.GetEventParticipantsByEventAndParticipant(0, It.IsAny<List<int>>())).Returns(new List<MpEventParticipantDto>());

            // Act
            _fixture.CheckForDuplicateSignIns(eventDtos, participantEventMapDto);

            // Assert
            Assert.AreEqual(participantEventMapDto.Participants[0].DuplicateSignIn, true);
        }

        [Test]
        public void ShouldFilterNonRsvpedChildcareParticipants()
        {
            // Arrange
            var phoneNumber = "555-555-5555";
            var siteId = 8;
            const string kioskId = "504c73c1-d664-4ccd-964e-e008e7ce2635";

            var eventDto = new EventDto
            {
                EventId = 1234567,
                EventTypeId = _applicationConfiguration.Object.ChildcareEventTypeId
            };

            _eventService.Setup(m => m.GetCurrentEventForSite(siteId, kioskId)).Returns(GetTestEvent(siteId, _applicationConfiguration.Object.ChildcareEventTypeId));
            _childSigninRepository.Setup(m => m.GetChildrenByPhoneNumber(phoneNumber, true)).Returns(GetMpHouseholdParticipants());
            _contactRepository.Setup(m => m.GetHeadsOfHouseholdByHouseholdId(9988999)).Returns(new List<MpContactDto>());

            _eventRepository.Setup(m => m.GetEventGroupsForEventByGroupTypeId(1234567, _applicationConfiguration.Object.ChildcareGroupTypeId)).Returns(GetGroupsForChildcareEvent());

            var mpHouseholdParticipants = GetMpHouseholdParticipants();
            var participantIds = mpHouseholdParticipants.Participants.Select(r => r.ParticipantId).ToList();

            _participantRepository.Setup(m => m.GetGroupParticipantsByParticipantAndGroupId(
                13245, participantIds)).Returns(GetGroupParticipantsForChildcareEvent);

            // Act
            var result = _fixture.GetChildrenAndEventByPhoneNumber(phoneNumber, siteId, eventDto);

            // Assert
            Assert.AreEqual(1, result.Participants.Count);
        }

        [Test]
        public void ShouldFilterNonRsvpedChildcareParticipantsNullEvent()
        {
            // Arrange
            var phoneNumber = "555-555-5555";
            var siteId = 8;
            const string kioskId = "504c73c1-d664-4ccd-964e-e008e7ce2635";

            var eventDto = new EventDto
            {
                EventId = 1234567,
                EventTypeId = _applicationConfiguration.Object.ChildcareEventTypeId
            };

            _eventService.Setup(m => m.GetCurrentEventForSite(siteId, kioskId)).Returns(GetTestEvent(siteId, _applicationConfiguration.Object.ChildcareEventTypeId));
            _childSigninRepository.Setup(m => m.GetChildrenByPhoneNumber(phoneNumber, true)).Returns(GetMpHouseholdParticipants());
            _contactRepository.Setup(m => m.GetHeadsOfHouseholdByHouseholdId(9988999)).Returns(new List<MpContactDto>());

            _eventRepository.Setup(m => m.GetEventGroupsForEventByGroupTypeId(1234567, _applicationConfiguration.Object.ChildcareGroupTypeId)).Returns(GetGroupsForChildcareEvent());

            var mpHouseholdParticipants = GetMpHouseholdParticipants();
            var participantIds = mpHouseholdParticipants.Participants.Select(r => r.ParticipantId).ToList();

            _participantRepository.Setup(m => m.GetGroupParticipantsByParticipantAndGroupId(
                13245, participantIds)).Returns(GetGroupParticipantsForChildcareEvent);

            // Act
            var result = _fixture.GetChildrenAndEventByPhoneNumber(phoneNumber, siteId, eventDto);

            // Assert
            Assert.AreEqual(1, result.Participants.Count);
        }

        [Test]
        public void ShouldNotFilterKCParticipants()
        {
            // Arrange
            var phoneNumber = "555-555-5555";
            var siteId = 8;
            const string kioskId = "504c73c1-d664-4ccd-964e-e008e7ce2635";

            var eventDto = new EventDto
            {
                EventId = 1234567,
                EventTypeId = 20
            };

            _eventService.Setup(m => m.GetCurrentEventForSite(siteId, kioskId)).Returns(GetTestEvent(siteId, _applicationConfiguration.Object.ChildcareEventTypeId));
            _childSigninRepository.Setup(m => m.GetChildrenByPhoneNumber(phoneNumber, true)).Returns(GetMpHouseholdParticipants());
            _contactRepository.Setup(m => m.GetHeadsOfHouseholdByHouseholdId(9988999)).Returns(new List<MpContactDto>());

            // Act
            var result = _fixture.GetChildrenAndEventByPhoneNumber(phoneNumber, siteId, eventDto);

            // Assert
            Assert.AreEqual(2, result.Participants.Count);
        }

        [Test]
        public void ShouldNotFilterKCParticipantsNullEvent()
        {
            // Arrange
            var phoneNumber = "555-555-5555";
            var siteId = 8;
            const string kioskId = "504c73c1-d664-4ccd-964e-e008e7ce2635";

            var eventDto = new EventDto
            {
                EventId = 1234567,
                EventTypeId = 20
            };

            _eventService.Setup(m => m.GetCurrentEventForSite(siteId, kioskId)).Returns(GetTestEvent(siteId, _applicationConfiguration.Object.ChildcareEventTypeId));
            _childSigninRepository.Setup(m => m.GetChildrenByPhoneNumber(phoneNumber, true)).Returns(GetMpHouseholdParticipants());
            _contactRepository.Setup(m => m.GetHeadsOfHouseholdByHouseholdId(9988999)).Returns(new List<MpContactDto>());

            // Act
            var result = _fixture.GetChildrenAndEventByPhoneNumber(phoneNumber, siteId, eventDto);

            // Assert
            Assert.AreEqual(2, result.Participants.Count);
        }

        [Test]
        public void ShouldNotFilterNewFamilyParticipants()
        {
            // Arrange
            var phoneNumber = "555-555-5555";
            var siteId = 8;
            const string kioskId = "504c73c1-d664-4ccd-964e-e008e7ce2635";

            var eventDto = new EventDto
            {
                EventId = 1234567,
                EventTypeId = 243
            };

            _eventService.Setup(m => m.GetCurrentEventForSite(siteId, kioskId)).Returns(GetTestEvent(siteId, _applicationConfiguration.Object.ChildcareEventTypeId));
            _childSigninRepository.Setup(m => m.GetChildrenByPhoneNumber(phoneNumber, true)).Returns(GetMpHouseholdParticipants());
            _contactRepository.Setup(m => m.GetHeadsOfHouseholdByHouseholdId(9988999)).Returns(new List<MpContactDto>());

            // Act
            var result = _fixture.GetChildrenAndEventByPhoneNumber(phoneNumber, siteId, eventDto, true);

            // Assert
            Assert.AreEqual(2, result.Participants.Count);
        }

        [Test]
        public void ShouldReturnMiddleSchoolParticipants()
        {
            // Arrange
            var phoneNumber = "555-555-5555";
            var siteId = 8;
            var kioskTypeId = 4; // MSM kiosk type
            const string kioskId = "504c73c1-d664-4ccd-964e-e008e7ce2635";

            var msmGroupsIds = new List<int>
            {
                173931,
                173932,
                173933
            };

            var eventDto = new EventDto
            {
                EventId = 1234567,
                EventTypeId = _applicationConfiguration.Object.StudentMinistryGradesSixToEightEventTypeId
            };

            _eventService.Setup(m => m.GetCurrentEventForSite(siteId, kioskId)).Returns(GetTestEvent(siteId, _applicationConfiguration.Object.ChildcareEventTypeId));
            _childSigninRepository.Setup(m => m.GetChildrenByPhoneNumberAndGroupIds(phoneNumber, It.IsAny<List<int>>(), true)).Returns(GetMpHouseholdParticipantsForMSMEvent());
            _contactRepository.Setup(m => m.GetHeadsOfHouseholdByHouseholdId(9988999)).Returns(new List<MpContactDto>());

            // Act
            var result = _fixture.GetChildrenAndEventByPhoneNumber(phoneNumber, siteId, eventDto, true);

            // Assert
            Assert.AreEqual(1, result.Participants.Count(r => msmGroupsIds.Contains(r.GroupId.GetValueOrDefault())));
            Assert.AreEqual(0, result.Participants.Count(r => !msmGroupsIds.Contains(r.GroupId.GetValueOrDefault())));
        }

        [Test]
        public void ShouldReturnHighSchoolParticipants()
        {
            // Arrange
            var phoneNumber = "555-555-5555";
            var siteId = 8;
            var kioskTypeId = 4; // MSM kiosk type
            const string kioskId = "504c73c1-d664-4ccd-964e-e008e7ce2635";

            var hsmGroupsIds = new List<int>
            {
                173927,
                173928,
                173929,
                173930
            };

            var eventDto = new EventDto
            {
                EventId = 1234567,
                EventTypeId = _applicationConfiguration.Object.StudentMinistryGradesSixToEightEventTypeId
            };

            _eventService.Setup(m => m.GetCurrentEventForSite(siteId, kioskId)).Returns(GetTestEvent(siteId, _applicationConfiguration.Object.ChildcareEventTypeId));
            _childSigninRepository.Setup(m => m.GetChildrenByPhoneNumberAndGroupIds(phoneNumber, It.IsAny<List<int>>(), true)).Returns(GetMpHouseholdParticipantsForHSMEvent());
            _contactRepository.Setup(m => m.GetHeadsOfHouseholdByHouseholdId(9988999)).Returns(new List<MpContactDto>());

            // Act
            var result = _fixture.GetChildrenAndEventByPhoneNumber(phoneNumber, siteId, eventDto, true);

            // Assert
            Assert.AreEqual(1, result.Participants.Count(r => hsmGroupsIds.Contains(r.GroupId.GetValueOrDefault())));
            Assert.AreEqual(0, result.Participants.Count(r => !hsmGroupsIds.Contains(r.GroupId.GetValueOrDefault())));
        }

        [Test]
        public void ShouldReturnMiddleAndHighSchoolParticipants()
        {
            // Arrange
            var phoneNumber = "555-555-5555";
            var siteId = 8;
            var kioskTypeId = 4; // MSM kiosk type
            const string kioskId = "504c73c1-d664-4ccd-964e-e008e7ce2635";

            var mpKioskConfig = new MpKioskConfigDto
            {
                KioskTypeId = 4
            };

            var bigGroupsIds = new List<int>
            {
                173927,
                173928,
                173929,
                173930,
                173931,
                173932,
                173933
            };

            var eventDto = new EventDto
            {
                EventId = 1234567,
                EventTypeId = _applicationConfiguration.Object.StudentMinistryGradesSixToEightEventTypeId
            };
            _kioskRepository.Setup(m => m.GetMpKioskConfigByIdentifier(Guid.Parse(kioskId))).Returns(mpKioskConfig);
            _eventService.Setup(m => m.GetCurrentEventForSite(siteId, kioskId)).Returns(GetTestEvent(siteId, _applicationConfiguration.Object.ChildcareEventTypeId));
            _childSigninRepository.Setup(m => m.GetChildrenByPhoneNumberAndGroupIds(phoneNumber, It.IsAny<List<int>>(), true)).Returns(GetMpHouseholdParticipantsForBigEvent());
            _contactRepository.Setup(m => m.GetHeadsOfHouseholdByHouseholdId(9988999)).Returns(new List<MpContactDto>());

            // Act
            var result = _fixture.GetChildrenAndEventByPhoneNumber(phoneNumber, siteId, eventDto, true);

            // Assert
            Assert.AreEqual(2, result.Participants.Count(r => bigGroupsIds.Contains(r.GroupId.GetValueOrDefault())));
            Assert.AreEqual(0, result.Participants.Count(r => !bigGroupsIds.Contains(r.GroupId.GetValueOrDefault())));
        }

        private EventDto GetTestEvent(int siteId, int eventTypeId)
        {
            var eventDto = new EventDto
            {
                EventId = 1234567,
                EventSiteId = siteId,
                EventTypeId = eventTypeId
            };

            return eventDto;
        }

        private MpHouseholdParticipantsDto GetMpHouseholdParticipants()
        {
            var mpHouseholdParticipants = new MpHouseholdParticipantsDto
            {
                HouseholdId = 9988999
            };

            mpHouseholdParticipants.Participants = new List<MpParticipantDto>
            {
                new MpParticipantDto
                {
                    ParticipantId = 1122333
                },
                new MpParticipantDto
                {
                    ParticipantId = 2233444
                }
            };

            return mpHouseholdParticipants;
        }

        private MpHouseholdParticipantsDto GetMpHouseholdParticipantsForMSMEvent()
        {
            var mpHouseholdParticipants = new MpHouseholdParticipantsDto
            {
                HouseholdId = 9988999
            };

            mpHouseholdParticipants.Participants = new List<MpParticipantDto>
            {
                new MpParticipantDto
                {
                    ParticipantId = 2233444,
                    GroupId = 173931,
                    FirstName = "Jimmy",
                    LastName = "Sixthgrade"
                }
            };

            return mpHouseholdParticipants;
        }

        private MpHouseholdParticipantsDto GetMpHouseholdParticipantsForHSMEvent()
        {
            var mpHouseholdParticipants = new MpHouseholdParticipantsDto
            {
                HouseholdId = 9988999
            };

            mpHouseholdParticipants.Participants = new List<MpParticipantDto>
            {
                new MpParticipantDto
                {
                    ParticipantId = 2233444,
                    GroupId = 173928,
                    FirstName = "Bobby",
                    LastName = "EleventhGrade"
                }
            };

            return mpHouseholdParticipants;
        }

        private MpHouseholdParticipantsDto GetMpHouseholdParticipantsForBigEvent()
        {
            var mpHouseholdParticipants = new MpHouseholdParticipantsDto
            {
                HouseholdId = 9988999
            };

            mpHouseholdParticipants.Participants = new List<MpParticipantDto>
            {
                new MpParticipantDto
                {
                    ParticipantId = 2233444,
                    GroupId = 173928,
                    FirstName = "Bobby",
                    LastName = "EleventhGrade"
                },
                new MpParticipantDto
                {
                    ParticipantId = 1122333,
                    GroupId = 173932,
                    FirstName = "Jimmy",
                    LastName = "SixthGrade"
                }
            };

            return mpHouseholdParticipants;
        }

        private List<MpEventGroupDto> GetGroupsForChildcareEvent()
        {
            var mpEventGroupDtos = new List<MpEventGroupDto>
            {
                new MpEventGroupDto
                {
                    EventId = 1234567,
                    GroupId = 13245
                },
                new MpEventGroupDto
                {
                    EventId = 1234567,
                    GroupId = 25346,

                }
            };

            return mpEventGroupDtos;
        }

        private List<MpGroupParticipantDto> GetGroupParticipantsForChildcareEvent()
        {
            var groupParticipants = new List<MpGroupParticipantDto>
            {
                new MpGroupParticipantDto
                {
                    GroupId = 12345,
                    ParticipantId = 1122333
                }
            };

            return groupParticipants;
        }
    }
}
