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

        public ChildSigninService(IChildSigninRepository childSigninRepository,
                                  IEventRepository eventRepository,
                                  IGroupRepository groupRepository,
                                  IEventService eventService,
                                  IPdfEditor pdfEditor,
                                  IPrintingService printingService,
                                  IKioskRepository kioskRepository,
                                  IContactRepository contactRepository)
        {
            _childSigninRepository = childSigninRepository;
            _eventRepository = eventRepository;
            _groupRepository = groupRepository;
            _eventService = eventService;
            _kioskRepository = kioskRepository;
            _contactRepository = contactRepository;
            _printingService = printingService;
            _pdfEditor = pdfEditor;
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

            // TODO: Also need to check capacity on the room here
            var mpEventParticipantDtoList = (from participant in participantEventMapDto.Participants.Where(r => r.Selected)
                // Get groups for the participant
                let groupIds = _groupRepository.GetGroupsForParticipantId(participant.ParticipantId)

                // TODO: Gracefully handle exception for mix of valid and invalid signins
                let eventGroup = eventGroupsForEvent.Find(r => groupIds.Exists(g => r.GroupId == g.Id))
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
                        RoomId = eventGroup.RoomReservation.RoomId
                    }).ToList();

            foreach (var eventParticipant in participantEventMapDto.Participants)
            {
                eventParticipant.AssignedRoomId = mpEventParticipantDtoList.Find(r => r.ParticipantId == eventParticipant.ParticipantId)?.RoomId;

                if (eventParticipant.AssignedRoomId != null)
                {
                    eventParticipant.AssignedRoomName = eventRooms.First(r => r.RoomId == eventParticipant.AssignedRoomId).RoomName;
                }
            }

            // populate the room info on the dto
            var response = new ParticipantEventMapDto
            {
                CurrentEvent = participantEventMapDto.CurrentEvent,
                Participants = _childSigninRepository.CreateEventParticipants(mpEventParticipantDtoList).Select(Mapper.Map<ParticipantDto>).ToList(),
                Contacts = participantEventMapDto.Contacts
            };

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

            var headsOfHousehold = string.Join(", ", participantEventMapDto.Contacts.Select(c => c.Nickname).ToArray());

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
                    {"Informative2", "You must have this label to pick up your child"}
                };
                var labelTemplate = participant.AssignedRoomId == null ? Properties.Resources.Activity_Kit_Label : Properties.Resources.Checkin_KC_Label;
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
    }
}