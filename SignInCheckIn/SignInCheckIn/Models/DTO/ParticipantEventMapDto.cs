using System.Collections.Generic;

namespace SignInCheckIn.Models.DTO
{
    public class ParticipantEventMapDto
    {
        public EventDto CurrentEvent { get; set; }
        public List<ParticipantDto> Participants { get; set; } 
        public List<ContactDto> Contacts { get; set; }
        public int ServicesAttended { get; set; }
        public int HouseholdId { get; set; }
        public string HouseholdPhoneNumber { get; set; }
        public int KioskTypeId { get; set; }
    }
}