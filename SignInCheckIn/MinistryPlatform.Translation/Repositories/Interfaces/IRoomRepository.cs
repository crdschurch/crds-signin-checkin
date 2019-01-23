using MinistryPlatform.Translation.Models.DTO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace MinistryPlatform.Translation.Repositories.Interfaces
{
    public interface IRoomRepository
    {
        List<MpEventRoomDto> GetRoomsForEvent(int eventId, int locationId);
        List<MpEventRoomDto> GetRoomsForEvent(List<int> eventId, int locationId);

        MpEventRoomDto CreateOrUpdateEventRoom(MpEventRoomDto eventRoom);

        MpEventRoomDto GetEventRoom(int eventId, int? roomId = null);

        MpRoomDto GetRoom(int roomId);

        List<MpBumpingRuleDto> GetBumpingRulesByRoomId(int fromRoomId);

        List<MpRoomDto> GetAvailableRoomsBySite(int locationId);

        void DeleteBumpingRules(IEnumerable<int> ruleIds);

        void DeleteEventRoom(string authenticationToken, int eventRoomId);

        void CreateBumpingRules(List<MpBumpingRuleDto> bumpingRules);

        List<MpBumpingRuleDto> GetBumpingRulesForEventRooms(List<int?> eventRoomIds, int? fromEventRoomId);

        MpEventRoomDto GetEventRoomForEventMaps(List<int> eventIds, int roomId);

        List<MpBumpingRoomsDto> GetBumpingRoomsForEventRoom(int eventId, int fromEventRoomId);

        List<List<JObject>> GetManageRoomsListData(int eventId);

        List<List<JObject>> GetSingleRoomGroupsData(int eventId, int roomId);

        List<List<JObject>> SaveSingleRoomGroupsData(int eventId, int roomId, string groupsXml);

        List<MpEventRoomDto> GetEventRoomsByEventRoomIds(List<int> eventRoomsIds);
    }
}
