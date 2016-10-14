using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinistryPlatform.Translation.Models.DTO;

namespace MinistryPlatform.Translation.Repositories.Interfaces
{
    public interface IRoomRepository
    {
        List<MpRoomDto> GetRoomsForEvent(int eventId, int congregationId);

        MpEventRoomDto GetEventRoom(int eventId, int eventRoomId);

        MpEventRoomDto GetEventRoom(int eventRoomId);

        MpEventRoomDto CreateOrUpdateEventRoom(string authenticationToken, MpEventRoomDto eventRoom);
    }
}
