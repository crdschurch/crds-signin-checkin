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
        List<MpEventGroupDto> GetEventGroupsForEventRoom(int eventId, int roomId);
        void DeleteEventGroups(string authenticationToken, IEnumerable<int> eventGroupIds);
        List<MpEventGroupDto> CreateEventGroups(string authenticationToken, List<MpEventGroupDto> eventGroups);
        void ResetEventSetup(string authenticationToken, int eventId);
        void ImportEventSetup(string authenticationToken, int destinationEventId, int sourceEventId);
    }
}
