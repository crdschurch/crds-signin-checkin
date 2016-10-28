using System;

namespace SignInCheckIn.Models.DTO
{
    public class ParticipantDto
    {
        public int ParticipantId { get; set; }
        public int ContactId { get; set; }
        public int HouseholdId { get; set; }
        public int HouseholdPositionId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}