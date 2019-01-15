using MinistryPlatform.Translation.Models.DTO;
using System.Collections.Generic;

namespace MinistryPlatform.Translation.Repositories.Interfaces
{
    public interface IGroupRepository
    {
        MpGroupDto GetGroup(string authenticationToken, int groupId, bool includeAttributes = false);
        List<MpGroupDto> GetGroups(IEnumerable<int> groupIds, bool includeAttributes = false);
        List<MpGroupDto> GetGroupsByAttribute(IEnumerable<MpAttributeDto> attributes, bool includeAttributes = false);
        List<MpGroupDto> GetGroupsForParticipantId(int participantId);
    }
}
