using MinistryPlatform.Translation.Models.DTO;
using System;
using System.Collections.Generic;

namespace MinistryPlatform.Translation.Repositories.Interfaces
{
    public interface IEventRepository
    {
        List<MpEventDto> GetEventTemplates(int site);
        List<MpEventDto> GetEvents(DateTime startDate, DateTime endDate, int site, bool? includeSubevents = false, List<int> eventTypeIds = null, bool excludeIds = true);
        MpEventDto GetEventById(int eventId);
        MpEventDto CreateSubEvent(MpEventDto mpEventDto);
        MpEventDto UpdateEvent(MpEventDto mpEventDto);
        List<MpEventGroupDto> GetEventGroupsForEvent(int eventId);
        List<MpEventGroupDto> GetEventGroupsForEventByGroupTypeId(int eventId, int groupTypeId);
        List<MpEventGroupDto> GetEventGroupsForEvent(List<int> eventIds);
        List<MpEventGroupDto> GetEventGroupsForEventRoom(int eventId, int roomId);
        void DeleteEventGroups(string authenticationToken, IEnumerable<int> eventGroupIds);
        List<MpEventGroupDto> CreateEventGroups(string authenticationToken, List<MpEventGroupDto> eventGroups);
        void ResetEventSetup(int eventId);
        void ImportEventSetup(int destinationEventId, int sourceEventId);
        MpEventDto GetSubeventByParentEventId(int eventId, int eventTypeId);
        List<MpEventDto> GetEventAndCheckinSubevents(int eventId, bool includeTemplates = false);
        List<MpEventDto> GetSubeventsForEvents(List<int> eventIds, int? eventTypeId);
        List<MpEventGroupDto> GetEventGroupsByGroupIdAndEventIds(int groupId, List<int> eventIds);
        List<MpCapacityDto> GetCapacitiesForEvent(int eventId);
    }
}
