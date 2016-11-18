using System;
using Microsoft.Practices.ObjectBuilder2;

namespace SignInCheckIn.Models.DTO
{
    public class ParticipantDto
    {
        public int EventParticipantId { get; set; }
        public int ParticipantId { get; set; }
        public int ContactId { get; set; }
        public int HouseholdId { get; set; }
        public int HouseholdPositionId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool Selected { get; set; } = false;
        public int ParticipationStatusId { get; set; }
        public int? AssignedRoomId { get; set; }
        public string AssignedRoomName { get; set; }
        public int? AssignedSecondaryRoomId { get; set; } // adventure club field
        public string AssignedSecondaryRoomName { get; set; } // adventure club field
    }
}