using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinistryPlatform.Translation.Models.DTO;

namespace MinistryPlatform.Translation.Repositories.Interfaces
{
    public interface IParticipantRepository
    {
        List<MpEventParticipantDto> GetChildParticipantsByEvent(int eventId);
        List<MpContactDto> GetHeadsOfHouseholdByHouseholdId(int householdId);
        List<MpNewParticipantDto> CreateParticipantsWithContacts(string token, List<MpNewParticipantDto> mpParticipantDtos);
    }
}
