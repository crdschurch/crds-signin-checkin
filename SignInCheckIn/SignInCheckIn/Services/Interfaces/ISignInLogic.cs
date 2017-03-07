using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinistryPlatform.Translation.Models.DTO;
using SignInCheckIn.Models.DTO;

namespace SignInCheckIn.Services.Interfaces
{
    public interface ISignInLogic
    {
        List<MpEventParticipantDto> SignInParticipants(ParticipantEventMapDto participantEventMapDto);
    }
}
