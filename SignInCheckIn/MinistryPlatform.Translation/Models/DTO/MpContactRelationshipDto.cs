﻿using System;
using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json;

namespace MinistryPlatform.Translation.Models.DTO
{
    [MpRestApiTable(Name = "Contact_Relationships")]
    public class MpContactRelationshipDto
    {
        [JsonProperty(PropertyName = "Contact_Relationship_ID")]
        public int ContactRelationshipId { get; set; }

        [JsonProperty(PropertyName = "Contact_ID")]
        public int ContactId { get; set; }

        [JsonProperty(PropertyName = "Relationship_ID")]
        public int RelationshipId { get; set; }

        [JsonProperty(PropertyName = "Related_Contact_ID")]
        public int RelatedContactId { get; set; }

        [JsonProperty(PropertyName = "Start_Date")]
        public DateTime? StartDate { get; set; }

        [JsonProperty(PropertyName = "End_Date")]
        public DateTime? EndDate { get; set; }
    }
}
