using SignInCheckIn.Models.DTO;

namespace SignInCheckIn.Services.Interfaces
{
    public interface IChildCheckinService
    {
        ParticipantEventMapDto GetChildrenForCurrentEventAndRoom(int roomId, int siteId, int? eventId);
        ParticipantDto CheckinChildrenForCurrentEventAndRoom(ParticipantDto eventParticipant);
        ParticipantDto GetEventParticipantByCallNumber(int eventId, int callNumber);
    }
}
