using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;

namespace MinistryPlatform.Translation.Repositories
{
    public class KioskRepository : IKioskRepository
    {
        private readonly IApiUserRepository _apiUserRepository;
        private readonly IMinistryPlatformRestRepository _ministryPlatformRestRepository;
        private readonly List<string> _kioskColumns;

        public KioskRepository(IApiUserRepository apiUserRepository, IMinistryPlatformRestRepository ministryPlatformRestRepository)
        {
            _apiUserRepository = apiUserRepository;
            _ministryPlatformRestRepository = ministryPlatformRestRepository;

            _kioskColumns = new List<string>
            {
                "[Kiosk_Config_ID]",
                "[_Kiosk_IDentifier]",
                "[Kiosk_Name]",
                "[Kiosk_Description]",
                "[Kiosk_Type_ID]",
                "[Location_ID]",
                "[Congregation_ID]",
                "[Room_ID]",
                "[Start_Date]",
                "[End_Date]"
            };
        }

        public MpKioskConfigDto GetMpKioskConfigByIdentifier(Guid kioskIdentifier)
        {
            var apiUserToken = _apiUserRepository.GetToken();

            var configs = _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken)
                .Search<MpKioskConfigDto>($"[_Kiosk_Identifier]='{kioskIdentifier}' AND [End_Date] IS NULL", _kioskColumns);

            return configs.FirstOrDefault();
        }
    }
}
