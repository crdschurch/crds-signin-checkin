﻿using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Translation.Models.DTO
{
    [MpRestApiTable(Name = "Addresses")]
    public class MpAddressDto
    {
        [JsonProperty(PropertyName = "Address_ID")]
        public int? AddressId { get; set; }

        [JsonProperty(PropertyName = "Address_Line_1")]
        public string AddressLine1 { get; set; }

        [JsonProperty(PropertyName = "Address_Line_2")]
        public string AddressLine2 { get; set; }

        [JsonProperty(PropertyName = "City")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "State/Region")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "Postal_Code")]
        public string ZipCode { get; set; }

        [JsonProperty(PropertyName = "County")]
        public string County { get; set; }

        [JsonProperty(PropertyName = "Country_Code")]
        public string CountryCode { get; set; }
    }
}
