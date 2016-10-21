using System.Collections.Generic;
using System.Linq;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;

namespace MinistryPlatform.Translation.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly IMinistryPlatformRestRepository _ministryPlatformRestRepository;
        private readonly IApiUserRepository _apiUserRepository;
        private readonly List<string> _attributeColumns;

        public GroupRepository(IMinistryPlatformRestRepository ministryPlatformRestRepository, IApiUserRepository apiUserRepository)
        {
            _ministryPlatformRestRepository = ministryPlatformRestRepository;
            _apiUserRepository = apiUserRepository;

            _attributeColumns = new List<string>
            {
                "Attribute_ID_Table.[Attribute_ID]",
                "Attribute_ID_Table.[Attribute_Name]",
                "Attribute_ID_Table.[Sort_Order]",
                "Attribute_ID_Table_Attribute_Type_ID_Table.[Attribute_Type_ID]",
                "Attribute_ID_Table_Attribute_Type_ID_Table.[Attribute_Type]"
            };
        }

        public MpGroupDto GetGroup(string authenticationToken, int groupId, bool includeAttributes = false)
        {
            var token = authenticationToken ?? _apiUserRepository.GetToken();
            var group =  _ministryPlatformRestRepository.UsingAuthenticationToken(token).Get<MpGroupDto>(groupId);
            return includeAttributes
                ? group.SetKidsClubAttributes(_ministryPlatformRestRepository.UsingAuthenticationToken(token).SearchTable<MpAttributeDto>("Group_Attributes", $"Group_Attributes.Group_ID = {group.Id}", _attributeColumns))
                : group;
        }

        public List<MpGroupDto> GetGroups(string authenticationToken, IEnumerable<int> groupIds, bool includeAttributes = false)
        {
            var token = authenticationToken ?? _apiUserRepository.GetToken();
            var searchString = $"Group_ID IN ({string.Join(",", groupIds)})";
            var groups =  _ministryPlatformRestRepository.UsingAuthenticationToken(token).Search<MpGroupDto>(searchString);
            if (!includeAttributes)
            {
                return groups;
            }

            return groups.Select(
                g =>
                    g.SetKidsClubAttributes(_ministryPlatformRestRepository.UsingAuthenticationToken(token)
                                                .SearchTable<MpAttributeDto>("Group_Attributes", $"Group_Attributes.Group_ID = {g.Id}", _attributeColumns))).ToList();
        }
    }
}
