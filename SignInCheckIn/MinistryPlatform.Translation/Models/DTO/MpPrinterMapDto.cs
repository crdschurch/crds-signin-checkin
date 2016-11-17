using MinistryPlatform.Translation.Models.Attributes;
using Newtonsoft.Json;

namespace MinistryPlatform.Translation.Models.DTO
{
    [MpRestApiTable(Name = "cr_Printer_Maps")]
    public class MpPrinterMapDto
    {
        [JsonProperty(PropertyName = "Printer_Map_ID")]
        public int PrinterMapId { get; set; }

        [JsonProperty(PropertyName = "Printer_ID")]
        public int PrinterId { get; set; }

        [JsonProperty(PropertyName = "Printer_Name")]
        public int PrinterName { get; set; }

        [JsonProperty(PropertyName = "Computer_ID")]
        public int ComputerId { get; set; }

        [JsonProperty(PropertyName = "Computer_Name")]
        public int ComputerName { get; set; }
    }
}
