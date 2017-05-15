using Newtonsoft.Json;

namespace MinistryPlatform.Translation.Models.DTO
{
    public class MpCapacityDto
    {
        [JsonProperty(PropertyName = "Age_Bracket_Key")]
        public string CapacityKey { get; set; } // Nursery, 1Yr, etc

        [JsonProperty(PropertyName = "Attendance")]
        public int CurrentParticipants { get; set; }

        [JsonProperty(PropertyName = "Capacity")]
        public int MaxCapacity { get; set; }
    }
}
