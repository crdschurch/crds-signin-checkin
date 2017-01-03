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
        List<MpEventParticipantDto> GetChildParticipantsByEvent(string token, List<int> eventIds);
        MpNewParticipantDto CreateParticipantWithContact(string token, MpNewParticipantDto mpNewParticipantDto);
        List<MpGroupParticipantDto> CreateGroupParticipants(string token, List<MpGroupParticipantDto> mpGroupParticipantDtos);
        void UpdateEventParticipants(List<MpEventParticipantDto> mpEventParticipantDtos);
    }
}
