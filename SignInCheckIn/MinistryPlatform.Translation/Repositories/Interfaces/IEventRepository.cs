using System;
using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;

namespace MinistryPlatform.Translation.Repositories.Interfaces
{
    public interface IEventRepository
    {
        List<MpEventDto> GetEvents(DateTime startDate, DateTime endDate, int site);
        MpEventDto GetEventById(int eventId);

        List<MpEventGroupDto> GetEventGroupsForEvent(int eventId);
    }
}
