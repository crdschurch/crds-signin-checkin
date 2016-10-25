using System.Collections.Generic;

namespace MinistryPlatform.Translation.Models.DTO
{
    public class MpAgeRangeDto
    {
        public string Name { get; set; }
        public List<MpAttributeDto> Ranges { get; set; }
    }
}
