using System;
using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;
using SignInCheckIn.Models.DTO;

namespace SignInCheckIn.Services.Interfaces
{
    public interface IChildSigninService
    {
        ParticipantEventMapDto GetChildrenAndEventByPhoneNumber(string phoneNumber, int siteId, EventDto existingEventDto);
        ParticipantEventMapDto SigninParticipants(ParticipantEventMapDto participantEventMapDto);
        ParticipantEventMapDto PrintParticipants(ParticipantEventMapDto participantEventMapDto, string kioskIdentifier);
        void CreateNewFamily(string token, NewFamilyDto newFamilyDto, string kioskIdentifier);
        List<MpNewParticipantDto> SaveNewFamilyData(string token, NewFamilyDto newFamilyDto);
        List<int> CheckAcEventStatus(ParticipantEventMapDto participantEventMapDto);
    }
}
