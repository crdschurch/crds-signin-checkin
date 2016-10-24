using System.Collections.Generic;
using System.Linq;
using MinistryPlatform.Translation.Models;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;

namespace MinistryPlatform.Translation.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly IMinistryPlatformRestRepository _ministryPlatformRestRepository;
        private readonly IApiUserRepository _apiUserRepository;
        private readonly List<string> _attributeColumns;
        private readonly KidsClubGroupAttributesConfiguration _kcGroupAttributesConfiguration;

        public GroupRepository(IMinistryPlatformRestRepository ministryPlatformRestRepository, IApiUserRepository apiUserRepository, KidsClubGroupAttributesConfiguration kcGroupAttributesConfig)
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

            _kcGroupAttributesConfiguration = kcGroupAttributesConfig;
        }

        public MpGroupDto GetGroup(string authenticationToken, int groupId, bool includeAttributes = false)
        {
            var token = authenticationToken ?? _apiUserRepository.GetToken();
            var group =  _ministryPlatformRestRepository.UsingAuthenticationToken(token).Get<MpGroupDto>(groupId);
            return SetKidsClubGroupAttributes(group, includeAttributes, token);
        }

        public List<MpGroupDto> GetGroups(string authenticationToken, IEnumerable<int> groupIds, bool includeAttributes = false)
        {
            var token = authenticationToken ?? _apiUserRepository.GetToken();
            var searchString = $"Group_ID IN ({string.Join(",", groupIds)})";
            var groups =  _ministryPlatformRestRepository.UsingAuthenticationToken(token).Search<MpGroupDto>(searchString);

            return groups.Select(g => SetKidsClubGroupAttributes(g, includeAttributes, token)).ToList();
        }

        private MpGroupDto SetKidsClubGroupAttributes(MpGroupDto group, bool includeAttributes, string token)
        {
            if (!includeAttributes)
            {
                return group;
            }

            var attributes = _ministryPlatformRestRepository.UsingAuthenticationToken(token)
                .SearchTable<MpAttributeDto>("Group_Attributes", $"Group_Attributes.Group_ID = {group.Id}", _attributeColumns);
            if (attributes == null || !attributes.Any())
            {
                return group;
            }
            group.AgeRange = attributes.Find(a => a.Type.Id == _kcGroupAttributesConfiguration.AgesAttributeTypeId);
            group.Grade = attributes.Find(a => a.Type.Id == _kcGroupAttributesConfiguration.GradesAttributeTypeId);
            group.BirthMonth = attributes.Find(a => a.Type.Id == _kcGroupAttributesConfiguration.BirthMonthsAttributeTypeId);
            group.NurseryMonth = attributes.Find(a => a.Type.Id == _kcGroupAttributesConfiguration.NurseryAgesAttributeTypeId);

            return group;

        }
    }
}
