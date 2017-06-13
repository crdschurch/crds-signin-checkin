using System;
using System.Web.UI;

namespace SignInCheckIn.Models.DTO
{
    public class ContactDto
    {
        public int ContactId { get; set; }

        public int ParticipantId { get; set; }

        public int HouseholdId { get; set; }

        public int HouseholdPositionId { get; set; }

        public int GenderId { get; set; }

        public int YearGrade { get; set; }

        public bool IsSpecialNeeds { get; set; }

        public string HomePhone { get; set; }

        public string MobilePhone { get; set; }

        public string Nickname { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string DisplayName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string AddressLine1 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Address => $"{AddressLine1}, {City} {State}, {ZipCode}";

        public string CongregationName { get; set; }

        public string EmailAddress { get; set; }
    }
}
