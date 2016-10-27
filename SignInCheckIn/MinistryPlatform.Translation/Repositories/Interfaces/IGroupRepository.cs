using System;
using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;

namespace MinistryPlatform.Translation.Repositories.Interfaces
{
    public interface IGroupRepository
    {
        MpGroupDto GetGroup(string authenticationToken, int groupId, bool includeAttributes = false);
        List<MpGroupDto> GetGroups(string authenticationToken, IEnumerable<int> groupIds, bool includeAttributes = false);
        List<MpGroupDto> GetGroupsByAttribute(string authenticationToken, IEnumerable<MpAttributeDto> attributes);
    }
}
