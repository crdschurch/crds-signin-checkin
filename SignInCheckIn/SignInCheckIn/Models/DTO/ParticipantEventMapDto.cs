using System.Collections.Generic;

namespace SignInCheckIn.Models.DTO
{
    public class ParticipantEventMapDto
    {
        public EventDto CurrentEvent { get; set; }
        public List<ParticipantDto> Participants { get; set; } 
        public List<ContactDto> Contacts { get; set; }
    }
}