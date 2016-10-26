using System.Collections.Generic;
using System.Linq;
using Crossroads.Utilities.Services.Interfaces;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;

namespace MinistryPlatform.Translation.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly IMinistryPlatformRestRepository _ministryPlatformRestRepository;
        private readonly IApiUserRepository _apiUserRepository;
        private readonly List<string> _attributeColumns;
        private readonly List<string> _groupColumns;
        private readonly IApplicationConfiguration _applicationConfiguration;

        public GroupRepository(IApiUserRepository apiUserRepository, IMinistryPlatformRestRepository ministryPlatformRestRepository, IApplicationConfiguration applicationConfiguration)
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

            _groupColumns = new List<string>
            {
                "Groups.Group_ID",
                "Groups.Group_Name"
            };

            _applicationConfiguration = applicationConfiguration;
        }

        public MpGroupDto GetGroup(string authenticationToken, int groupId, bool includeAttributes = false)
        {
            var token = authenticationToken ?? _apiUserRepository.GetToken();
            var group =  _ministryPlatformRestRepository.UsingAuthenticationToken(token).Get<MpGroupDto>(groupId, _groupColumns);
            return SetKidsClubGroupAttributes(group, includeAttributes, token);
        }

        public List<MpGroupDto> GetGroups(string authenticationToken, IEnumerable<int> groupIds, bool includeAttributes = false)
        {
            var token = authenticationToken ?? _apiUserRepository.GetToken();
            var searchString = $"Group_ID IN ({string.Join(",", groupIds)})";
            var groups =  _ministryPlatformRestRepository.UsingAuthenticationToken(token).Search<MpGroupDto>(searchString, _groupColumns);

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
            group.AgeRange = attributes.Find(a => a.Type.Id == _applicationConfiguration.AgesAttributeTypeId);
            group.Grade = attributes.Find(a => a.Type.Id == _applicationConfiguration.GradesAttributeTypeId);
            group.BirthMonth = attributes.Find(a => a.Type.Id == _applicationConfiguration.BirthMonthsAttributeTypeId);
            group.NurseryMonth = attributes.Find(a => a.Type.Id == _applicationConfiguration.NurseryAgesAttributeTypeId);

            return group;

        }
    }
}
