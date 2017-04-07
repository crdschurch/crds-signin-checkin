using Crossroads.Web.Common;
using Crossroads.Web.Common.MinistryPlatform;

namespace MinistryPlatform.Translation.Models
{
    [MpRestApiTable(Name = "Contact_Attributes")]
    public class MpContactAttributeDto
    {
        public int Contact_ID { get; set; }
        public int Attribute_ID { get; set; }
    }
}