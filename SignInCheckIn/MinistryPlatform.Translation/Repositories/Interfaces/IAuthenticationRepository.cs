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

        //Dictionary<string, object> Authenticate(string username, string password);

        //Boolean ChangePassword(string token, string emailAddress, string firstName, string lastName, string password, string mobilephone);

        ///// <summary>
        ///// Change a users password
        ///// </summary>
        ///// <param name="token"></param>
        ///// <param name="newPassword"></param>
        ///// <returns></returns>
        //Boolean ChangePassword(string token, string newPassword);

        ////Get ID of currently logged in user
        //int GetContactId(string token);
    }
}