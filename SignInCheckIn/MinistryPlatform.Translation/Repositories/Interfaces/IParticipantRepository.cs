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
        List<MpEventParticipantDto> GetChildParticipantsByEvent(string token, int eventId, string search);
        MpNewParticipantDto CreateParticipantWithContact(MpNewParticipantDto mpNewParticipantDto, string token = null);
        List<MpGroupParticipantDto> CreateGroupParticipants(string token, List<MpGroupParticipantDto> mpGroupParticipantDtos);
        void UpdateEventParticipants(List<MpEventParticipantDto> mpEventParticipantDtos);
        MpEventParticipantDto GetEventParticipantByEventParticipantId(string token, int eventParticipantId);
        List<MpEventParticipantDto> GetEventParticipantsByEventAndParticipant(int eventId, List<int> participantIds);
        List<MpGroupParticipantDto> GetGroupParticipantsByParticipantAndGroupId(int groupId, List<int> participantIds);
        List<MpContactDto> GetFamiliesForSearch(string token, string search);
        MpHouseholdDto GetHouseholdByHouseholdId(string token, int householdId);
        void UpdateHouseholdInformation(string token, MpHouseholdDto householdDto);
        MpParticipantDto Update(MpParticipantDto participanttDto, string token);
    }
}
