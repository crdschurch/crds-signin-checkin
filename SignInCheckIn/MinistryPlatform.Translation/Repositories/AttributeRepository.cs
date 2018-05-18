using System.Collections.Generic;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Translation.Models;
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
            var token = authenticationToken ?? _apiUserRepository.GetDefaultApiClientToken();

            return GetAttributesByAttributeTypeId(new[] {attributeTypeId}, token);
        }

        public List<MpAttributeDto> GetAttributesByAttributeTypeId(IEnumerable<int> attributeTypeIds, string authenticationToken = null)
        {
            return _ministryPlatformRestRepository.UsingAuthenticationToken(authenticationToken ?? _apiUserRepository.GetDefaultApiClientToken()).Search<MpAttributeDto>($"Attribute_Type_ID_Table.[Attribute_Type_ID] IN ({string.Join(",", attributeTypeIds)})", _attributeColumns);
        }

        public MpContactAttributeDto CreateContactAttribute(MpContactAttributeDto attribute)
        {
            var token = _apiUserRepository.GetDefaultApiClientToken();
            var attributeColumns = new List<string>
                    {
                        "Contact_ID",
                        "Attribute_ID"
                    };   
            return _ministryPlatformRestRepository.UsingAuthenticationToken(token).Create(attribute, attributeColumns);
        }
    }
}
