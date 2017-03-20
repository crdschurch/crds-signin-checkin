using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;
using Newtonsoft.Json.Linq;

namespace MinistryPlatform.Translation.Repositories.Interfaces
{
    public interface ISiteRepository
    {
        List<MpCongregationDto> GetAll();
    }
}
