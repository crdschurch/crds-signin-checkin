using System;
using System.Collections.Generic;
using SignInCheckIn.Models.DTO;

namespace SignInCheckIn.Services.Interfaces
{
    public interface IEventService
    {
        List<EventDto> GetCheckinEvents(DateTime startDate, DateTime endDate, int site);

        EventDto GetEvent(int eventId);
        EventDto GetCurrentEventForSite(int siteId);
        bool CheckEventTimeValidity(EventDto mpEventDto);
        List<EventRoomDto> ImportEventSetup(string authenticationToken, int destinationEventId, int sourceEventId);
        List<EventRoomDto> ResetEventSetup(string authenticationToken, int eventId);
    }
}
