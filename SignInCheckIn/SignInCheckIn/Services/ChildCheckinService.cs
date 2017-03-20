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
        private readonly IEventRepository _eventRepository;

        public ChildCheckinService(IChildCheckinRepository childCheckinRepository, IContactRepository contactRepository, IRoomRepository roomRepository, IApplicationConfiguration applicationConfiguration, IEventService eventService,
            IEventRepository eventRepository)
        {
            _childCheckinRepository = childCheckinRepository;
            _contactRepository = contactRepository;
            _roomRepository = roomRepository;
            _applicationConfiguration = applicationConfiguration;
            _eventService = eventService;
            _eventRepository = eventRepository;
        }

        // TODO: The call to _eventService needs to be refactored at some future point to directly access the repo layer
        public ParticipantEventMapDto GetChildrenForCurrentEventAndRoom(int roomId, int siteId, int? eventId)
        {
            var eventDto = (eventId == null) ? _eventService.GetCurrentEventForSite(siteId) : _eventService.GetEvent((int) eventId);
            var eventAndSubeventIds = new List<int> { eventDto.EventId };
            var subEvents = _eventRepository.GetSubeventsForEvents(new List<int> {eventDto.EventId}, null).ToList();
            eventAndSubeventIds.AddRange(subEvents.Select(r => r.EventId).ToList());

            var mpCurrentEventRoom = _roomRepository.GetEventRoomForEventMaps(eventAndSubeventIds, roomId);
            if (mpCurrentEventRoom == null)
            {
                return new ParticipantEventMapDto
                {
                    Participants = null,
                    CurrentEvent = eventDto
                };
            }
        
            var childrenDtos = GetChildrenForCurrentEventAndRoom(mpCurrentEventRoom.EventId, mpCurrentEventRoom.RoomId);

            var participantEventMapDto = new ParticipantEventMapDto
            {
                Participants = childrenDtos,
                CurrentEvent = eventDto
            };

            return participantEventMapDto;
        }

        public List<ParticipantDto> GetChildrenForCurrentEventAndRoom(int eventId, int roomId)
        {
            var mpChildren = _childCheckinRepository.GetChildrenByEventAndRoom(eventId, roomId);
            return Mapper.Map<List<MpParticipantDto>, List<ParticipantDto>>(mpChildren);
        }

        public ParticipantDto CheckinChildrenForCurrentEventAndRoom(ParticipantDto eventParticipant)
        {
            _childCheckinRepository.CheckinChildrenForCurrentEventAndRoom(eventParticipant.ParticipationStatusId, eventParticipant.EventParticipantId);
            return eventParticipant;
        }

        public bool OverrideChildIntoRoom(int eventId, int eventParticipantId, int roomId)
        {
            var eventRoom = _roomRepository.GetEventRoom(eventId, roomId);
            if (eventRoom == null)
            {
                var acSubevent = _eventRepository.GetSubeventByParentEventId(eventId, _applicationConfiguration.AdventureClubEventTypeId);
                eventRoom = _roomRepository.GetEventRoom(acSubevent.EventId, roomId);
            }
            if (eventRoom == null)
            {
                throw new Exception($"no event room for room {roomId}");
            }
            bool isClosed = !eventRoom.AllowSignIn;
            bool isAtCapacity = eventRoom.Capacity <= (eventRoom.CheckedIn + eventRoom.SignedIn);
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
                _childCheckinRepository.OverrideChildIntoRoom(eventParticipantId, roomId, eventRoom.EventId);
                return true;
            }
        }

        public ParticipantDto GetEventParticipantByCallNumber(int eventId, int callNumber, int roomId, bool? excludeThisRoom = false)
        {
            var kcSubevent = _eventRepository.GetSubeventByParentEventId(eventId, _applicationConfiguration.AdventureClubEventTypeId);
            var mpEventParticipant = _childCheckinRepository.GetEventParticipantByCallNumber(new List<int> { eventId, kcSubevent.EventId }, callNumber);
            if (excludeThisRoom == true) { 
                // if child is in room and checked in, dont show
                var checkedInParticipationStatusId = _applicationConfiguration.CheckedInParticipationStatusId;
                if (mpEventParticipant.RoomId == roomId && mpEventParticipant.ParticipantStatusId == checkedInParticipationStatusId)
                {
                    throw new Exception("Child is checkin into this room");
                };
            }
            mpEventParticipant.HeadsOfHousehold = _contactRepository.GetHeadsOfHouseholdByHouseholdId(mpEventParticipant.CheckinHouseholdId.Value);
            var participant = Mapper.Map<MpEventParticipantDto, ParticipantDto>(mpEventParticipant);
            return participant;
        }
    }
}