using System;
using System.Collections.Generic;
using SignInCheckIn.Models.DTO;

namespace SignInCheckIn.Services.Interfaces
{
    public interface IEventService
    {
        List<EventDto> GetCheckinEvents(DateTime startDate, DateTime endDate, int site);

        EventDto GetEvent(int eventId);
    }
}
