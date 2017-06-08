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
    }
}
