using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;

namespace MinistryPlatform.Translation.Repositories.Interfaces
{
    public interface IRoomRepository
    {
        List<MpEventRoomDto> GetRoomsForEvent(int eventId, int locationId);

        MpEventRoomDto CreateOrUpdateEventRoom(string authenticationToken, MpEventRoomDto eventRoom);

        MpEventRoomDto GetEventRoom(int eventId, int roomId);
    }
}
