using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;

namespace MinistryPlatform.Translation.Repositories.Interfaces
{
    public interface IChildSigninRepository
    {
        List<MpParticipantDto> GetChildrenByPhoneNumber(string phoneNumber, MpEventDto eventDto);
        List<MpEventParticipantDto> CreateEventParticipants(List<MpEventParticipantDto> mpEventParticipantDtos);
    }
}
