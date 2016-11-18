namespace SignInCheckIn.Models.DTO
{
    public class ContactDto
    {
        public int ContactId { get; set; }

        public int HouseholdId { get; set; }

        public int HouseholdPositionId { get; set; }

        public string HomePhone { get; set; }

        public string MobilePhone { get; set; }

        public string Nickname { get; set; }

        public string LastName { get; set; }
    }
}