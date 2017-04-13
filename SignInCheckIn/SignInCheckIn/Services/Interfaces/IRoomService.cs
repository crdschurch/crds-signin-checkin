using System;
using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;
using SignInCheckIn.Models.DTO;

namespace SignInCheckIn.Services.Interfaces
{
    public interface IRoomService
    {
        List<EventRoomDto> GetLocationRoomsByEventId(string authenticationToken, int eventId);
        EventRoomDto CreateOrUpdateEventRoom(string authenticationToken, EventRoomDto eventRoom);
        EventRoomDto GetEventRoomAgesAndGrades(string authenticationToken, int eventId, int roomId);
        EventRoomDto UpdateEventRoomAgesAndGrades(string authenticationToken, int eventId, int roomId, EventRoomDto eventRoom);
        List<EventRoomDto> GetAvailableRooms(string token, int roomId, int eventId);
        List<EventRoomDto> UpdateAvailableRooms(string authenticationToken, int roomId, int locationId, List<EventRoomDto> eventRoomDtos);
        List<AgeGradeDto> GetGradeAttributes(string authenticationToken, int siteId, string kioskId, int? eventId = null);
        List<MpGroupDto> GetEventUnassignedGroups(string authenticationToken, int eventId);
        EventRoomDto CreateOrUpdateAdventureClubRoom(string authenticationToken, EventRoomDto eventRoom);
        EventRoomDto GetEventRoom(int eventId, int roomId);
        EventRoomDto GetEventRoom(int eventId, int roomId, bool canCreateEventRoom);
    }
}
