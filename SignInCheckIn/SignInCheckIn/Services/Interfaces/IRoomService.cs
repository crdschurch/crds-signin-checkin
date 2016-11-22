﻿using System.Collections.Generic;
using SignInCheckIn.Models.DTO;

namespace SignInCheckIn.Services.Interfaces
{
    public interface IRoomService
    {
        List<EventRoomDto> GetLocationRoomsByEventId(int eventId);
        EventRoomDto CreateOrUpdateEventRoom(string authenticationToken, EventRoomDto eventRoom);
        EventRoomDto GetEventRoomAgesAndGrades(string authenticationToken, int eventId, int roomId);
        EventRoomDto UpdateEventRoomAgesAndGrades(string authenticationToken, int eventId, int roomId, EventRoomDto eventRoom);
        List<EventRoomDto> GetAvailableRooms(int roomId, int eventId);
        List<EventRoomDto> UpdateAvailableRooms(string authenticationToken, int roomId, int locationId, List<EventRoomDto> eventRoomDtos);
        EventRoomDto CreateEventRoom(string token, EventRoomDto eventRoomDto);
    }
}
