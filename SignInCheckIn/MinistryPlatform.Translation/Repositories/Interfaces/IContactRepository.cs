using System;
using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;

namespace MinistryPlatform.Translation.Repositories.Interfaces
{
    public interface IContactRepository
    {
        List<MpContactDto> GetHeadsOfHouseholdByHouseholdId(int householdId);
        MpHouseholdDto CreateHousehold(string token, MpHouseholdDto mpHouseholdDto);
        MpContactDto Update(MpContactDto contactDto, string token);
        void CreateContactRelationships(string token, List<MpContactRelationshipDto> contactRelationshipDtos);
        MpContactDto GetContactById(string token, int contactId);
        MpUserDto CreateUserRecord(string token, MpUserDto mpUserDto);
        void CreateUserRoles(string token, List<MpUserRoleDto> mpUserRoleDtos);
        void CreateContactPublications(string token, List<MpContactPublicationDto> contactPublicationDtos);
        List<MpUserDto> GetUserByEmailAddress(string token, string emailAddress);
    }
}
