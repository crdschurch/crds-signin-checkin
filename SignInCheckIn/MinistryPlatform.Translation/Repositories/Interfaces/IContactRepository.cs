using MinistryPlatform.Translation.Models.DTO;
using System.Collections.Generic;

namespace MinistryPlatform.Translation.Repositories.Interfaces
{
    public interface IContactRepository
    {
        List<MpContactDto> GetHeadsOfHouseholdByHouseholdId(int householdId);
        MpHouseholdDto CreateHousehold(MpHouseholdDto mpHouseholdDto);
        MpContactDto Update(MpContactDto contactDto);
        void CreateContactRelationships(List<MpContactRelationshipDto> contactRelationshipDtos);
        MpContactDto GetContactById(int contactId);
        MpUserDto CreateUserRecord(MpUserDto mpUserDto);
        void CreateUserRoles(List<MpUserRoleDto> mpUserRoleDtos);
        void CreateContactPublications(List<MpContactPublicationDto> contactPublicationDtos);
        List<MpUserDto> GetUserByEmailAddress(string emailAddress);
    }
}
