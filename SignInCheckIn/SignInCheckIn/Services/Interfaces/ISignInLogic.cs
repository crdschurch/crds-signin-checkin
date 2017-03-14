using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;
using SignInCheckIn.Models.DTO;

namespace SignInCheckIn.Services.Interfaces
{
    public interface ISignInLogic
    {
        List<MpEventParticipantDto> SignInParticipants(ParticipantEventMapDto participantEventMapDto, List<MpEventDto> eligibleEvents);
    }
}
