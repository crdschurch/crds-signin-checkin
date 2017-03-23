using System;
using System.Collections.Generic;
using System.Linq;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;

namespace MinistryPlatform.Translation.Repositories
{
    public class KioskRepository : IKioskRepository
    {
        private readonly IApiUserRepository _apiUserRepository;
        private readonly IMinistryPlatformRestRepository _ministryPlatformRestRepository;
        private readonly List<string> _kioskColumns;
        private readonly List<string> _printerMapColumns; 

        public KioskRepository(IApiUserRepository apiUserRepository, IMinistryPlatformRestRepository ministryPlatformRestRepository)
        {
            _apiUserRepository = apiUserRepository;
            _ministryPlatformRestRepository = ministryPlatformRestRepository;

            _kioskColumns = new List<string>
            {
                "[Kiosk_Config_ID]",
                "[_Kiosk_Identifier]",
                "[Kiosk_Name]",
                "[Kiosk_Description]",
                "[Kiosk_Type_ID]",
                "cr_Kiosk_Configs.[Location_ID]",
                "cr_Kiosk_Configs.[Congregation_ID]",
                "Congregation_ID_Table.[Congregation_Name]",
                "cr_Kiosk_Configs.[Room_ID]",
                "Room_ID_Table.Room_Name",
                "cr_Kiosk_Configs.[Start_Date]",
                "cr_Kiosk_Configs.[End_Date]",
                "Printer_Map_ID"
            };

            _printerMapColumns = new List<string>
            {
                "[Printer_Map_ID]",
                "[Printer_ID]",
                "[Printer_Name]",
                "[Computer_ID]",
                "[Computer_Name]"
            };
        }

        public MpKioskConfigDto GetMpKioskConfigByIdentifier(Guid kioskIdentifier)
        {
            var apiUserToken = _apiUserRepository.GetToken();

            var configs = _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken)
                .Search<MpKioskConfigDto>($"[_Kiosk_Identifier]='{kioskIdentifier}' AND cr_Kiosk_Configs.[End_Date] IS NULL", _kioskColumns);

            return configs.FirstOrDefault();
        }

        public MpPrinterMapDto GetPrinterMapById(int printerMapId)
        {
            var apiUserToken = _apiUserRepository.GetToken();

            var configs = _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken)
                .Search<MpPrinterMapDto>($"[Printer_Map_ID]={printerMapId}", _printerMapColumns);

            return configs.FirstOrDefault();
        }
    }
}
