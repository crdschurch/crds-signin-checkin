using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crossroads.Utilities.Services.Interfaces
{
    public interface IPasswordService
    {
        string GetNewUserPassword(int length, int numSpecialChars);
        string GeneratorPasswordResetToken(string username);
    }
}
