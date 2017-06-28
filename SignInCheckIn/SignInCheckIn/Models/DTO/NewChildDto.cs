using System;

namespace SignInCheckIn.Models.DTO
{
    public class NewChildDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int? YearGrade { get; set; }
        public int? HouseholdId { get; set; }
    }
}