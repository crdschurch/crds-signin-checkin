using System;
using System.Collections.Generic;
using SignInCheckIn.Models.DTO;

namespace SignInCheckIn.Services.Interfaces
{
    public interface IChildSigninService
    {
        ParticipantEventMapDto GetChildrenAndEventByPhoneNumber(string phoneNumber, int siteId);
        ParticipantEventMapDto SigninParticipants(ParticipantEventMapDto participantEventMapDto);
        ParticipantEventMapDto PrintParticipants(ParticipantEventMapDto participantEventMapDto, string kioskIdentifier);
        void CreateNewFamily(string token, NewFamilyDto newFamilyDto);
    }
}
