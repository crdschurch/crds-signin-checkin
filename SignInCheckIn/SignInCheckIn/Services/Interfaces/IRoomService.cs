using System.Collections.Generic;
using SignInCheckIn.Models.DTO;

namespace SignInCheckIn.Services.Interfaces
{
    public interface IRoomService
    {
        List<EventRoomDto> GetLocationRoomsByEventId(int eventId);
        EventRoomDto CreateOrUpdateEventRoom(string authenticationToken, EventRoomDto eventRoom);
        List<AgeGradeDto> GetEventRoomAgesAndGrades(string authenticationToken, int eventId, int roomId);
    }
}
