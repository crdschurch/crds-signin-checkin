using MinistryPlatform.Translation.Models.DTO;
using System.Collections.Generic;

namespace MinistryPlatform.Translation.Repositories.Interfaces
{
    public interface IParticipantRepository
    {
        List<MpEventParticipantDto> GetChildParticipantsByEvent(int eventId, string search);
        MpNewParticipantDto CreateParticipantWithContact(MpNewParticipantDto mpNewParticipantDto, string token = null);
        List<MpGroupParticipantDto> CreateGroupParticipants(List<MpGroupParticipantDto> mpGroupParticipantDtos);
        void UpdateEventParticipants(List<MpEventParticipantDto> mpEventParticipantDtos);
        MpEventParticipantDto GetEventParticipantByEventParticipantId(int eventParticipantId);
        List<MpEventParticipantDto> GetEventParticipantsByEventAndParticipant(int eventId, List<int> participantIds);
        List<MpGroupParticipantDto> GetGroupParticipantsByParticipantAndGroupId(int groupId, List<int> participantIds);
        List<MpGroupParticipantDto> GetGroupParticipantsByParticipantId(int participantId);
        List<MpContactDto> GetFamiliesForSearch(string search);
        MpHouseholdDto GetHouseholdByHouseholdId(int householdId);
        void UpdateHouseholdInformation(MpHouseholdDto householdDto);
        void DeleteGroupParticipants(List<MpGroupParticipantDto> groupParticipants);
    }
}
