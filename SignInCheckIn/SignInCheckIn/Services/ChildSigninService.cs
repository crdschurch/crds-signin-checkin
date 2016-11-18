using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Crossroads.Utilities.Services.Interfaces;
using Microsoft.Win32.SafeHandles;
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
        private readonly IContactRepository _contactRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IEventService _eventService;
        private readonly IGroupRepository _groupRepository;
        private readonly IKioskRepository _kioskRepository;
        private readonly IPrintingService _printingService;
        private readonly IPdfEditor _pdfEditor;

        private string _kcLabelPath;
        private string _activityKitLabelPath;

        public ChildSigninService(IChildSigninRepository childSigninRepository, IEventRepository eventRepository, 
            IGroupRepository groupRepository, IEventService eventService, IPdfEditor pdfEditor, IPrintingService printingService,
            IContactRepository contactRepository, IKioskRepository kioskRepository)
        {
            _childSigninRepository = childSigninRepository;
            _contactRepository = contactRepository;
            _eventRepository = eventRepository;
            _groupRepository = groupRepository;
            _eventService = eventService;
            _kioskRepository = kioskRepository;
            _printingService = printingService;
            _pdfEditor = pdfEditor;

            _kcLabelPath = @"C:..\Printing.Utilities\Templates\DefaultLabel.pdf";
            _activityKitLabelPath = @"C:..\Printing.Utilities\Templates\Activity_Kit_Label.pdf";
        }

        public ParticipantEventMapDto GetChildrenAndEventByPhoneNumber(string phoneNumber, int siteId)
        {
            var eventDto = _eventService.GetCurrentEventForSite(siteId);
            var householdId = _childSigninRepository.GetHouseholdIdByPhoneNumber(phoneNumber);

            var mpChildren = _childSigninRepository.GetChildrenByHouseholdId(householdId, Mapper.Map<MpEventDto>(eventDto));
            var childrenDtos = Mapper.Map<List<MpParticipantDto>, List<ParticipantDto>>(mpChildren);

            //var mpHouseholdContactDtos = _contactRepository.GetHeadsOfHouseholdByHouseholdId(householdId);

            ParticipantEventMapDto participantEventMapDto = new ParticipantEventMapDto
            {
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
            var eventRooms = eventGroupsForEvent.Select(r => r.RoomReservation);

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
                        TimeIn = DateTime.Now,
                        OpportunityId = null,
                        RoomId = eventGroup.RoomReservation.RoomId
                    }).ToList();

            // TODO: Get the room ID and room name back on the returned participant dtos
            foreach (var eventParticipant in participantEventMapDto.Participants)
            {
                eventParticipant.AssignedRoomId = mpEventParticipantDtoList.First(r => r.ParticipantId == eventParticipant.ParticipantId).RoomId;

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

            return response;
        }

        public ParticipantEventMapDto PrintParticipants(ParticipantEventMapDto participantEventMapDto, string kioskIdentifier)
        {
            // TODO: Finish this
            var kiofkConfig = _kioskRepository.GetMpKioskConfigByIdentifier(Guid.Parse(kioskIdentifier));

            if (kiofkConfig.PrinterMapId != null)
            {
                var kioskPrinterMap = _kioskRepository.GetPrinterMapById(kiofkConfig.PrinterMapId.GetValueOrDefault());
            }
            else
            {
                throw new Exception("Printer Map Id Not Set For Kisok " + kiofkConfig.KioskConfigId);
            }

            // TODO: Correctly build out the list here - just a temporary field for now
            //var headsOfHouseholdString = participantEventMapDto.Contacts.Select(r => r.Nickname).First();
            var headsOfHouseholdString = "some random parents";

            foreach (var participant in participantEventMapDto.Participants.Where(r => r.Selected == true))
            {
                if (participant.AssignedRoomId == null)
                {
                    // print an "I Got a rock" label
                }
                else
                {
                    Dictionary<string, string> printValues = new Dictionary<string, string>
                    {
                        {"ChildName", participant.FirstName},
                        {"ChildRoomName1", participant.AssignedRoomName},
                        {"ChildRoomName2", participant.AssignedSecondaryRoomName},
                        {"ChildEventName", participantEventMapDto.CurrentEvent.EventTitle},
                        {"ChildParentName", headsOfHouseholdString},
                        {"ChildCallNumber", 1234.ToString()},
                        {"ParentCallNumber", 1234.ToString()},
                        {"ParentRoomName1", participant.AssignedRoomName},
                        {"ParentRoomName2", participant.AssignedSecondaryRoomName},
                        {"Informative1", "This label is worn by a parent/guardian"},
                        {"Informative2", "You must have this label to pick up your child"}
                    };
 
                    var checkinLabel = Properties.Resources.Checkin_KC_Label;
                    var processedPdfBytes = _pdfEditor.PopulatePdfMergeFields(checkinLabel, printValues);

                    // TODO: Update with data from MP for printer id
                    PrintRequestDto printRequestDto = new PrintRequestDto
                    {
                        printerId = 175030,
                        content = processedPdfBytes + "=",
                        contentType = "pdf_base64",
                        title = "Print job for event and participant",
                        source = "CRDS Checkin"
                    };

                    var printJobId = _printingService.SendPrintRequest(printRequestDto);
                }
            }

            return participantEventMapDto;
        }
    }
}
