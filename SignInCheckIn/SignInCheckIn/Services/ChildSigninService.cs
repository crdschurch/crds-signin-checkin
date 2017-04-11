using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Crossroads.Utilities.Services.Interfaces;
using MinistryPlatform.Translation.Models;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Printing.Utilities.Models;
using Printing.Utilities.Services.Interfaces;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Services.Interfaces;

namespace SignInCheckIn.Services
{
    public class ChildSigninService : IChildSigninService
    {
        private readonly IChildSigninRepository _childSigninRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IEventService _eventService;
        private readonly IGroupRepository _groupRepository;
        private readonly IKioskRepository _kioskRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IPrintingService _printingService;
        private readonly IPdfEditor _pdfEditor;
        private readonly IParticipantRepository _participantRepository;
        private readonly IApplicationConfiguration _applicationConfiguration;
        private readonly IGroupLookupRepository _groupLookupRepository;
        private readonly IAttributeRepository _attributeRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly ISignInLogic _signInLogic;

        private readonly int _defaultEarlyCheckinPeriod;
        private readonly int _defaultLateCheckinPeriod;

        public ChildSigninService(IChildSigninRepository childSigninRepository,
                                  IEventRepository eventRepository,
                                  IGroupRepository groupRepository,
                                  IEventService eventService,
                                  IPdfEditor pdfEditor,
                                  IPrintingService printingService,
                                  IContactRepository contactRepository,
                                  IKioskRepository kioskRepository,
                                  IParticipantRepository participantRepository,
                                  IApplicationConfiguration applicationConfiguration,
                                  IGroupLookupRepository groupLookupRepository,
                                  IRoomRepository roomRepository,
                                  IConfigRepository configRepository,
                                  IAttributeRepository attributeRepository,
                                  ISignInLogic signInLogic)
        {
            _childSigninRepository = childSigninRepository;
            _eventRepository = eventRepository;
            _groupRepository = groupRepository;
            _eventService = eventService;
            _kioskRepository = kioskRepository;
            _contactRepository = contactRepository;
            _printingService = printingService;
            _pdfEditor = pdfEditor;
            _participantRepository = participantRepository;
            _applicationConfiguration = applicationConfiguration;
            _groupLookupRepository = groupLookupRepository;
            _roomRepository = roomRepository;
            _attributeRepository = attributeRepository;
            _signInLogic = signInLogic;

            _defaultEarlyCheckinPeriod = int.Parse(configRepository.GetMpConfigByKey("DefaultEarlyCheckIn").Value);
            _defaultLateCheckinPeriod = int.Parse(configRepository.GetMpConfigByKey("DefaultLateCheckIn").Value);
        }

        public ParticipantEventMapDto GetChildrenAndEventByHouseholdId(int householdId, int siteId)
        {
            var eventDto = _eventService.GetCurrentEventForSite(siteId);

            var household = _childSigninRepository.GetChildrenByHouseholdId(householdId, eventDto.EventId);

            var childrenDtos = Mapper.Map<List<MpParticipantDto>, List<ParticipantDto>>(household);

            var headsOfHousehold = Mapper.Map<List<ContactDto>>(_contactRepository.GetHeadsOfHouseholdByHouseholdId(householdId));

            var participantEventMapDto = new ParticipantEventMapDto
            {
                Contacts = headsOfHousehold,
                Participants = childrenDtos,
                CurrentEvent = eventDto
            };

            return participantEventMapDto;
        }

        // mod this to include the lookup for MSM/HSM
        public ParticipantEventMapDto GetChildrenAndEventByPhoneNumber(string phoneNumber, int siteId, EventDto existingEventDto, bool newFamilyRegistration = false)
        {
            // this will have to check if it's a childcare event
            var eventDto = existingEventDto ?? _eventService.GetCurrentEventForSite(siteId);

            //if (new)
            var eventSpecificGroupIds = GetGroupIdsByEventTypeId(eventDto.EventTypeId);
            MpHouseholdParticipantsDto household;

            if (eventSpecificGroupIds.Count == 0)
            {
                household = _childSigninRepository.GetChildrenByPhoneNumber(phoneNumber, true);
            }
            else
            {
                household = _childSigninRepository.GetChildrenByPhoneNumberAndGroupIds(phoneNumber, eventSpecificGroupIds, true);
            }

            if (!household.HouseholdId.HasValue && household.HouseholdId != 0)
            {
                throw new ApplicationException($"Could not locate household for phone number {phoneNumber}");
            }

            // check the household participants here if this is a childcare search - we do not do this check on a new family
            // registration, as we don't require them to have an RSVP
            if (eventDto.EventTypeId == _applicationConfiguration.ChildcareEventTypeId && household.Participants.Any() && newFamilyRegistration == false)
            {
                // this may be an unwarranted assumption that there will only be one CC group - consider edge cases
                var childcareEventGroup = _eventRepository.GetEventGroupsForEventByGroupTypeId(
                    eventDto.EventId,
                    _applicationConfiguration.ChildcareGroupTypeId).First();

                var childcareGroupParticipants = _participantRepository.GetGroupParticipantsByParticipantAndGroupId(childcareEventGroup.GroupId,
                                                                                                                    household.Participants.Select(r => r.ParticipantId).ToList());

                household.Participants = household.Participants.Where(item => childcareGroupParticipants.Any(r => r.ParticipantId == item.ParticipantId)).ToList();
            }

            var childrenDtos = Mapper.Map<List<MpParticipantDto>, List<ParticipantDto>>(household.Participants);

            var headsOfHousehold = Mapper.Map<List<ContactDto>>(_contactRepository.GetHeadsOfHouseholdByHouseholdId(household.HouseholdId.Value));

            var participantEventMapDto = new ParticipantEventMapDto
            {
                Contacts = headsOfHousehold,
                Participants = childrenDtos,
                CurrentEvent = eventDto
            };

            participantEventMapDto.HouseholdPhoneNumber = phoneNumber;
            participantEventMapDto.HouseholdId = household.HouseholdId.GetValueOrDefault();

            return participantEventMapDto;
        }

        // this is a super specific and hardcoded function - we may want to consider adding a table
        // or some other form of lookup to handle this in the future - we could theoretically
        // get the group ids by the event groups on an event, but this might be complicated by having
        // groups we're not accounting for
        private List<int> GetGroupIdsByEventTypeId(int eventTypeId)
        {
            if (eventTypeId == _applicationConfiguration.StudentMinistryGradesSixToEightEventTypeId)
            {
                return new List<int>
                {
                    _applicationConfiguration.MsmSixth,
                    _applicationConfiguration.MsmSeventh,
                    _applicationConfiguration.MsmEighth
                };
            }

            if (eventTypeId == _applicationConfiguration.StudentMinistryGradesNineToTwelveEventTypeId)
            {
                return new List<int>
                {
                    _applicationConfiguration.HighSchoolNinth,
                    _applicationConfiguration.HighSchoolTenth,
                    _applicationConfiguration.HighSchoolEleventh,
                    _applicationConfiguration.HighSchoolTwelfth
                };
            }

            if (eventTypeId == _applicationConfiguration.BigEventTypeId)
            {
                return new List<int>
                {
                    _applicationConfiguration.MsmSixth,
                    _applicationConfiguration.MsmSeventh,
                    _applicationConfiguration.MsmEighth,
                    _applicationConfiguration.HighSchoolNinth,
                    _applicationConfiguration.HighSchoolTenth,
                    _applicationConfiguration.HighSchoolEleventh,
                    _applicationConfiguration.HighSchoolTwelfth
                };
            }

            return new List<int>();
        } 

        public ParticipantEventMapDto SigninParticipants(ParticipantEventMapDto participantEventMapDto)
        {
            // create participant records for guests, and assign to a group
            if (participantEventMapDto.Participants.Any(r => r.GuestSignin == true))
            {
                ProcessGuestSignins(participantEventMapDto);
            }

            // check the current and next event set to make sure they're not signed in to one of those events already
            var eventsForSignin = GetEventsForSignin(participantEventMapDto);
            CheckForDuplicateSignIns(eventsForSignin, participantEventMapDto);

            // this needs to be a list of participants, not a list of event participants - the automapping
            // needs to happen in the logic class
            var rawParticipants = _signInLogic.SignInParticipants(participantEventMapDto, eventsForSignin);

            // TODO: Determine that there doesn't need to be anything further done with assigning the participants this way, like
            // some logic around it
            var response = new ParticipantEventMapDto
            {
                CurrentEvent = participantEventMapDto.CurrentEvent,
                Contacts = participantEventMapDto.Contacts,
                Participants = rawParticipants
            };

            // set checkin household data on the participants
            response.Participants.ForEach(r => {
                r.CheckinHouseholdId = participantEventMapDto.HouseholdId;
                r.CheckinPhone = participantEventMapDto.HouseholdPhoneNumber;
            });

            // sort the participants, and use only the first participant in each group as the return print value thing
            var groupedParticipants = SetParticipantsPrintInformation(response.Participants, eventsForSignin);
            response.Participants = groupedParticipants.Select(r => r.First()).ToList();

            // Add back those participants that didn't get a room assigned - this may be able to be removed
            // TODO: Verify this can be removed
            //response.Participants.AddRange(participantEventMapDto.Participants.Where(p => !p.AssignedRoomId.HasValue && p.Selected));
            response.Participants.ForEach(p => p.Selected = true);

            return response;
        }

        // we check on participants now, not by the event set, to determine how to print - each event participant set is handled individually
        private List<List<ParticipantDto>> SetParticipantsPrintInformation(List<ParticipantDto> participants, IReadOnlyList<MpEventDto> eventsForSignin)
        {
            var groupedParticipants = participants.GroupBy(r => r.ParticipantId).Select(r => r.ToList()).ToList();

            foreach (var groupedParticipantSet in groupedParticipants)
            {
                SetCallNumber(groupedParticipantSet[0], groupedParticipantSet[0].EventParticipantId);

                // if there is a second participant record, set their print data
                if (groupedParticipantSet.Count > 1)
                {
                    SetCallNumber(groupedParticipantSet[1], groupedParticipantSet[0].EventParticipantId);

                    // set the information for printing on the print cards
                    groupedParticipantSet[0].AssignedSecondaryRoomId = groupedParticipantSet[1].AssignedRoomId;
                    groupedParticipantSet[0].AssignedSecondaryRoomName = groupedParticipantSet[1].AssignedRoomName;
                    var participantEvent = eventsForSignin.Single(e => e.EventId == groupedParticipantSet[1].EventId);
                    groupedParticipantSet[0].EventIdSecondary = participantEvent.ParentEventId.HasValue ? participantEvent.ParentEventId.Value : participantEvent.EventId;
                }
            }

            var mpParticipantDtos = participants.Where(r => r.EventParticipantId != 0).Select(Mapper.Map<MpEventParticipantDto>).ToList();
            _participantRepository.UpdateEventParticipants(mpParticipantDtos);
            return groupedParticipants;
        }

        private void SetCallNumber(ParticipantDto participant, int eventParticipantId)
        {
            var callNumber = $"0000{eventParticipantId}";
            participant.CallNumber = callNumber.Substring(callNumber.Length - 4);
        }

        // need to be able to assign to two rooms - which is what signing into AC is
        private IEnumerable<MpEventParticipantDto> SetParticipantsAssignedRoom(ParticipantEventMapDto participantEventMapDto, bool checkEventTime)
        {
            // JPC - need to handle already-assigned event participants here

            // Get Event and make sure it occures at a valid time
            var eventDto = GetEvent(participantEventMapDto.CurrentEvent.EventId, checkEventTime);

            // Get groups that are configured for the event
            var eventGroups = _eventRepository.GetEventGroupsForEvent(participantEventMapDto.CurrentEvent.EventId);

            // Get a list of participants with their groups and expected rooms - maps groups onto participants
            var mpEventParticipantDtoList = SetParticipantsGroupsAndExpectedRooms(eventGroups, participantEventMapDto);

            foreach (var eventParticipant in participantEventMapDto.Participants)
            {
                if (!eventParticipant.Selected)
                {
                    continue;
                }

                if (eventParticipant.DuplicateSignIn == true)
                {
                    eventParticipant.SignInErrorMessage = $"{eventParticipant.Nickname} is already signed in for this event.";
                    continue;
                }

                var mpEventParticipant = mpEventParticipantDtoList.Find(r => r.ParticipantId == eventParticipant.ParticipantId);

                if (!mpEventParticipant.HasKidsClubGroup)
                {
                    eventParticipant.SignInErrorMessage = $"Age/Grade Group Not Assigned. {eventParticipant.Nickname} is not in a Kids Club Group (DOB: {eventParticipant.DateOfBirth.ToShortDateString() })";
                }

                else if (!mpEventParticipant.HasRoomAssignment)
                {
                    var group = mpEventParticipant.GroupId.HasValue ? _groupRepository.GetGroup(null, mpEventParticipant.GroupId.Value) : null;
                    eventParticipant.SignInErrorMessage = $"There are no '{@group?.Name}' rooms open during the {eventDto.EventTitle} for {eventParticipant.Nickname}";
                }
                else
                {
                    SetParticipantsRoomAssignment(eventParticipant, mpEventParticipant, eventGroups);
                }
            }

            return mpEventParticipantDtoList;
        }

        private EventDto GetEvent(int eventId,  bool checkEventTime)
        {
            // Get Event and make sure it occures at a valid time
            var eventDto = _eventService.GetEvent(eventId);
            if (checkEventTime && _eventService.CheckEventTimeValidity(eventDto) == false)
            {
                throw new Exception("Sign-In Not Available For Event " + eventDto.EventId);
            }

            return eventDto;
        }

        private static List<MpEventParticipantDto> SetParticipantsGroupsAndExpectedRooms(List<MpEventGroupDto> eventGroupsForEvent, ParticipantEventMapDto participantEventMapDto)
        {
            var mpEventParticipantDtoList = (
                // Get selected participants
                from participant in participantEventMapDto.Participants.Where(r => r.Selected && r.DuplicateSignIn == false)

                // Get the event group id that they belong to
                let eventGroup = participant.GroupId == null ? null : eventGroupsForEvent.Find(eg => eg.GroupId == participant.GroupId)

                // Create the Event Participant
                select new MpEventParticipantDto
                {
                    EventId = participantEventMapDto.CurrentEvent.EventId,
                    ParticipantId = participant.ParticipantId,
                    ParticipantStatusId = 3, // Status ID of 3 = "Attended"
                    FirstName = participant.FirstName,
                    LastName = participant.LastName,
                    Nickname = participant.Nickname,
                    TimeIn = DateTime.Now,
                    OpportunityId = null,
                    RoomId = eventGroup?.RoomReservation.RoomId,
                    GroupId = participant.GroupId
                }
            ).ToList();

            return mpEventParticipantDtoList;
        }

        private void SetParticipantsRoomAssignment(ParticipantDto eventParticipant, MpEventParticipantDto mpEventParticipant, IEnumerable<MpEventGroupDto> eventGroups)
        {
            var assignedRoomId = mpEventParticipant.RoomId;
            if (assignedRoomId == null) return;

            var assignedRoom = eventGroups.First(eg => eg.RoomReservation.RoomId == assignedRoomId.Value).RoomReservation;
            var signedAndCheckedIn = (assignedRoom.CheckedIn ?? 0) + (assignedRoom.SignedIn ?? 0);

            mpEventParticipant.RoomId = null;

            if (!assignedRoom.AllowSignIn || assignedRoom.Capacity <= signedAndCheckedIn) {
                ProcessBumpingRules(eventParticipant, mpEventParticipant, assignedRoom);
                return;
            }

            assignedRoom.SignedIn = (assignedRoom.SignedIn ?? 0) + 1;
            eventParticipant.AssignedRoomId = assignedRoom.RoomId;
            mpEventParticipant.RoomId = assignedRoom.RoomId;
            mpEventParticipant.RoomName = assignedRoom.RoomName;
        }

        private void ProcessBumpingRules(ParticipantDto eventParticipant, MpEventParticipantDto mpEventParticipant, MpEventRoomDto expectedRoomDto)
        {
            if (expectedRoomDto.EventRoomId == null) return;
            var bumpingRooms = _roomRepository.GetBumpingRoomsForEventRoom(mpEventParticipant.EventId, expectedRoomDto.EventRoomId ?? 0);

            // go through the bumping rooms in priority order and get the first one that is open and has capacity
            if (bumpingRooms == null)
            {
                return;
            }
            foreach (var bumpingRoom in bumpingRooms)
            {
                // check if open and has capacity
                var signedAndCheckedIn = bumpingRoom.CheckedIn + bumpingRoom.SignedIn;
                if (!bumpingRoom.AllowSignIn || bumpingRoom.Capacity <= signedAndCheckedIn) continue;

                eventParticipant.AssignedRoomId = bumpingRoom.RoomId;
                mpEventParticipant.RoomId = bumpingRoom.RoomId;
                mpEventParticipant.RoomName = bumpingRoom.RoomName;
                return;
            }
        }

        public ParticipantEventMapDto PrintParticipant(int eventParticipantId, string kioskIdentifier, string token)
        {
            var participantEventMapDto = GetParticipantEventMapDtoByEventParticipant(eventParticipantId, token);
            return PrintParticipants(participantEventMapDto, kioskIdentifier);
        }

        private ParticipantEventMapDto GetParticipantEventMapDtoByEventParticipant(int eventParticipantId, string token)
        {
            // Get participant from event participant id
            var participants = new List<ParticipantDto>();
            var participant = Mapper.Map<ParticipantDto>(_participantRepository.GetEventParticipantByEventParticipantId(token, eventParticipantId));
            participant.Selected = true;
            participants.Add(participant);

            // Get event from participants event id
            var currentEvent = _eventService.GetEvent(participant.EventId);

            // Get Contact records of Heads of Household
            var headOfHouseholds = new List<ContactDto>();
            if (participant.CheckinHouseholdId.HasValue)
            {
                headOfHouseholds = _contactRepository.GetHeadsOfHouseholdByHouseholdId(participant.CheckinHouseholdId.Value).Select(Mapper.Map<ContactDto>).ToList();
            }

            return new ParticipantEventMapDto
            {
                Participants = participants,
                CurrentEvent = currentEvent,
                Contacts = headOfHouseholds
            };
        }

        public ParticipantEventMapDto PrintParticipants(ParticipantEventMapDto participantEventMapDto, string kioskIdentifier)
        {
            var kioskConfig = _kioskRepository.GetMpKioskConfigByIdentifier(Guid.Parse(kioskIdentifier));
            MpPrinterMapDto kioskPrinterMap;

            if (kioskConfig.PrinterMapId != null)
            {
                kioskPrinterMap = _kioskRepository.GetPrinterMapById(kioskConfig.PrinterMapId.GetValueOrDefault());
            }
            else
            {
                throw new Exception("Printer Map Id Not Set For Kisok " + kioskConfig.KioskConfigId);
            }

            var headsOfHousehold = string.Join(", ", participantEventMapDto.Contacts.Select(c => $"{c.Nickname} {c.LastName}").ToArray());

            foreach (var participant in participantEventMapDto.Participants.Where(r => r.Selected))
            {
                // the AssignedRoom and AssignedSecondaryRoom are not necessarily the first and second
                // chronologically. So if there are two events, lets get the event id's for each
                // (EventId and EventIdSecondary) and see if we should switch them around so they
                // print in order on the tag
                var firstRoomName = participant.AssignedRoomName;
                var secondRoomName = participant.AssignedSecondaryRoomName;
                if (participant.AssignedSecondaryRoomId != null)
                {
                    if (participantEventMapDto.CurrentEvent.EventId != participant.EventId)
                    {
                        firstRoomName = participant.AssignedSecondaryRoomName;
                        secondRoomName = participant.AssignedRoomName;
                    }
                }
                var printValues = new Dictionary<string, string>
                {
                    {"ChildName", participant.Nickname},
                    {"ChildRoomName1", firstRoomName},
                    {"ChildRoomName2", secondRoomName},
                    {"ChildEventName", participantEventMapDto.CurrentEvent.EventTitle},
                    {"ChildParentName", headsOfHousehold},
                    {"ChildCallNumber", participant.CallNumber},
                    {"ParentCallNumber", participant.CallNumber},
                    {"ParentRoomName1", firstRoomName},
                    {"ParentRoomName2", secondRoomName},
                    {"Informative1", "This label is worn by a parent/guardian"},
                    {"Informative2", "You must have this label to pick up your child"},
                    {"ErrorText", participant.SignInErrorMessage}
                };

                // Choose the correct label template
                var labelTemplate = participant.ErrorSigningIn
                    ? Properties.Resources.Error_Label
                    : participant.NotSignedIn ? Properties.Resources.Activity_Kit_Label : Properties.Resources.Checkin_KC_Label;
                var mergedPdf = _pdfEditor.PopulatePdfMergeFields(labelTemplate, printValues);

                var printRequestDto = new PrintRequestDto
                {
                    printerId = kioskPrinterMap.PrinterId,
                    content = mergedPdf + "=",
                    contentType = "pdf_base64",
                    title = $"Print job for {participantEventMapDto.CurrentEvent.EventTitle}, participant {participant.Nickname} (id #{participant.ParticipantId})",
                    source = "CRDS Checkin"
                };

                _printingService.SendPrintRequest(printRequestDto);
            }

            return participantEventMapDto;
        }

        public ParticipantEventMapDto CreateNewFamily(string token, NewFamilyDto newFamilyDto, string kioskIdentifier)
        {
            var newFamilyParticipants = SaveNewFamilyData(token, newFamilyDto);
            CreateGroupParticipants(token, newFamilyParticipants);

            var participantEventMapDto = GetChildrenAndEventByPhoneNumber(newFamilyDto.ParentContactDto.PhoneNumber, newFamilyDto.EventDto.EventSiteId, newFamilyDto.EventDto, true);

            // mark all as selected so they get signed in, but guard against an exception with no participants
            if (participantEventMapDto.Participants.Any())
            {
                participantEventMapDto.Participants.ForEach(p => p.Selected = true);

                // sign them all into a room
                participantEventMapDto = SigninParticipants(participantEventMapDto);

                // print labels
                PrintParticipants(participantEventMapDto, kioskIdentifier);
            }

            return participantEventMapDto;
        }

        public List<MpNewParticipantDto> SaveNewFamilyData(string token, NewFamilyDto newFamilyDto)
        {
            // Step 1 - create the household
            MpHouseholdDto mpHouseholdDto = new MpHouseholdDto
            {
                HouseholdName = newFamilyDto.ParentContactDto.LastName,
                HomePhone = newFamilyDto.ParentContactDto.PhoneNumber,
                CongregationId = newFamilyDto.EventDto.EventSiteId,
                HouseholdSourceId = _applicationConfiguration.KidsClubRegistrationSourceId
            };

            mpHouseholdDto = _contactRepository.CreateHousehold(token, mpHouseholdDto);

            // Step 2 - create the parent contact w/participant
            MpNewParticipantDto parentNewParticipantDto = new MpNewParticipantDto
            {
                ParticipantTypeId = _applicationConfiguration.AttendeeParticipantType,
                ParticipantStartDate = DateTime.Now,
                Contact = new MpContactDto
                {
                    FirstName = newFamilyDto.ParentContactDto.FirstName,
                    Nickname = newFamilyDto.ParentContactDto.FirstName,
                    LastName = newFamilyDto.ParentContactDto.LastName,
                    DisplayName = newFamilyDto.ParentContactDto.LastName + ", " + newFamilyDto.ParentContactDto.FirstName,
                    HouseholdId = mpHouseholdDto.HouseholdId,
                    HouseholdPositionId = _applicationConfiguration.HeadOfHouseholdId,
                    Company = false
                }
            };

            // parentNewParticipantDto.Contact.DateOfBirth = null;
            _participantRepository.CreateParticipantWithContact(parentNewParticipantDto);

            // Step 3 create the children contacts
            List<MpNewParticipantDto> mpNewChildParticipantDtos = new List<MpNewParticipantDto>();

            foreach (var childContactDto in newFamilyDto.ChildContactDtos)
            {
                var newParticipant = CreateNewParticipantWithContact(childContactDto.FirstName,
                                                childContactDto.LastName,
                                                childContactDto.DateOfBirth,
                                                childContactDto.YearGrade,
                                                mpHouseholdDto.HouseholdId,
                                                _applicationConfiguration.MinorChildId
                    );

                mpNewChildParticipantDtos.Add(newParticipant);

            }

            return mpNewChildParticipantDtos;
        }

        // this really can just return void, but we need to get the grade group id on the mp new participant dto
        public List<MpGroupParticipantDto> CreateGroupParticipants(string token, List<MpNewParticipantDto> mpParticipantDtos)
        {
            // Step 4 - create the group participants
            List<MpGroupParticipantDto> groupParticipantDtos = new List<MpGroupParticipantDto>();

            foreach (var tempItem in mpParticipantDtos)
            {
                MpGroupParticipantDto groupParticipantDto = new MpGroupParticipantDto
                {
                    GroupId = _groupLookupRepository.GetGroupId(tempItem.Contact.DateOfBirth ?? new DateTime(), tempItem.GradeGroupAttributeId),
                    ParticipantId = tempItem.ParticipantId,
                    GroupRoleId = _applicationConfiguration.GroupRoleMemberId,
                    StartDate = DateTime.Now,
                    EmployeeRole = false,
                    AutoPromote = true
                };

                groupParticipantDtos.Add(groupParticipantDto);
            }

            return _participantRepository.CreateGroupParticipants(token, groupParticipantDtos);
        }

        // this will pull the current event set and next event set for the site - logic to determine which
        // events to sign into now lives in the signin logic class
        public List<MpEventDto> GetEventsForSignin(ParticipantEventMapDto participantEventMapDto)
        {
            var dateToday = DateTime.Parse(DateTime.Now.ToShortDateString());

            var dailyEvents = _eventRepository.GetEvents(dateToday, dateToday, participantEventMapDto.CurrentEvent.EventSiteId, true)
                .Where(r => CheckEventTimeValidity(r)).OrderBy(r => r.EventStartDate).ToList();

            var eligibleEvents = new List<MpEventDto>();

            // pull off (at most) the top 2 service events
            var serviceEventSet = dailyEvents.Where(r => r.ParentEventId == null).Take(2).ToList();

            // we need to get first two event services, and then the matching ac events
            for (int i = 0; i < serviceEventSet.Count; i++)
            {
                eligibleEvents.Add(serviceEventSet[i]);

                if (dailyEvents.Any(r => r.ParentEventId == serviceEventSet[i].EventId))
                {
                    eligibleEvents.Add(dailyEvents.First(r => r.ParentEventId == serviceEventSet[i].EventId));
                }
            }

            return eligibleEvents;
        }

        private bool CheckEventTimeValidity(MpEventDto mpEventDto)
        {
            // check to see if the event's start is equal to or later than the time minus the offset period
            var offsetPeriod = DateTime.Now.AddMinutes(-(mpEventDto.EarlyCheckinPeriod ?? _defaultEarlyCheckinPeriod));
            return mpEventDto.EventStartDate >= offsetPeriod;
        }

        private void SyncInvalidSignins(List<MpEventParticipantDto> mpEventParticipantDtoList, ParticipantEventMapDto participantEventMapDto)
        {
            // null out the room assignment for both participant records if they can't sign in to one or the other,
            // so that they get a rock
            foreach (var participantItem in mpEventParticipantDtoList.Where(participantItem => mpEventParticipantDtoList.Any(r => r.HasRoomAssignment == false && r.ParticipantId == participantItem.ParticipantId)))
            {
                foreach (var subItem in mpEventParticipantDtoList.Where(r => r.ParticipantId == participantItem.ParticipantId))
                {
                    subItem.RoomId = null;
                }

                foreach (var subItem in participantEventMapDto.Participants.Where(r => r.ParticipantId == participantItem.ParticipantId))
                {
                    subItem.AssignedRoomId = null;
                }
            }
        }

        public MpNewParticipantDto CreateNewParticipantWithContact(string firstName, string lastName,
            DateTime dateOfBirth, int? gradeGroupId, int householdId, int householdPositionId, bool? isSpecialNeeds = false, int? genderId = 0)
        {
            MpNewParticipantDto childNewParticipantDto = new MpNewParticipantDto
            {
                ParticipantTypeId = _applicationConfiguration.AttendeeParticipantType,
                ParticipantStartDate = DateTime.Now,
                Contact = new MpContactDto
                {
                    FirstName = firstName,
                    Nickname = firstName,
                    LastName = lastName,
                    DisplayName = lastName + ", " + firstName,
                    HouseholdId = householdId,
                    HouseholdPositionId = householdPositionId,
                    Company = false,
                    DateOfBirth = dateOfBirth,
                    GenderId = genderId.Value
                }
            };

            if (genderId.HasValue && genderId.Value > 0)
            {
                childNewParticipantDto.Contact.GenderId = genderId.Value;
            }

            if (gradeGroupId.HasValue && gradeGroupId > 0)
            {
                childNewParticipantDto.GradeGroupAttributeId = gradeGroupId;
            }
            var newParticipant = _participantRepository.CreateParticipantWithContact(childNewParticipantDto);
            newParticipant.Contact = childNewParticipantDto.Contact;

            if (isSpecialNeeds == true)
            {
                var newSpecialNeedsAttribute = new MpContactAttributeDto()
                {
                    Contact_ID = newParticipant.ContactId.Value,
                    Attribute_ID = _applicationConfiguration.SpecialNeedsAttributeId,
                    Start_Date = DateTime.Now
                };
                _attributeRepository.CreateContactAttribute(newSpecialNeedsAttribute);
            }

            return newParticipant;
        }

        public void ProcessGuestSignins(ParticipantEventMapDto participantEventMapDto)
        {
            List<MpNewParticipantDto> newGuestParticipantDtos = new List<MpNewParticipantDto>();

            foreach (var guestParticipant in participantEventMapDto.Participants.Where(r => r.GuestSignin == true))
            {
                var newGuestParticipantDto = CreateNewParticipantWithContact(guestParticipant.FirstName,
                                                guestParticipant.LastName,
                                                guestParticipant.DateOfBirth,
                                                guestParticipant.YearGrade,
                                                _applicationConfiguration.GuestHouseholdId,
                                                _applicationConfiguration.MinorChildId
                    );

                guestParticipant.ParticipantId = newGuestParticipantDto.ParticipantId;

                newGuestParticipantDtos.Add(newGuestParticipantDto);
            }

            var newGroupParticipants = CreateGroupParticipants(null, newGuestParticipantDtos);

            // get the group id and assign it to the participant dto for signin
            foreach (var guest in participantEventMapDto.Participants.Where(r => r.GuestSignin == true))
            {
                guest.GroupId = newGroupParticipants.First(r => r.ParticipantId == guest.ParticipantId).GroupId;
                guest.Selected = true;
            }
        }

        public bool ReverseSignin(string token, int eventParticipantId)
        {
            // load the event participant, check their status
            var mpEventParticipantDto = _participantRepository.GetEventParticipantByEventParticipantId(token, eventParticipantId);

            if (mpEventParticipantDto.ParticipantStatusId == _applicationConfiguration.CheckedInParticipationStatusId)
            {
                return false;
            }
            else
            {
                mpEventParticipantDto.ParticipantStatusId = _applicationConfiguration.CancelledParticipationStatusId;
                mpEventParticipantDto.EndDate = DateTime.Now;

                List<MpEventParticipantDto> mpEventParticipantDtos = new List<MpEventParticipantDto>
                {
                    mpEventParticipantDto
                };

                _participantRepository.UpdateEventParticipants(mpEventParticipantDtos);

                return true;
            }
        }

        public void CheckForDuplicateSignIns(List<MpEventDto> eventsForSignin, ParticipantEventMapDto participantEventMapDto)
        {
            var eventIds = eventsForSignin.Select(r => r.EventId).ToList();

            foreach (var eventItemId in eventIds)
            {
                var signedInParticipants = _participantRepository.GetEventParticipantsByEventAndParticipant(
                    eventItemId,
                    participantEventMapDto.Participants.Select(r => r.ParticipantId).ToList());

                foreach (var participant in participantEventMapDto.Participants)
                {
                    if (signedInParticipants.Any(r => r.ParticipantId == participant.ParticipantId))
                    {
                        participant.DuplicateSignIn = true;
                    }
                }
            }
        }
    }
}
