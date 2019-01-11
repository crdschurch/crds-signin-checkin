using Crossroads.Web.Auth.Models;
using System;

namespace SignInCheckIn.Security
{
    public static class VerifyRoles
    {
        public static void KidsClubTools(AuthDTO authDto)
        {
            const int KidsClubTools = 112;
            if (!authDto.Authorization.MpRoles.ContainsKey(KidsClubTools))
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}