using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;

namespace MinistryPlatform.Translation.Repositories.Interfaces
{
    public interface ILookupRepository
    {
        List<MpStateDto> GetStates();
        List<MpCountryDto> GetCountries();
        List<MpCongregationDto> GetCongregations();
    }
}