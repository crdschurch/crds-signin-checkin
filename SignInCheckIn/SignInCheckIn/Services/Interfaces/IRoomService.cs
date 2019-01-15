using MinistryPlatform.Translation.Models.DTO;
using SignInCheckIn.Models.DTO;
using System.Collections.Generic;

namespace SignInCheckIn.Services.Interfaces
{
    public interface IRoomService
    {
        List<EventRoomDto> GetLocationRoomsByEventId(int eventId);
        EventRoomDto CreateOrUpdateEventRoom(EventRoomDto eventRoom);
        EventRoomDto GetEventRoomAgesAndGrades(int eventId, int roomId);
        EventRoomDto UpdateEventRoomAgesAndGrades(int eventId, int roomId, EventRoomDto eventRoom);
        List<EventRoomDto> GetAvailableRooms(int roomId, int eventId);
        List<EventRoomDto> UpdateAvailableRooms(int roomId, int locationId, List<EventRoomDto> eventRoomDtos);
        List<AgeGradeDto> GetGradeAttributes(string authenticationToken, int siteId, string kioskId, int? eventId = null);
        List<MpGroupDto> GetEventUnassignedGroups(int eventId);
        EventRoomDto CreateOrUpdateAdventureClubRoom(string authenticationToken, EventRoomDto eventRoom);
        EventRoomDto GetEventRoom(int eventId, int roomId);
        EventRoomDto GetEventRoom(int eventId, int roomId, bool canCreateEventRoom);
    }
}
