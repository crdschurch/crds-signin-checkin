using System;
using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;

namespace MinistryPlatform.Translation.Repositories.Interfaces
{
    public interface IEventRepository
    {
        List<MpEventDto> GetEvents(DateTime startDate, DateTime endDate, int site, bool? includeSubevents = false, List<int> eventTypeIds = null, bool excludeIds = true);
        MpEventDto GetEventById(int eventId);
        MpEventDto CreateSubEvent(string token, MpEventDto mpEventDto);
        MpEventDto UpdateEvent(string token, MpEventDto mpEventDto);
        List<MpEventGroupDto> GetEventGroupsForEvent(int eventId);
        List<MpEventGroupDto> GetEventGroupsForEventByGroupTypeId(int eventId, int groupTypeId);
        List<MpEventGroupDto> GetEventGroupsForEvent(List<int> eventIds);
        List<MpEventGroupDto> GetEventGroupsForEventRoom(int eventId, int roomId);
        void DeleteEventGroups(string authenticationToken, IEnumerable<int> eventGroupIds);
        List<MpEventGroupDto> CreateEventGroups(string authenticationToken, List<MpEventGroupDto> eventGroups);
        void ResetEventSetup(string authenticationToken, int eventId);
        void ImportEventSetup(string authenticationToken, int destinationEventId, int sourceEventId);
        MpEventDto GetSubeventByParentEventId(int eventId, int eventTypeId);
        List<MpEventDto> GetEventAndCheckinSubevents(string token, int eventId);
        List<MpEventDto> GetSubeventsForEvents(List<int> eventIds, int? eventTypeId);
        MpEventDto GetSubeventByParentEventId(string token, int serviceEventId, int eventTypeId);
        List<MpEventGroupDto> GetEventGroupsByGroupIdAndEventIds(int groupId, List<int> eventIds);
    }
}
