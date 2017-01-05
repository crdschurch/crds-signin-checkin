using System;
using System.Linq;
using Microsoft.Practices.ObjectBuilder2;
using Newtonsoft.Json;

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

        [JsonIgnore]
        public DateTime? TimeIn { get; set; } = null;
        [JsonIgnore]
        public DateTime? TimeConfirmed { get; set; } = null;
        [JsonIgnore]
        public DateTime? TimeOut { get; set; } = null;
        [JsonIgnore]
        public int Notes { get; set; }
        [JsonIgnore]
        public int? GroupParticipantId { get; set; }
        [JsonIgnore]
        public int? CheckinStation { get; set; }
        [JsonIgnore]
        public bool CallParents { get; set; }
        [JsonIgnore]
        public int? GroupRoleId { get; set; }
        [JsonIgnore]
        public int? ResponseId { get; set; }
        [JsonIgnore]
        public int? OpportunityId { get; set; }
        [JsonIgnore]
        public bool RegistrantMessageSent { get; set; }
        [JsonIgnore]
        public int EventId { get; set; }

        public string CallNumber { get; set; }

        public string SignInErrorMessage { get; set; }

        public bool ErrorSigningIn => Selected && !string.IsNullOrWhiteSpace(SignInErrorMessage);

        public bool SignedIn => Selected && AssignedRoomId != null;

        public bool NotSignedIn => Selected && AssignedRoomId == null && string.IsNullOrWhiteSpace(SignInErrorMessage);

        public int? GroupId { get; set; }

        public bool GuestSignin { get; set; }

        public int? YearGrade { get; set; }

        public string CheckinPhone { get; set; }

        public int? CheckinHouseholdId { get; set; }
    }
}
