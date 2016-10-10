using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MinistryPlatform.Translation.Models.DTO;

namespace SignInCheckIn.Models.Authentication
{
    public class LoginReturn
    {
        public LoginReturn() { }
        public LoginReturn(string userToken, int userId, string username, string userEmail, List<MpRoleDto> roles)
        {
            this.userId = userId;
            this.userToken = userToken;
            this.username = username;
            this.userEmail = userEmail;
            this.roles = roles;
        }
        public string userToken { get; set; }
        public string userTokenExp { get; set; }
        public string refreshToken { get; set; }
        public int userId { get; set; }
        public string username { get; set; }
        public string userEmail { get; set; }
        public List<MpRoleDto> roles { get; set; }
        public int age { get; set; }
    }
}