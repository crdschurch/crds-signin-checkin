using System;
using MinistryPlatform.Translation.Models.Attributes;
using Newtonsoft.Json;

namespace MinistryPlatform.Translation.Models.DTO
{
    [MpRestApiTable(Name = "cr_Kiosk_Configs")]
    public class MpKioskConfigDto
    {
        [JsonProperty(PropertyName = "Kiosk_Config_ID")]
        public int KioskConfigId { get; set; }

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

        [JsonProperty(PropertyName = "Congregation_Name")]
        public string CongregationName { get; set; }

        [JsonProperty(PropertyName = "Room_ID")]
        public int RoomId { get; set; }

        [JsonProperty(PropertyName = "Room_Name")]
        public string RoomName { get; set; }

        [JsonProperty(PropertyName = "Start_Date")]
        public DateTime StartDate { get; set; }

        [JsonProperty(PropertyName = "End_Date")]
        public DateTime? EndDate { get; set; }

        [JsonProperty(PropertyName = "Printer_Map_ID")]
        public int? PrinterMapId { get; set; }
    }
}
