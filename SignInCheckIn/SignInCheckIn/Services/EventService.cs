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
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IApplicationConfiguration _applicationConfiguration;
        private readonly IParticipantRepository _participantRepository;
        private readonly int _defaultEarlyCheckinPeriod;
        private readonly int _defaultLateCheckinPeriod;

        public EventService(IEventRepository eventRepository, IConfigRepository configRepository, IRoomRepository roomRepository,
            IApplicationConfiguration applicationConfiguration, IParticipantRepository participantRepository)
        {
            _eventRepository = eventRepository;
            _roomRepository = roomRepository;
            _applicationConfiguration = applicationConfiguration;
            _participantRepository = participantRepository;

            _defaultEarlyCheckinPeriod = int.Parse(configRepository.GetMpConfigByKey("DefaultEarlyCheckIn").Value);
            _defaultLateCheckinPeriod = int.Parse(configRepository.GetMpConfigByKey("DefaultLateCheckIn").Value);
        }

        public List<EventDto> GetCheckinEvents(DateTime startDate, DateTime endDate, int site)
        {
            var events = Mapper.Map<List<MpEventDto>, List<EventDto>>(_eventRepository.GetEvents(startDate, endDate, site));
            foreach (var eventDto in events)
            {
                eventDto.IsCurrentEvent = CheckEventTimeValidity(eventDto);
            }
            return events;
        }

        public EventDto GetEvent(int eventId)
        {
            return Mapper.Map<EventDto>(_eventRepository.GetEventById(eventId));
        }

        public EventDto GetCurrentEventForSite(int siteId)
        {
            // look between midnights on the current day
            var eventOffsetStartString = DateTime.Now.ToShortDateString();
            var eventOffsetStartTime = DateTime.Parse(eventOffsetStartString);
            var eventOffsetEndTime = DateTime.Parse(eventOffsetStartString).AddDays(1);

            var currentEvents = _eventRepository.GetEvents(eventOffsetStartTime, eventOffsetEndTime, siteId).Where(r => CheckEventTimeValidity(Mapper.Map<MpEventDto, EventDto>(r))).ToList();

            if (!currentEvents.Any())
            {
                throw new Exception("No current events for site");
            }

            return Mapper.Map<MpEventDto, EventDto>(currentEvents.First());
        }

        public bool CheckEventTimeValidity(EventDto eventDto)
        {
            // use the event's checkin period if available, otherwise default to the mp config values
            var beginSigninWindow = eventDto.EventStartDate.AddMinutes(-(eventDto.EarlyCheckinPeriod ?? _defaultEarlyCheckinPeriod));
            var endSigninWindow = eventDto.EventStartDate.AddMinutes(eventDto.LateCheckinPeriod ?? _defaultLateCheckinPeriod);

            return DateTime.Now >= beginSigninWindow && DateTime.Now <= endSigninWindow;
        }

        public List<EventRoomDto> ImportEventSetup(string authenticationToken, int destinationEventId, int sourceEventId)
        {
            var targetEvent = _eventRepository.GetEventById(destinationEventId);

            _eventRepository.ResetEventSetup(authenticationToken, destinationEventId);
            _eventRepository.ImportEventSetup(authenticationToken, destinationEventId, sourceEventId);

            return Mapper.Map<List<EventRoomDto>>(_roomRepository.GetRoomsForEvent(destinationEventId, targetEvent.LocationId));
        }

        public List<EventRoomDto> ResetEventSetup(string authenticationToken, int eventId)
        {
            var targetEvent = _eventRepository.GetEventById(eventId);

            _eventRepository.ResetEventSetup(authenticationToken, eventId);
            return Mapper.Map<List<EventRoomDto>>(_roomRepository.GetRoomsForEvent(eventId, targetEvent.LocationId));
        }

        // this is only getting a parent and the ac event - this will need to be changed as part of the
        // upcoming refactor story - US6056
        public List<EventDto> GetEventMaps(string token, int eventId)
        {
            var events = _eventRepository.GetEventAndCheckinSubevents(token, eventId);
            var parentEvent = events.First(r => r.ParentEventId == null);

            // 1. See if there's an existing AC subevent
            if (!events.Any(r => r.ParentEventId == eventId && r.EventTypeId == _applicationConfiguration.AdventureClubEventTypeId))
            {
                // 2. If not, create it
                MpEventDto mpEventDto = new MpEventDto();
                mpEventDto.EventTitle = $"Adventure Club for Event {parentEvent.EventId}";
                mpEventDto.ParentEventId = parentEvent.EventId;
                mpEventDto.EventTypeId = _applicationConfiguration.AdventureClubEventTypeId;
                mpEventDto.CongregationId = parentEvent.CongregationId;
                mpEventDto.LocationId = parentEvent.LocationId;
                mpEventDto.ProgramId = parentEvent.ProgramId;
                mpEventDto.PrimaryContact = parentEvent.PrimaryContact;
                mpEventDto.MinutesForSetup = parentEvent.MinutesForSetup;
                mpEventDto.MinutesForCleanup = parentEvent.MinutesForCleanup;
                mpEventDto.EventStartDate = parentEvent.EventStartDate;
                mpEventDto.EventEndDate = parentEvent.EventEndDate;
                mpEventDto.Cancelled = true;
                mpEventDto.AllowCheckIn = parentEvent.AllowCheckIn;
                var subEvent = _eventRepository.CreateSubEvent(token, mpEventDto);
                events.Add(subEvent);
            }

            return Mapper.Map<List<MpEventDto>, List<EventDto>>(events);
        }
    }
}