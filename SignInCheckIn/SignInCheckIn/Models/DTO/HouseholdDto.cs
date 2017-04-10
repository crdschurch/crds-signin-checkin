using System.Web.UI;

namespace SignInCheckIn.Models.DTO
{
    public class HouseholdDto
    {
        public int HouseholdId { get; set; }
        public string HomePhone { get; set; }
        public string HouseholdName { get; set; }
        public int CongregationId { get; set; }
        public int HouseholdSourceId { get; set; }
        public string AddressId { get; set; }
    }
}
