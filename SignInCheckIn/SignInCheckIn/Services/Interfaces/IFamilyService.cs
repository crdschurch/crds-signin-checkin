using MinistryPlatform.Translation.Models.DTO;
using SignInCheckIn.Models.DTO;
using System.Collections.Generic;

namespace SignInCheckIn.Services.Interfaces
{
    public interface IFamilyService
    {
        List<ContactDto> CreateNewFamily(List<NewParentDto> newParentDtos, string kioskIdentifier);
        List<MpNewParticipantDto> AddFamilyMembers(int householdId, List<ContactDto> newContacts);
        UserDto GetUserByEmailAddress(string emailAddress);
    }
}
