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
        ParticipantEventMapDto PrintParticipant(int eventParticipantId, string kioskIdentifier, string token);
        ParticipantEventMapDto PrintParticipants(ParticipantEventMapDto participantEventMapDto, string kioskIdentifier);
        void CreateNewFamily(string token, NewFamilyDto newFamilyDto, string kioskIdentifier);
        List<MpNewParticipantDto> SaveNewFamilyData(string token, NewFamilyDto newFamilyDto);
        List<MpEventDto> GetEventsForSignin(ParticipantEventMapDto participantEventMapDto);
        void ReverseSignin(string token, int eventParticipantId);
    }
}
