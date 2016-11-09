using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;

namespace MinistryPlatform.Translation.Repositories.Interfaces
{
    public interface IChildCheckinRepository
    {
        List<MpParticipantDto> GetChildrenByEventAndRoom(int eventId, int roomId);
        void CheckinChildrenForCurrentEventAndRoom(int checkinStatusId, int eventParticipantId);
    }
}
