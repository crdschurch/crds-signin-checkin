using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SignInCheckIn.Models.DTO
{
    public class AgeGradeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<AgeRangeDto> Ranges { get; set; }
        public int SortOrder { get; set; }
        public int TypeId { get; set; }
        public int EventGroupId { get; set; }

#region Selected Property
        private bool _selected;

        public bool Selected
        {
            get { return HasRanges ? !Ranges.Exists(r => !r.Selected) : _selected; }
            set
            {
                if (!HasRanges)
                {
                    _selected = value;
                }
            }
        }
#endregion

        [JsonIgnore]
        public bool HasRanges => Ranges != null && Ranges.Any();

        public class AgeRangeDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public bool Selected { get; set; }
            public int SortOrder { get; set; }
            public int TypeId { get; set; }
            public List<int> EventGroupIds { get; set; }
        }
    }
}