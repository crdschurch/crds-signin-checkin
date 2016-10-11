using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Win32;

namespace MinistryPlatform.Translation.Repositories.Interfaces
{
    public interface IAuthenticationRepository
    {
        Dictionary<string, object> Authenticate(string username, string password);
        Dictionary<string, object> RefreshToken(string refreshToken);
        List<string> GetUserRolesFromToken(string token);
    }
}