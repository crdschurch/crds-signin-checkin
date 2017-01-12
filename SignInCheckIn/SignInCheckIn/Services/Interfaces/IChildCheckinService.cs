using System;
using SignInCheckIn.Models.DTO;

namespace SignInCheckIn.Services.Interfaces
{
    public interface IChildCheckinService
    {
        ParticipantEventMapDto GetChildrenForCurrentEventAndRoom(int roomId, int siteId, int? eventId);
        ParticipantDto CheckinChildrenForCurrentEventAndRoom(ParticipantDto eventParticipant);
        ParticipantDto GetEventParticipantByCallNumber(int eventId, int callNumber, int roomId);
        Boolean OverrideChildIntoRoom(int eventId, int eventParticipantId, int roomId);
    }
}
