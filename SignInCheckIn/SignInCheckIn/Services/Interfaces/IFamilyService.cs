using System;
using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;
using SignInCheckIn.Models.DTO;

namespace SignInCheckIn.Services.Interfaces
{
    public interface IFamilyService
    {
        List<ContactDto> CreateNewFamily(string token, List<NewParentDto> newParentDtos, string kioskIdentifier);
        List<MpNewParticipantDto> AddFamilyMembers(string token, int householdId, List<ContactDto> newContacts);
        MpUserDto GetUserByEmailAddress(string token, string emailAddress);
    }
}
