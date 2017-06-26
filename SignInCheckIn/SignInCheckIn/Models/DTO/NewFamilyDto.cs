using System.Collections.Generic;
using Newtonsoft.Json;

namespace SignInCheckIn.Models.DTO
{
    public class NewFamilyDto
    {
        [JsonProperty(PropertyName = "event")]
        public EventDto EventDto { get; set; }

        [JsonProperty(PropertyName = "parents")]
        public List<NewParentDto> ParentContactDtos { get; set; }

        [JsonProperty(PropertyName = "children")]
        public List<NewChildDto> ChildContactDtos { get; set; }
    }
}