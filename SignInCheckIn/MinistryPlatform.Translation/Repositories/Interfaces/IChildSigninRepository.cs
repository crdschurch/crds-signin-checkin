using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;

namespace MinistryPlatform.Translation.Repositories.Interfaces
{
    public interface IChildSigninRepository
    {
        List<MpParticipantDto> GetChildrenByHouseholdId(int? householdId);
        List<MpEventParticipantDto> CreateEventParticipants(List<MpEventParticipantDto> mpEventParticipantDtos);
        int? GetHouseholdIdByPhoneNumber(string phoneNumber);
    }
}
