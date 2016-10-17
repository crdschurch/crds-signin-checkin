using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Models.Json;
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

        public List<EventDto> GetCheckinEvents()
        {
            var mpEvents = _eventRepository.GetEvents();

            //foreach (var eventItem in mpEvents)
            //{
            //    returnEvents.Add(new EventDto
            //    {
            //        EventId = eventItem.EventId,
            //        EventTitle = eventItem.EventTitle,
            //        EventStartDate = eventItem.EventStartDate,
            //        EventType = eventItem.EventType,
            //        EventSite = eventItem.CongregationName
            //    });
            //}
            return Mapper.Map<List<MpEventDto>, List<EventDto>>(mpEvents);
        }
    }
}