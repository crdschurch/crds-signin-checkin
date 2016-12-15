using System.Collections.Generic;
using System.Linq;

namespace MinistryPlatform.Translation.Models.DTO
{
    public class MpHouseholdParticipantsDto
    {
        public int? HouseholdId { get; set; }
        public List<MpParticipantDto> Participants { get; set; }

        public bool HasHousehold => HouseholdId.HasValue && HouseholdId != 0;

        public bool HasParticipants => Participants != null && Participants.Any();
    }
}
