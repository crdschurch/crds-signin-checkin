using Crossroads.Web.Auth.Models;
using System;

namespace SignInCheckIn.Security
{
    public static class VerifyRoles
    {
        private const int KidsClubToolsId = 112;
        public static void KidsClubTools(AuthDTO authDto)
        {
            if (!authDto.Authorization.MpRoles.ContainsKey(KidsClubToolsId))
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}