using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignInCheckIn.Models.DTO
{
    public class NewParentDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public int CongregationId { get; set; }
        public string EmailAddress { get; set; }
        public int GenderId { get; set; }
    }
}