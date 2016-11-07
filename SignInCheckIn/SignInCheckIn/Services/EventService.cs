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
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly int _defaultEarlyCheckinPeriod;
        private readonly int _defaultLateCheckinPeriod;

        public EventService(IEventRepository eventRepository, IConfigRepository configRepository)
        {
            _eventRepository = eventRepository;

            _defaultEarlyCheckinPeriod = int.Parse(configRepository.GetMpConfigByKey("DefaultEarlyCheckIn").Value);
            _defaultLateCheckinPeriod = int.Parse(configRepository.GetMpConfigByKey("DefaultLateCheckIn").Value);
        }

        public List<EventDto> GetCheckinEvents(DateTime startDate, DateTime endDate, int site)
        {
            return Mapper.Map<List<MpEventDto>, List<EventDto>>(_eventRepository.GetEvents(startDate, endDate, site));
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
    }
}