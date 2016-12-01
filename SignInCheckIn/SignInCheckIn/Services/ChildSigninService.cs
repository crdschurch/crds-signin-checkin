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

        public ChildSigninService(IChildSigninRepository childSigninRepository,
                                  IEventRepository eventRepository,
                                  IGroupRepository groupRepository,
                                  IEventService eventService,
                                  IPdfEditor pdfEditor,
                                  IPrintingService printingService,
                                  IContactRepository contactRepository,
                                  IKioskRepository kioskRepository,
                                  IParticipantRepository participantRepository,
                                  IApplicationConfiguration applicationConfiguration)
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
        }

        public ParticipantEventMapDto GetChildrenAndEventByPhoneNumber(string phoneNumber, int siteId)
        {
            var eventDto = _eventService.GetCurrentEventForSite(siteId);
            var householdId = _childSigninRepository.GetHouseholdIdByPhoneNumber(phoneNumber);
            if (householdId == null)
            {
                throw new ApplicationException($"Could not locate household for phone number {phoneNumber}");
            }

            var mpChildren = _childSigninRepository.GetChildrenByHouseholdId(householdId, Mapper.Map<MpEventDto>(eventDto));
            var childrenDtos = Mapper.Map<List<MpParticipantDto>, List<ParticipantDto>>(mpChildren);

            var headsOfHousehold = Mapper.Map<List<ContactDto>>(_contactRepository.GetHeadsOfHouseholdByHouseholdId(householdId.Value));

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
            var eventDto = _eventService.GetEvent(participantEventMapDto.CurrentEvent.EventId);

            if (_eventService.CheckEventTimeValidity(eventDto) == false)
            {
                throw new Exception("Sign-In Not Available For Event " + eventDto.EventId);
            }

            // Get groups that are configured for the event
            var eventGroupsForEvent = _eventRepository.GetEventGroupsForEvent(participantEventMapDto.CurrentEvent.EventId);
            var eventRooms = eventGroupsForEvent.Select(r => r.RoomReservation).ToList();

            var mpEventParticipantDtoList = (from participant in participantEventMapDto.Participants.Where(r => r.Selected)
                // TODO: Gracefully handle exception for mix of valid and invalid signins
                let eventGroup = participant.GroupId == null ? null : eventGroupsForEvent.Find(eg => eg.GroupId == participant.GroupId)
                select
                    new MpEventParticipantDto
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
                    }).ToList();

            foreach (var eventParticipant in participantEventMapDto.Participants)
            {
                if (eventParticipant.Selected)
                {
                    var mpEventParticipant = mpEventParticipantDtoList.Find(r => r.ParticipantId == eventParticipant.ParticipantId);
                    eventParticipant.AssignedRoomId = null;
                    eventParticipant.AssignedRoomName = null;

                    if (!mpEventParticipant.HasKidsClubGroup)
                    {
                        eventParticipant.SignInErrorMessage = $"Please go to the Kids Club Info Desk and give them this label.  ERROR: {eventParticipant.FirstName} is not in a Kids Club Group (DOB: {eventParticipant.DateOfBirth.ToShortDateString() })";
                    }
                    else if (!mpEventParticipant.HasRoomAssignment)
                    {
                        var group = mpEventParticipant.GroupId.HasValue ? _groupRepository.GetGroup(null, mpEventParticipant.GroupId.Value) : null;
                        eventParticipant.SignInErrorMessage = $"Please go to the Kids Club Info Desk and give them this label.  ERROR: '{group?.Name}' is not assigned to any rooms for {eventDto.EventTitle} for {eventParticipant.FirstName}";
                    }
                    else
                    {
                        var assignedRoomId = mpEventParticipant.RoomId;
                        var assignedRoom = assignedRoomId != null ? eventRooms.First(r => r.RoomId == assignedRoomId.Value) : null;
                        // TODO Temporarily checking if the room is closed - should be handled in bumping rules eventually
                        if (assignedRoom != null && assignedRoom.AllowSignIn)
                        {
                            eventParticipant.AssignedRoomId = assignedRoom.RoomId;
                            eventParticipant.AssignedRoomName = assignedRoom.RoomName;
                        }
                    }
                }
            }

            // populate the room info on the dto
            var response = new ParticipantEventMapDto
            {
                CurrentEvent = participantEventMapDto.CurrentEvent,
                // TODO Only creating event participants for kids with a room assigned - should be handled in bumping rules eventually
                Participants =
                    _childSigninRepository.CreateEventParticipants(
                        mpEventParticipantDtoList.Where(
                            p => participantEventMapDto.Participants.Find(q => q.Selected && q.ParticipantId == p.ParticipantId && q.AssignedRoomId.HasValue) != null).ToList())
                        .Select(Mapper.Map<ParticipantDto>)
                        .ToList(),
                Contacts = participantEventMapDto.Contacts
            };

            // TODO Add back those participants that didn't get a room assigned - should be handled in bumping rules eventually
            response.Participants.AddRange(participantEventMapDto.Participants.Where(p => p.Selected && !p.AssignedRoomId.HasValue));

            response.Participants.ForEach(p => p.Selected = true);

            return response;
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

        public void CreateNewFamily(string token, NewFamilyDto newFamilyDto)
        {
            SaveNewFamilyData(token, newFamilyDto);
            // following stories will work assign to groups and print labels
        }

        private void SaveNewFamilyData(string token, NewFamilyDto newFamilyDto)
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

            List<MpNewParticipantDto> mpNewParticipantDtos = new List<MpNewParticipantDto>();

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

            mpNewParticipantDtos.Add(parentNewParticipantDto);

            // Step 3 create the children contacts
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
                        Company = false
                    }
                };

                mpNewParticipantDtos.Add(childNewParticipantDto);
            }

            _participantRepository.CreateParticipantsWithContacts(token, mpNewParticipantDtos);
        }
    }
}