using System;
using System.Collections.Generic;
using SignInCheckIn.Models.DTO;

namespace SignInCheckIn.Services.Interfaces
{
    public interface IChildCheckinService
    {
        List<ParticipantDto> GetChildrenByPhoneNumber(string phoneNumber);
    }
}
