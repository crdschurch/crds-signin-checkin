using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Crossroads.Utilities.Services.Interfaces;
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
        private readonly IRoomRepository _roomRepository;

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
                                  IRoomRepository roomRepository)
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
        }

        public ParticipantEventMapDto GetChildrenAndEventByPhoneNumber(string phoneNumber, int siteId, EventDto existingEventDto)
        {
            var eventDto = existingEventDto ?? _eventService.GetCurrentEventForSite(siteId);

            var household = _childSigninRepository.GetChildrenByPhoneNumber(phoneNumber);

            if (!household.HasHousehold)
            {
                throw new ApplicationException($"Could not locate household for phone number {phoneNumber}");
            }

            var childrenDtos = Mapper.Map<List<MpParticipantDto>, List<ParticipantDto>>(household.Participants);

            var headsOfHousehold = Mapper.Map<List<ContactDto>>(_contactRepository.GetHeadsOfHouseholdByHouseholdId(household.HouseholdId.Value));

            var participantEventMapDto = new ParticipantEventMapDto
            {
                Contacts = headsOfHousehold,
                Participants = childrenDtos,
                CurrentEvent = eventDto
            };

            return participantEventMapDto;
        }

        public ParticipantEventMapDto SigninParticipants(ParticipantEventMapDto participantEventMapDto)
        {
            List<int> eventIdsToSignIn = CheckAcEventStatus(participantEventMapDto);

            var mpEventParticipantDtoList = SetParticipantsAssignedRoom(participantEventMapDto).ToList();
            mpEventParticipantDtoList.ForEach(r => r.EventId = eventIdsToSignIn[0]);

            // need to create this at the function level to use later in the function
            ParticipantEventMapDto acParticipantEventMapDto = new ParticipantEventMapDto
            {
                Contacts = participantEventMapDto.Contacts,
                Participants = participantEventMapDto.Participants,
                ServicesAttended = participantEventMapDto.ServicesAttended
            };

            // call code to sign into AC here, if they are attending 2 services and there are 2 events
            if (participantEventMapDto.ServicesAttended == 2 && eventIdsToSignIn.Count == 2)
            {
                // get the ac event
                var mpAcEventDto = _eventRepository.GetEventById(eventIdsToSignIn[1]);
                acParticipantEventMapDto.CurrentEvent = Mapper.Map<EventDto>(mpAcEventDto);

                var subEventParticipants = SetParticipantsAssignedRoom(acParticipantEventMapDto).ToList();
                subEventParticipants.ForEach(r => r.EventId = eventIdsToSignIn[1]);
                mpEventParticipantDtoList.AddRange(subEventParticipants);
            }

            // null out the room assignment for both participant records if they can't sign in to one or the other,
            // so that they get a rock
            SyncInvalidSignins(mpEventParticipantDtoList, participantEventMapDto);

            // create participants if they're assigned to a room -- we still need to handle the case where there is an 
            // error and they can't be signed into both events
            var response = new ParticipantEventMapDto
            {
                CurrentEvent = participantEventMapDto.CurrentEvent,
                Participants =
                    _childSigninRepository.CreateEventParticipants(
                        mpEventParticipantDtoList.Where(p => participantEventMapDto.Participants.Find(q => q.Selected && q.ParticipantId == p.ParticipantId) != null && p.HasRoomAssignment).ToList())
                        .Select(Mapper.Map<ParticipantDto>).ToList(),
                Contacts = participantEventMapDto.Contacts
            };

            // set the data fields on the printed participant dto
            if (eventIdsToSignIn.Count == 2)
            {
                foreach (var item in response.Participants.Where(r => r.EventId == eventIdsToSignIn[1]))
                {
                    foreach (var subItem in response.Participants.Where(r => r.ParticipantId == item.ParticipantId && r.EventId == eventIdsToSignIn[0]))
                    {
                        subItem.AssignedSecondaryRoomId = item.AssignedRoomId;
                        subItem.AssignedSecondaryRoomName = item.AssignedRoomName;
                    }
                }

                response.Participants.RemoveAll(r => r.EventId == eventIdsToSignIn[1]);
            }

            // TODO Add back those participants that didn't get a room assigned - should be handled in bumping rules eventually
            response.Participants.AddRange(participantEventMapDto.Participants.Where(p => !p.AssignedRoomId.HasValue && p.Selected));

            response.Participants.ForEach(p => p.Selected = true);

            return response;
        }

        // need to be able to assign to two rooms - which is what signing into AC is
        private IEnumerable<MpEventParticipantDto> SetParticipantsAssignedRoom(ParticipantEventMapDto participantEventMapDto)
        {
            // Get Event and make sure it occures at a valid time
            var eventDto = GetEvent(participantEventMapDto);

            // Get groups that are configured for the event
            var eventGroups = _eventRepository.GetEventGroupsForEvent(participantEventMapDto.CurrentEvent.EventId);

            // Get a list of participants with their groups and expected rooms
            var mpEventParticipantDtoList = SetParticipantsGroupsAndExpectedRooms(eventGroups, participantEventMapDto);

            foreach (var eventParticipant in participantEventMapDto.Participants)
            {
                if (!eventParticipant.Selected) continue;
                var mpEventParticipant = mpEventParticipantDtoList.Find(r => r.ParticipantId == eventParticipant.ParticipantId);

                if (!mpEventParticipant.HasKidsClubGroup)
                {
                    eventParticipant.SignInErrorMessage = $"Please go to the Kids Club Info Desk and give them this label.  ERROR: {eventParticipant.FirstName} is not in a Kids Club Group (DOB: {eventParticipant.DateOfBirth.ToShortDateString() })";
                }
                else if (!mpEventParticipant.HasRoomAssignment)
                {
                    var group = mpEventParticipant.GroupId.HasValue ? _groupRepository.GetGroup(null, mpEventParticipant.GroupId.Value) : null;
                    eventParticipant.SignInErrorMessage = $"Please go to the Kids Club Info Desk and give them this label.  ERROR: '{@group?.Name}' is not assigned to any rooms for {eventDto.EventTitle} for {eventParticipant.FirstName}";
                }
                else
                {
                    SetParticipantsRoomAssignment(eventParticipant, mpEventParticipant, eventGroups);
                }
            }

            return mpEventParticipantDtoList;
        }

        private EventDto GetEvent(ParticipantEventMapDto participantEventMapDto)
        {
            // Get Event and make sure it occures at a valid time
            var eventDto = _eventService.GetEvent(participantEventMapDto.CurrentEvent.EventId);
            if (_eventService.CheckEventTimeValidity(eventDto) == false)
            {
                throw new Exception("Sign-In Not Available For Event " + eventDto.EventId);
            }

            return eventDto;
        }

        private static List<MpEventParticipantDto> SetParticipantsGroupsAndExpectedRooms(List<MpEventGroupDto> eventGroupsForEvent, ParticipantEventMapDto participantEventMapDto)
        {
            var mpEventParticipantDtoList = (
                // Get selected participants
                from participant in participantEventMapDto.Participants.Where(r => r.Selected)

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
            foreach(var bumpingRoom in bumpingRooms)
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
                var printValues = new Dictionary<string, string>
                {
                    {"ChildName", participant.FirstName},
                    {"ChildRoomName1", participant.AssignedRoomName},
                    {"ChildRoomName2", participant.AssignedSecondaryRoomName},
                    {"ChildEventName", participantEventMapDto.CurrentEvent.EventTitle},
                    {"ChildParentName", headsOfHousehold},
                    {"ChildCallNumber", participant.CallNumber},
                    {"ParentCallNumber", participant.CallNumber},
                    {"ParentRoomName1", participant.AssignedRoomName},
                    {"ParentRoomName2", participant.AssignedSecondaryRoomName},
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
                    title = $"Print job for {participantEventMapDto.CurrentEvent.EventTitle}, participant {participant.FirstName} (id #{participant.ParticipantId})",
                    source = "CRDS Checkin"
                };

                _printingService.SendPrintRequest(printRequestDto);
            }

            return participantEventMapDto;
        }

        public void CreateNewFamily(string token, NewFamilyDto newFamilyDto, string kioskIdentifier)
        {
            var newFamilyParticipants = SaveNewFamilyData(token, newFamilyDto);
            CreateGroupParticipants(token, newFamilyParticipants);
            
            var participantEventMapDto = GetChildrenAndEventByPhoneNumber(newFamilyDto.ParentContactDto.PhoneNumber, newFamilyDto.EventDto.EventSiteId, newFamilyDto.EventDto);

            // mark all as Selected so all children will be signed in
            participantEventMapDto.Participants.ForEach(p => p.Selected = true);

            // sign them all into a room
            participantEventMapDto = SigninParticipants(participantEventMapDto);

            // print labels
            PrintParticipants(participantEventMapDto, kioskIdentifier);
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
                ParticipantStartDate = System.DateTime.Now,
                Contact = new MpContactDto
                {
                    FirstName = newFamilyDto.ParentContactDto.FirstName,
                    Nickname = newFamilyDto.ParentContactDto.FirstName,
                    LastName = newFamilyDto.ParentContactDto.LastName,
                    DisplayName = newFamilyDto.ParentContactDto.FirstName + " " + newFamilyDto.ParentContactDto.LastName,
                    HouseholdId = mpHouseholdDto.HouseholdId,
                    HouseholdPositionId = _applicationConfiguration.HeadOfHouseholdId, 
                    Company = false
                }
            };

            // parentNewParticipantDto.Contact.DateOfBirth = null;
            _participantRepository.CreateParticipantWithContact(token, parentNewParticipantDto);

            // Step 3 create the children contacts
            List<MpNewParticipantDto> mpNewChildParticipantDtos = new List<MpNewParticipantDto>();

            foreach (var childContactDto in newFamilyDto.ChildContactDtos)
            {
                MpNewParticipantDto childNewParticipantDto = new MpNewParticipantDto
                {
                    ParticipantTypeId = _applicationConfiguration.AttendeeParticipantType,
                    ParticipantStartDate = System.DateTime.Now,
                    Contact = new MpContactDto
                    {
                        FirstName = childContactDto.FirstName,
                        Nickname = childContactDto.FirstName,
                        LastName = childContactDto.LastName,
                        DisplayName = childContactDto.FirstName + " " + childContactDto.LastName,
                        HouseholdId = mpHouseholdDto.HouseholdId,
                        HouseholdPositionId = _applicationConfiguration.MinorChildId,
                        Company = false,
                        DateOfBirth = childContactDto.DateOfBirth
                    }
                };

                var newParticipant = _participantRepository.CreateParticipantWithContact(token, childNewParticipantDto);
                newParticipant.Contact = childNewParticipantDto.Contact;
                newParticipant.GradeGroupAttributeId = childContactDto.YearGrade;
                mpNewChildParticipantDtos.Add(newParticipant);
            }

            return mpNewChildParticipantDtos;
        }

        // this really can just return void, but we need to get the grade group id on the mp new participant dto
        public void CreateGroupParticipants(string token, List<MpNewParticipantDto> mpParticipantDtos)
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
                    StartDate = System.DateTime.Now,
                    EmployeeRole = false,
                    AutoPromote = true
                };

                groupParticipantDtos.Add(groupParticipantDto);
            }

            _participantRepository.CreateGroupParticipants(token, groupParticipantDtos);
        }

        private MpEventDto GetNextAdventureClubEvent(EventDto eventDto)
        {
            var currentDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

            // get current and future events - not sure how to shrink this down...
            var dailyEvents = _eventRepository.GetEvents(DateTime.Now, currentDay, eventDto.EventSiteId).OrderBy(r => r.EventStartDate);

            if (!dailyEvents.Any())
            {
                return null;
            }

            var eventIds = dailyEvents.Select(r => r.EventId).ToList();

            var subEvents = _eventRepository.GetSubeventsForEvents(eventIds, _applicationConfiguration.AdventureClubEventTypeId);

            foreach (var parentEvent in dailyEvents)
            {
                // look to see if the next event in sequence has a child event of AC - if so, return that AC event id
                if (subEvents.Any(r => r.ParentEventId == parentEvent.EventId && r.EventTypeId == _applicationConfiguration.AdventureClubEventTypeId))
                {
                    return subEvents.First(r => r.ParentEventId == parentEvent.EventId && r.EventTypeId == _applicationConfiguration.AdventureClubEventTypeId);
                }
            }

            // null return event means there's no remaining AC event for that day
            return null;
        }

        // this gets called if the children are potentially signed into a current AC event - NTK if there is another regular event left in the day
        // to sign them into, otherwise they do not get signed into AC
        private MpEventDto GetNextServiceEvent(int currentAcEventSiteId, int eventId)
        {
            var currentDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

            var dailyEvents = _eventRepository.GetEvents(DateTime.Now, currentDay, currentAcEventSiteId).OrderBy(r => r.EventStartDate);

            if (dailyEvents.Any(r => r.ParentEventId == null && r.EventId != eventId))
            {
                // pull the next service event off the list where the service event id doesn't equal the current event's id
                return dailyEvents.First(r => r.EventId != eventId);
            }

            return null;
        }

        // simply return a list of two event ids to check into -- note that the first id is always a 
        // service event id
        private List<int> CheckAcEventStatus(ParticipantEventMapDto participantEventMapDto)
        {
            List<int> returnEventIds = new List<int>();

            if (participantEventMapDto.ServicesAttended == 2)
            {
                var nextAcEvent = GetNextAdventureClubEvent(participantEventMapDto.CurrentEvent);

                if (nextAcEvent != null)
                {
                    // case # 1 - the current AC event's parent event is the current service event - check to 
                    // see if there is another service event that day
                    var nextServiceEvent = GetNextServiceEvent(nextAcEvent.CongregationId, participantEventMapDto.CurrentEvent.EventId);

                    // sign them into a regular service
                    if (nextServiceEvent == null || nextServiceEvent.EventId == participantEventMapDto.CurrentEvent.EventId)
                    {
                        returnEventIds.Add(participantEventMapDto.CurrentEvent.EventId);
                        return returnEventIds;
                    }

                    // if the event being signed into is an ac event and there is a later ac event,
                    // sign them into the current service and the later ac event
                    if (nextServiceEvent.EventId == participantEventMapDto.CurrentEvent.EventId)
                    {
                        returnEventIds.Add(participantEventMapDto.CurrentEvent.EventId);
                        returnEventIds.Add(nextAcEvent.EventId);
                        return returnEventIds;
                    }

                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse -- JPC
                    if (nextServiceEvent != null && nextAcEvent.ParentEventId == participantEventMapDto.CurrentEvent.EventId)
                    {
                        returnEventIds.Add(participantEventMapDto.CurrentEvent.EventId);
                        returnEventIds.Add(nextAcEvent.EventId);
                        return returnEventIds;
                    }
                }
            }

            // just return the regular service event if there is no AC event
            returnEventIds.Add(participantEventMapDto.CurrentEvent.EventId);
            return returnEventIds;
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
    }
}
