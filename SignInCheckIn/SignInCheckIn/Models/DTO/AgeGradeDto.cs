using System.Collections.Generic;

namespace SignInCheckIn.Models.DTO
{
    public class AgeGradeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<AgeRangeDto> Ranges { get; set; }
        public bool Selected { get; set; }
        public int SortOrder { get; set; }
        public int TypeId { get; set; }

        public class AgeRangeDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public bool Selected { get; set; }
            public int SortOrder { get; set; }
            public int TypeId { get; set; }
        }
    }
}