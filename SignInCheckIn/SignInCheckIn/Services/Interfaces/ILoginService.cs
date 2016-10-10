using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignInCheckIn.Models.Authentication;

namespace SignInCheckIn.Services.Interfaces
{
    public interface ILoginService
    {
        LoginReturn Login(string username, string password);
    }
}
