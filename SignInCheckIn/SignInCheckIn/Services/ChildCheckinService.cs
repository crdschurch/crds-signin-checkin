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
        private readonly IContactRepository _contactRepository;
        private readonly IEventService _eventService;

        public ChildCheckinService(IChildCheckinRepository childCheckinRepository, IContactRepository contactRepository, IEventService eventService)
        {
            _childCheckinRepository = childCheckinRepository;
            _contactRepository = contactRepository;
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

        public ParticipantDto CheckinChildrenForCurrentEventAndRoom(ParticipantDto eventParticipant)
        {
            _childCheckinRepository.CheckinChildrenForCurrentEventAndRoom(eventParticipant.ParticipationStatusId, eventParticipant.EventParticipantId);
            return eventParticipant;
        }

        public ParticipantDto GetEventParticipantByCallNumber(int eventId, int callNumber)
        {
            var mpEventParticipant = _childCheckinRepository.GetEventParticipantByCallNumber(eventId, callNumber);
            mpEventParticipant.HeadsOfHousehold = _contactRepository.GetHeadsOfHouseholdByHouseholdId(mpEventParticipant.CheckinHouseholdId.Value);
            var participant = Mapper.Map<MpEventParticipantDto, ParticipantDto>(mpEventParticipant);
            return participant;
        }
    }
}