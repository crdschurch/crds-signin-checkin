using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Crossroads.Utilities.Services.Interfaces;

namespace Crossroads.Utilities.Services
{
    public class PasswordService : IPasswordService
    {
        public string GetNewUserPassword(int length, int numSpecialChars)
        {
            var newPassword = System.Web.Security.Membership.GeneratePassword(16, 2);
            return newPassword;
        }

        public string GeneratorPasswordResetToken(string username)
        {
            // create a token -- see http://stackoverflow.com/questions/664673/how-to-implement-password-resets
            var resetArray = Encoding.UTF8.GetBytes(Guid.NewGuid() + username + System.DateTime.Now);
            RNGCryptoServiceProvider prov = new RNGCryptoServiceProvider();
            prov.GetBytes(resetArray);
            var resetToken = Encoding.UTF8.GetString(resetArray);
            string cleanToken = Regex.Replace(resetToken, "[^A-Za-z0-9]", "");

            return cleanToken;
        }
    }
}
