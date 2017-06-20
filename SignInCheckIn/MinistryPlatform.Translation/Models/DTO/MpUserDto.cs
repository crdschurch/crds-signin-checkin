using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Translation.Models.DTO
{
    [MpRestApiTable(Name = "dp_Users")]
    public class MpUserDto
    {
        [JsonProperty(PropertyName = "User_ID")]
        public int UserId { get; set; }

        [JsonProperty(PropertyName = "First_Name")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "Last_Name")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "User_Email")]
        public string UserEmail { get; set; }

        [JsonProperty(PropertyName = "Password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "Company")]
        public bool Company { get; set; }

        [JsonProperty(PropertyName = "Display_Name")]
        public string DisplayName { get; set; }

        [JsonProperty(PropertyName = "Domain_ID")]
        public int DomainId { get; set; }

        [JsonProperty(PropertyName = "User_Name")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "Contact_ID")]
        public int ContactId { get; set; }

        [JsonProperty(PropertyName = "PasswordResetToken")]
        public string PasswordResetToken { get; set; }
    }
}
