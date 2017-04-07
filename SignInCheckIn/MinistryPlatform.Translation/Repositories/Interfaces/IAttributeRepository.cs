using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;

namespace MinistryPlatform.Translation.Repositories.Interfaces
{
    public interface IAttributeRepository
    {
        List<MpAttributeDto> GetAttributesByAttributeTypeId(int attributeTypeId, string authenticationToken = null);
        List<MpAttributeDto> GetAttributesByAttributeTypeId(IEnumerable<int> attributeTypeIds, string authenticationToken = null);
        MpAttributeDto CreateAttribute(MpAttributeDto attribute);
    }
}
