using MinistryPlatform.Translation.Models;
using MinistryPlatform.Translation.Models.DTO;
using System.Collections.Generic;

namespace MinistryPlatform.Translation.Repositories.Interfaces
{
    public interface IAttributeRepository
    {
        List<MpAttributeDto> GetAttributesByAttributeTypeId(int attributeTypeId);
        List<MpAttributeDto> GetAttributesByAttributeTypeId(IEnumerable<int> attributeTypeIds);
        MpContactAttributeDto CreateContactAttribute(MpContactAttributeDto attribute);
    }
}
