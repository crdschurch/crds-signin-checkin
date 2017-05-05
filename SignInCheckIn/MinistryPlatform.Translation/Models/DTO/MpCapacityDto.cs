namespace MinistryPlatform.Translation.Models.DTO
{
    public class MpCapacityDto
    {
        public string CapacityKey { get; set; } // Nursery, 1Yr, etc
        public int CurrentParticipants { get; set; }
        public int MaxCapacity { get; set; }
    }
}
