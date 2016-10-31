using System;
using System.Collections.Generic;
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

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public List<EventDto> GetCheckinEvents(DateTime startDate, DateTime endDate, int site)
        {
            return Mapper.Map<List<MpEventDto>, List<EventDto>>(_eventRepository.GetEvents(startDate, endDate, site));
        }

        public EventDto GetEvent(int eventId)
        {
            return Mapper.Map<EventDto>(_eventRepository.GetEventById(eventId));
        }
    }
}