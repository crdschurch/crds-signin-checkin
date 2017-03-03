using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignInCheckIn.Services.Interfaces
{
    public interface ISignInLogic
    {
        void SignInParticipant(int siteId, bool underThreeSignIn, bool adventureClubSignIn);
    }
}
