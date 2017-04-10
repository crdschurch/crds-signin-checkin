using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Translation.Models.DTO
{
    [MpRestApiTable(Name = "Address")]
    public class MpAddressDto
    {
        [JsonProperty(PropertyName = "Address_ID")]
        public string AddressId { get; set; }

        [JsonProperty(PropertyName = "Address_Line_1")]
        public string AddressLine1 { get; set; }

        [JsonProperty(PropertyName = "Address_Line_2")]
        public string AddressLine2 { get; set; }

        [JsonProperty(PropertyName = "City")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "State")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "Postal_Code")]
        public string ZipCode { get; set; }

        [JsonProperty(PropertyName = "County")]
        public string County { get; set; }

        [JsonProperty(PropertyName = "Country_Code")]
        public string CountryCode { get; set; }
    }
}
