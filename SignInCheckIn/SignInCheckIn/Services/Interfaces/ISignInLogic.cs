using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinistryPlatform.Translation.Models.DTO;

namespace SignInCheckIn.Services.Interfaces
{
    public interface ISignInLogic
    {
        List<MpEventParticipantDto> SignInParticipant(int siteId, bool adventureClubSignIn, bool underThreeSignIn, int groupId);
    }
}
