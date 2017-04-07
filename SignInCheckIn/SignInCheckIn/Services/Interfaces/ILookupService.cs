using System.Collections.Generic;
using SignInCheckIn.Models.DTO;

namespace SignInCheckIn.Services.Interfaces
{
    public interface ILookupService
    {
        List<StateDto> GetStates();
        List<CountryDto> GetCountries();
    }
}
