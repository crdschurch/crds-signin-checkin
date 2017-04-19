using System.Collections.Generic;
using AutoMapper;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Services.Interfaces;

namespace SignInCheckIn.Services
{
    public class LookupService : ILookupService
    {
        private readonly ILookupRepository _lookupRepository;

        public LookupService(ILookupRepository lookupRepository)
        {
            _lookupRepository = lookupRepository;
        }

        public List<StateDto> GetStates()
        {
            var states = _lookupRepository.GetStates();
            return Mapper.Map<List<MpStateDto>, List<StateDto>>(states);
        }

        public List<CountryDto> GetCountries()
        {
            var countries = _lookupRepository.GetCountries();
            return Mapper.Map<List<MpCountryDto>, List<CountryDto>>(countries);
        }
    }
}