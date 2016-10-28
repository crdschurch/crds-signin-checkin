using System;
using System.Collections.Generic;
using SignInCheckIn.Models.DTO;

namespace SignInCheckIn.Services.Interfaces
{
    public interface IChildSigninService
    {
        ParticipantEventMapDto GetChildrenAndEventByPhoneNumber(string phoneNumber, int siteId);
        void SigninParticipants(ParticipantEventMapDto participantEventMapDto);
    }
}
