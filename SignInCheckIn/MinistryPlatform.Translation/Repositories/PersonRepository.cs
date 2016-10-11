using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossroads.Utilities.Interfaces;
using MinistryPlatform.Translation.Repositories.Interfaces;

namespace MinistryPlatform.Translation.Repositories
{
    public class PersonRepository
    {
        private readonly IConfigurationWrapper _configurationWrapper;
        private readonly IMinistryPlatformRestRepository _ministryPlatformRestRepository;

        public PersonRepository(IConfigurationWrapper configurationWrapper, IMinistryPlatformRestRepository ministryPlatformRestRepository)
        {
            _configurationWrapper = configurationWrapper;
            _ministryPlatformRestRepository = ministryPlatformRestRepository;
        }

        //public List<MpRoleDTO> GetRolesByToken(string token)
        //{
            
        //} 

        //public static List<RoleDTO> GetMyRoles(string token)
        //{
        //    var pageId = Convert.ToInt32(ConfigurationManager.AppSettings["MyRoles"]);
        //    var pageRecords = MinistryPlatformService.GetRecordsDict(pageId, token);

        //    return pageRecords.Select(record => new RoleDTO
        //    {
        //        Id = (int)record["Role_ID"],
        //        Name = (string)record["Role_Name"]
        //    }).ToList();
        //}
    }
}
