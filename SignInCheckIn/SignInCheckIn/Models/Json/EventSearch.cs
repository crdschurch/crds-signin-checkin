using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignInCheckIn.Models.Json
{
    public class EventSearch
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<int> EventTypeIds { get; set; } 
    }
}