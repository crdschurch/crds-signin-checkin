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
        private readonly IKioskRepository _kioskRepository;
        private readonly int _defaultEarlyCheckinPeriod;
        private readonly int _defaultLateCheckinPeriod;

        public EventService(IEventRepository eventRepository, IConfigRepository configRepository, IRoomRepository roomRepository,
            IApplicationConfiguration applicationConfiguration, IParticipantRepository participantRepository, IKioskRepository kioskRepository)
        {
            _eventRepository = eventRepository;
            _roomRepository = roomRepository;
            _applicationConfiguration = applicationConfiguration;
            _participantRepository = participantRepository;
            _kioskRepository = kioskRepository;

            _defaultEarlyCheckinPeriod = int.Parse(configRepository.GetMpConfigByKey("DefaultEarlyCheckIn").Value);
            _defaultLateCheckinPeriod = int.Parse(configRepository.GetMpConfigByKey("DefaultLateCheckIn").Value);
        }

        public List<EventDto> GetCheckinEventTemplates(int site)
        {
            return Mapper.Map<List<MpEventDto>, List<EventDto>>(_eventRepository.GetEventTemplates(site));
        }

        public List<EventDto> GetCheckinEvents(DateTime startDate, DateTime endDate, int site, string kioskId)
        {
            // filter events we don't want to show on the checkin kiosk
            var kioskConfig = _kioskRepository.GetMpKioskConfigByIdentifier(Guid.Parse(kioskId));

            List<int> excludeEventTypeIds = new List<int>();

            if (kioskConfig.KioskTypeId == _applicationConfiguration.CheckinKioskTypeId)
            {
                excludeEventTypeIds.Add(_applicationConfiguration.StudentMinistryGradesSixToEightEventTypeId);
                excludeEventTypeIds.Add(_applicationConfiguration.StudentMinistryGradesNineToTwelveEventTypeId);
                excludeEventTypeIds.Add(_applicationConfiguration.BigEventTypeId);
            }

            var events = Mapper.Map<List<MpEventDto>, List<EventDto>>(_eventRepository.GetEvents(startDate, endDate, site));

            foreach (var eventDto in events)
            {
                eventDto.IsCurrentEvent = CheckEventTimeValidity(eventDto);
            }
            return events.Where(r => !excludeEventTypeIds.Contains(r.EventTypeId)).ToList();
        }

        public EventDto GetEvent(int eventId)
        {
            return Mapper.Map<EventDto>(_eventRepository.GetEventById(eventId));
        }

        public EventDto GetCurrentEventForSite(int siteId, string kioskId = "")
        {
            // load the kiosk id here...
            var kioskConfig = _kioskRepository.GetMpKioskConfigByIdentifier(Guid.Parse(kioskId));

            List<int> msmEventTypeIds = new List<int>
            {
                _applicationConfiguration.StudentMinistryGradesSixToEightEventTypeId,
                _applicationConfiguration.StudentMinistryGradesNineToTwelveEventTypeId,
                _applicationConfiguration.BigEventTypeId
            };

            // if it's not an SM event, we want to filter these events out and return only KC/Childcare events
            var excludeIds = kioskConfig.KioskTypeId != _applicationConfiguration.StudentMinistryKioskTypeId;

            // look between midnights on the current day
            var eventOffsetStartString = DateTime.Now.ToShortDateString();
            var eventOffsetStartTime = DateTime.Parse(eventOffsetStartString);
            var eventOffsetEndTime = DateTime.Parse(eventOffsetStartString).AddDays(1);

            var currentEvents =
                    _eventRepository.GetEvents(eventOffsetStartTime, eventOffsetEndTime, siteId, false, msmEventTypeIds, excludeIds).Where(r => CheckEventTimeValidity(Mapper.Map<MpEventDto, EventDto>(r))).ToList();

            if (!currentEvents.Any())
            {
                throw new Exception("No current events for site");
            }

            return Mapper.Map<MpEventDto, EventDto>(currentEvents.First());
        }

        public void UpdateAdventureClubStatusIfNecessary(MpEventDto eventDto, string token)
        {
            // we need to figure out if this event is the adventure club event or the service event
            // if it is not the AC event, we need to get the AC event
            if (eventDto.EventTypeId != _applicationConfiguration.AdventureClubEventTypeId)
            {
                eventDto = _eventRepository.GetSubeventByParentEventId(token, eventDto.EventId, _applicationConfiguration.AdventureClubEventTypeId);
            }

            // search to see if there are existing Event Rooms for the AC subevent
            var eventRoom = _roomRepository.GetEventRoom(eventDto.EventId);

            if (eventRoom != null)
            {
                // if there are, set cancelled to false
                eventDto.Cancelled = false;
                _eventRepository.UpdateEvent(token, eventDto);
            }
            else
            {
                // if that are not, set cancelled to true
                // if there are, set cancelled to false
                eventDto.Cancelled = true;
                _eventRepository.UpdateEvent(token, eventDto);
            }
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

            // import AC event if source has one
            var sourceAcSubevent = _eventRepository.GetSubeventByParentEventId(sourceEventId, _applicationConfiguration.AdventureClubEventTypeId);
            if (sourceAcSubevent != null)
            {
                var destinationEvent = _eventRepository.GetEventById(destinationEventId);
                var destinationAcSubevent = _eventRepository.GetSubeventByParentEventId(destinationEventId, _applicationConfiguration.AdventureClubEventTypeId);
                // create a new AC subevent under the destination event if one doesnt exist
                if (destinationAcSubevent == null)
                {
                    destinationAcSubevent = CreateAdventureClubSubevent(destinationEvent, authenticationToken);
                }
                else
                {
                    // if we aren't creating a new AC subevent, reset the existing one
                    _eventRepository.ResetEventSetup(authenticationToken, destinationAcSubevent.EventId);
                }
                _eventRepository.ImportEventSetup(authenticationToken, destinationAcSubevent.EventId, sourceAcSubevent.EventId);
                // set correct adventure club flag
                UpdateAdventureClubStatusIfNecessary(destinationAcSubevent, authenticationToken);
            }

            return Mapper.Map<List<EventRoomDto>>(_roomRepository.GetRoomsForEvent(destinationEventId, targetEvent.LocationId));
        }

        public MpEventDto CreateAdventureClubSubevent(MpEventDto parentEvent, string token)
        {
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
            return _eventRepository.CreateSubEvent(token, mpEventDto);
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
            var events = _eventRepository.GetEventAndCheckinSubevents(token, eventId, true);
            var parentEvent = events.First(r => r.ParentEventId == null);

            // 1. See if there's an existing AC subevent
            if (!events.Any(r => r.ParentEventId == eventId && r.EventTypeId == _applicationConfiguration.AdventureClubEventTypeId))
            {
                // 2. If not, create it
                var newAcEvent = CreateAdventureClubSubevent(parentEvent, token);
                events.Add(newAcEvent);
            }

            return Mapper.Map<List<MpEventDto>, List<EventDto>>(events);
        }

        public List<ParticipantDto> GetListOfChildrenForEvent(string token, int eventId, string search)
        {
            var result = _participantRepository.GetChildParticipantsByEvent(token, eventId, search);
            var children = new List<ParticipantDto>();

            foreach (var tmpChild in result)
            {
                var child = Mapper.Map<MpEventParticipantDto, ParticipantDto>(tmpChild);
                child.HeadsOfHousehold = tmpChild.HeadsOfHousehold.Select(Mapper.Map<MpContactDto, ContactDto>).ToList();
                children.Add(child);
            }

            children = children.OrderByDescending(r => r.AssignedRoomName).ThenBy(r => r.Nickname).ToList();

            return children;
        }

        public List<ContactDto> GetFamiliesForSearch(string token, string search)
        {
            var result = _participantRepository.GetFamiliesForSearch(token, search);
            return result.Select(Mapper.Map<MpContactDto, ContactDto>).ToList();
        }

        public HouseholdDto GetHouseholdByHouseholdId(string token, int householdId)
        {
            var result = _participantRepository.GetHouseholdByHouseholdId(token, householdId);
            return Mapper.Map<MpHouseholdDto, HouseholdDto>(result);
        }

        public HouseholdDto UpdateHouseholdInformation(string token, HouseholdDto householdDto)
        {
            var mpHouseholdDto = Mapper.Map<HouseholdDto, MpHouseholdDto>(householdDto);
            _participantRepository.UpdateHouseholdInformation(token, mpHouseholdDto);
            return Mapper.Map<MpHouseholdDto, HouseholdDto>(mpHouseholdDto);
        }

        // List<CapacityDto> GetCapacityBySite(int siteId);
        public List<CapacityDto> GetCapacityBySite(int siteId)
        {
            var eventId = GetCurrentEventForSiteKcOnly(siteId).EventId;
            var result = _eventRepository.GetCapacitiesForEvent(eventId);
            return Mapper.Map<List<MpCapacityDto>, List<CapacityDto>>(result);
        }

        // this function only supports the capacity app
        public EventDto GetCurrentEventForSiteKcOnly(int siteId)
        {
            List<int> msmEventTypeIds = new List<int>
            {
                _applicationConfiguration.StudentMinistryGradesSixToEightEventTypeId,
                _applicationConfiguration.StudentMinistryGradesNineToTwelveEventTypeId,
                _applicationConfiguration.BigEventTypeId
            };

            // if it's not an SM event, we want to filter these events out and return only KC/Childcare events
            var excludeIds = true;

            // look between midnights on the current day
            var eventOffsetStartString = DateTime.Now.ToShortDateString();
            var eventOffsetStartTime = DateTime.Parse(eventOffsetStartString);
            var eventOffsetEndTime = DateTime.Parse(eventOffsetStartString).AddDays(1);

            var currentEvents =
                    _eventRepository.GetEvents(eventOffsetStartTime, eventOffsetEndTime, siteId, false, msmEventTypeIds, excludeIds).Where(r => CheckEventTimeValidity(Mapper.Map<MpEventDto, EventDto>(r))).ToList();

            if (!currentEvents.Any())
            {
                throw new Exception("No current events for site");
            }

            return Mapper.Map<MpEventDto, EventDto>(currentEvents.First());
        }
    }
}