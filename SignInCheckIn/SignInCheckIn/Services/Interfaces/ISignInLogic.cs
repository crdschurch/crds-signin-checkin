using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;
using SignInCheckIn.Models.DTO;

namespace SignInCheckIn.Services.Interfaces
{
    public interface ISignInLogic
    {
        List<ParticipantDto> SignInParticipants(ParticipantEventMapDto participantEventMapDto, List<MpEventDto> eligibleEvents);
    }
}
