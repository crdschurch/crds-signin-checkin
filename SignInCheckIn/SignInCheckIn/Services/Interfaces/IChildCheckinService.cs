using SignInCheckIn.Models.DTO;

namespace SignInCheckIn.Services.Interfaces
{
    public interface IChildCheckinService
    {
        ParticipantEventMapDto GetChildrenForCurrentEventAndRoom(int roomId, int siteId, int? eventId);
        void CheckinChildrenForCurrentEventAndRoom(bool checkIn, int eventParticipantId);
    }
}
