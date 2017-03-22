using System;
using System.Collections.Generic;
using SignInCheckIn.Models.DTO;

namespace SignInCheckIn.Services.Interfaces
{
    public interface ISiteService
    {
        List<CongregationDto> GetAll();
    }
}
