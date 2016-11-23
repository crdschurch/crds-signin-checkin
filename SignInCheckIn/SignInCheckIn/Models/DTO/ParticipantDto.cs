using System;
using System.Linq;
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

        public string CallNumber
        {
            // TODO Faking out a call number for now (last 4 of EventParticipantId), eventually need to store a real call number on Event Participant
            get
            {
                var c = $"0000{EventParticipantId}";
                return c.Substring(c.Length - 4);
            }
        }

        public string SignInErrorMessage { get; set; }

        public bool ErrorSigningIn => Selected && !string.IsNullOrWhiteSpace(SignInErrorMessage);

        public bool SignedIn => Selected && AssignedRoomId != null;

        public bool NotSignedIn => Selected && AssignedRoomId == null && string.IsNullOrWhiteSpace(SignInErrorMessage);

        public int? GroupId { get; set; }
    }
}