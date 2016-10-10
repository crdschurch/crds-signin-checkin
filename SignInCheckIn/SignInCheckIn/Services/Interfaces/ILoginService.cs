using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignInCheckIn.Services.Interfaces
{
    public interface ILoginService
    {
        void Login(string username, string password);
    }
}
