using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Translation.Models.DTO
{
    [MpRestApiTable(Name = "dp_User_Roles")]
    public class MpUserRoleDto
    {
        [JsonProperty(PropertyName = "User_ID")]
        public int UserId { get; set; }

        [JsonProperty(PropertyName = "Role_ID")]
        public int RoleId { get; set; }
    }
}
