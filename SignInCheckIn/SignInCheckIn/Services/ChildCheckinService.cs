using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Services.Interfaces;

namespace SignInCheckIn.Services
{
    public class ChildCheckinService : IChildCheckinService
    {
        private readonly IChildCheckinRepository _childCheckinRepository;
        private readonly IEventService _eventService;

        public ChildCheckinService(IChildCheckinRepository childCheckinRepository, IEventService eventService)
        {
            _childCheckinRepository = childCheckinRepository;
            _eventService = eventService;
        }

        public ParticipantEventMapDto GetChildrenForCurrentEventAndRoom(int roomId, int siteId, int? eventId)
        {
            var eventDto = (eventId == null) ? _eventService.GetCurrentEventForSite(siteId) : _eventService.GetEvent((int) eventId);
            var mpChildren = _childCheckinRepository.GetChildrenByEventAndRoom(eventDto.EventId, roomId);
            var childrenDtos = Mapper.Map<List<MpParticipantDto>, List<ParticipantDto>>(mpChildren);

            ParticipantEventMapDto participantEventMapDto = new ParticipantEventMapDto
            {
                Participants = childrenDtos,
                CurrentEvent = eventDto
            };

            return participantEventMapDto;
        }

        public void CheckinChildrenForCurrentEventAndRoom(bool checkIn, int eventParticipantId)
        {
            _childCheckinRepository.CheckinChildrenForCurrentEventAndRoom(checkIn, eventParticipantId);
        }
    }
}