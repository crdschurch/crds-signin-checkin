using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(7);
            List<int> serviceTypeIds = new List<int>();
            serviceTypeIds.Add(1);

            var mpEvents = _eventRepository.GetEvents(startDate, endDate, serviceTypeIds);
            List<EventDto> returnEvents = new List<EventDto>();

            foreach (var eventItem in mpEvents)
            {
                returnEvents.Add(new EventDto
                {
                    EventId = eventItem.EventId,
                    EventTitle = eventItem.EventTitle,
                    EventStartDate = eventItem.EventStartDate,
                    EventType = eventItem.EventType,
                    EventSite = eventItem.CongregationName
                });
            }
            
            return returnEvents;
        }
    }
}