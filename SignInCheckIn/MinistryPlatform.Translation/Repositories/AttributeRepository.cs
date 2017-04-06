using System.Collections.Generic;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;

namespace MinistryPlatform.Translation.Repositories
{
    public class AttributeRepository : IAttributeRepository
    {
        private readonly IApiUserRepository _apiUserRepository;
        private readonly IMinistryPlatformRestRepository _ministryPlatformRestRepository;
        private readonly List<string> _attributeColumns;

        public AttributeRepository(IApiUserRepository apiUserRepository, IMinistryPlatformRestRepository ministryPlatformRestRepository)
        {
            _apiUserRepository = apiUserRepository;
            _ministryPlatformRestRepository = ministryPlatformRestRepository;

            _attributeColumns = new List<string>
            {
                "Attributes.[Attribute_ID]",
                "Attributes.[Attribute_Name]",
                "Attributes.[Sort_Order]",
                "Attribute_Type_ID_Table.[Attribute_Type_ID]",
                "Attribute_Type_ID_Table.[Attribute_Type]"
            };
        }

        public List<MpAttributeDto> GetAttributesByAttributeTypeId(int attributeTypeId, string authenticationToken = null)
        {
            var token = authenticationToken ?? _apiUserRepository.GetToken();

            return GetAttributesByAttributeTypeId(new[] {attributeTypeId}, token);
        }

        public List<MpAttributeDto> GetAttributesByAttributeTypeId(IEnumerable<int> attributeTypeIds, string authenticationToken = null)
        {
            return _ministryPlatformRestRepository.UsingAuthenticationToken(authenticationToken ?? _apiUserRepository.GetToken()).Search<MpAttributeDto>($"Attribute_Type_ID_Table.[Attribute_Type_ID] IN ({string.Join(",", attributeTypeIds)})", _attributeColumns);
        }

        public int CreateAttribute(MpAttributeDto attribute)
        {
            var token = _apiUserRepository.GetToken();
            var attributeColumns = new Dictionary<string, object>
                    {
                        {"Attribute_Name", attribute.Name.ToLower()},
                        {"Attribute_Category_ID", attribute.CategoryId},
                        {"Attribute_Type_ID", attribute.AttributeTypeId},
                        {"PreventMultipleSelection", attribute.PreventMultipleSelection},
                        {"Sort_Order", attribute.SortOrder}
                    };
            
            return _ministryPlatformRestRepository.UsingAuthenticationToken(token).Create(attribute, attributeColumns);
        }

    }
}
