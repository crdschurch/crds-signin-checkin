using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Crossroads.Utilities.Services.Interfaces;
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
        private readonly IRoomRepository _roomRepository;
        private readonly IApplicationConfiguration _applicationConfiguration;
        private readonly IEventService _eventService;

        public ChildCheckinService(IChildCheckinRepository childCheckinRepository, IContactRepository contactRepository, IRoomRepository roomRepository, IApplicationConfiguration applicationConfiguration, IEventService eventService)
        {
            _childCheckinRepository = childCheckinRepository;
            _contactRepository = contactRepository;
            _roomRepository = roomRepository;
            _applicationConfiguration = applicationConfiguration;
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

        public Boolean OverrideChildIntoRoom(int eventId, int eventParticipantId, int roomId)
        {
            MpEventRoomDto eventRoom = _roomRepository.GetEventRoom(eventId, roomId);
            Boolean isClosed = !eventRoom.AllowSignIn;
            var isAtCapacity = eventRoom.Capacity <= (eventRoom.CheckedIn + eventRoom.SignedIn);
            if (isClosed)
            {
                throw new Exception("closed");
            }
            else if (isAtCapacity)
            {
                throw new Exception("capacity");
            }
            else
            {
                _childCheckinRepository.OverrideChildIntoRoom(eventParticipantId, roomId);
                return true;
            }
            return false;
        }

        public ParticipantDto GetEventParticipantByCallNumber(int eventId, int callNumber, int roomId)
        {
            var mpEventParticipant = _childCheckinRepository.GetEventParticipantByCallNumber(eventId, callNumber);
            if (mpEventParticipant == null) return null;
            // if child is in room and checked in, dont show
            var checkedInParticipationStatusId = _applicationConfiguration.CheckedInParticipationStatusId;
            if (mpEventParticipant.RoomId == roomId && mpEventParticipant.ParticipantStatusId == checkedInParticipationStatusId) return null;

            mpEventParticipant.HeadsOfHousehold = _contactRepository.GetHeadsOfHouseholdByHouseholdId(mpEventParticipant.CheckinHouseholdId.Value);
            var participant = Mapper.Map<MpEventParticipantDto, ParticipantDto>(mpEventParticipant);
            return participant;
        }
    }
}