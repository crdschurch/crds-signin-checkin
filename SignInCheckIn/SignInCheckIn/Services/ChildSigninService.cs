using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Crossroads.Utilities.Services.Interfaces;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
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
        private readonly IPrintingService _printingService;
        private readonly IPdfEditor _pdfEditor;

        public ChildSigninService(IChildSigninRepository childSigninRepository, IEventRepository eventRepository, 
            IGroupRepository groupRepository, IEventService eventService, IPrintingService printingService,
            IPdfEditor pdfEditor)
        {
            _childSigninRepository = childSigninRepository;
            _eventRepository = eventRepository;
            _groupRepository = groupRepository;
            _eventService = eventService;
            _printingService = printingService;
            _pdfEditor = pdfEditor;
        }

        public ParticipantEventMapDto GetChildrenAndEventByPhoneNumber(string phoneNumber, int siteId)
        {
            var eventDto = _eventService.GetCurrentEventForSite(siteId);
            var mpChildren = _childSigninRepository.GetChildrenByPhoneNumber(phoneNumber);
            var childrenDtos = Mapper.Map<List<MpParticipantDto>, List<ParticipantDto>>(mpChildren);

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


            var response = new ParticipantEventMapDto
            {
                CurrentEvent = participantEventMapDto.CurrentEvent,
                Participants = _childSigninRepository.CreateEventParticipants(mpEventParticipantDtoList).Select(Mapper.Map<ParticipantDto>).ToList()
            };

            response.Participants.ForEach(p => p.Selected = true);

            return response;
        }
    }
}