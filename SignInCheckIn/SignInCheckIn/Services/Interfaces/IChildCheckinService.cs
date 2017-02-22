using System;
using System.Collections.Generic;
using SignInCheckIn.Models.DTO;

namespace SignInCheckIn.Services.Interfaces
{
    public interface IChildCheckinService
    {
        ParticipantEventMapDto GetChildrenForCurrentEventAndRoom(int roomId, int siteId, int? eventId);
        List<ParticipantDto> GetChildrenForCurrentEventAndRoom(int roomId, int eventId)
        ParticipantDto CheckinChildrenForCurrentEventAndRoom(ParticipantDto eventParticipant);
        ParticipantDto GetEventParticipantByCallNumber(int eventId, int callNumber, int roomId, bool? excludeThisRoom = false);
        bool OverrideChildIntoRoom(int eventId, int eventParticipantId, int roomId);
    }
}
