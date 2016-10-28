using System;
using System.Collections.Generic;
using SignInCheckIn.Models.DTO;

namespace SignInCheckIn.Services.Interfaces
{
    public interface IChildSigninService
    {
        List<ParticipantDto> GetChildrenByPhoneNumber(string phoneNumber);
        void SigninParticipants(ParticipantEventMapDto participantEventMapDto);
    }
}
