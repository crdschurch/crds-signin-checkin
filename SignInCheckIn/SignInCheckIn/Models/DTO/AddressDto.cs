using System.Web.UI;

namespace SignInCheckIn.Models.DTO
{
    public class AddressDto
    {
        public string AddressId { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string County { get; set; }
        public string CountryCode { get; set; }
    }
}
