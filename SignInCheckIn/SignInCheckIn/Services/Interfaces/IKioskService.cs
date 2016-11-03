using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignInCheckIn.Models.DTO;

namespace SignInCheckIn.Services.Interfaces
{
    public interface IKioskService
    {
        KioskConfigDto GetKioskConfigByIdentifier(Guid kioskIdentifier);
    }
}
