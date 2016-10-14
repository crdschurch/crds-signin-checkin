using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Models.Json;

namespace SignInCheckIn.Services.Interfaces
{
    public interface IEventService
    {
        List<EventDto> GetCheckinEvents();
    }
}
