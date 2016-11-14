using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;

namespace MinistryPlatform.Translation.Repositories
{
    public class ConfigRepository : IConfigRepository
    {
        private readonly IApiUserRepository _apiUserRepository;
        private readonly IMinistryPlatformRestRepository _ministryPlatformRestRepository;

        private readonly List<string> _mpConfigColumns;

        public ConfigRepository(IApiUserRepository apiUserRepository, IMinistryPlatformRestRepository ministryPlatformRestRepository)
        {
            _apiUserRepository = apiUserRepository;
            _ministryPlatformRestRepository = ministryPlatformRestRepository;

            _mpConfigColumns = new List<string>
            {
                "Configuration_Setting_ID",
                "Application_Code",
                "Key_Name",
                "Value",
                "Description"
            };

        }

        public MpConfigDto GetMpConfigByKey(string key)
        {
            var apiUserToken = _apiUserRepository.GetToken();

            var mpConfigs = _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken)
                .Search<MpConfigDto>($"[Key_Name]='{key}'", _mpConfigColumns);

            if (mpConfigs.Any())
            {
                return mpConfigs.First();
            }

            throw new Exception("No matching config key found for value " + key);
        }
    }
}
