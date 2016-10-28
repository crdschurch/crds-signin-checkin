﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinistryPlatform.Translation.Models.Attributes;
using Newtonsoft.Json;

namespace MinistryPlatform.Translation.Models.DTO
{
    [MpRestApiTable(Name = "cr_Kiosk_Configs")]
    public class MpKioskConfigDto
    {
        [JsonProperty(PropertyName = "Kiosk_Config_ID")]
        public int KioskConfigId { get; set; }

        // TODO: Determine if this actually works - casting to a guid
        [JsonProperty(PropertyName = "_Kiosk_Identifier")]
        public Guid KioskIdentifier { get; set; }

        [JsonProperty(PropertyName = "Kiosk_Name")]
        public string KioskName { get; set; }

        [JsonProperty(PropertyName = "Kiosk_Description")]
        public string KioskDescription { get; set; }

        [JsonProperty(PropertyName = "Kiosk_Type_ID")]
        public int KioskTypeId { get; set; }

        [JsonProperty(PropertyName = "Location_ID")]
        public int LocationId { get; set; }

        [JsonProperty(PropertyName = "Congregation_ID")]
        public int CongregationId { get; set; }

        [JsonProperty(PropertyName = "Room_ID")]
        public int RoomId { get; set; }

        [JsonProperty(PropertyName = "Start_Date")]
        public DateTime StartDate { get; set; }

        [JsonProperty(PropertyName = "End_Date")]
        public DateTime? EndDate { get; set; }
    }
}
