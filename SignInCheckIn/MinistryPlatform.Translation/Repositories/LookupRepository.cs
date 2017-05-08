using System;
using System.Collections.Generic;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;

namespace MinistryPlatform.Translation.Repositories
{
    public class LookupRepository : ILookupRepository
    {
        private readonly IApiUserRepository _apiUserRepository;
        private readonly IMinistryPlatformRestRepository _ministryPlatformRestRepository;

        public LookupRepository(IApiUserRepository apiUserRepository,
                                IMinistryPlatformRestRepository ministryPlatformRestRepository)
        {
            _apiUserRepository = apiUserRepository;
            _ministryPlatformRestRepository = ministryPlatformRestRepository;
        }

        /// <summary>
        /// Gets a list of the states in MP
        /// </summary>
        /// <returns></returns>
        public List<MpStateDto> GetStates()
        {
            var apiUserToken = _apiUserRepository.GetToken();

            var columns = new List<string>
            {
                "States.[State_ID]",
                "States.[State_Name]",
                "States.[State_Abbreviation]",
            };

            return _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken)
                .Search<MpStateDto>("1=1", columns);
        }


        /// <summary>
        /// Gets a list of the states in MP
        /// </summary>
        /// <returns></returns>
        public List<MpCountryDto> GetCountries()
        {
            var apiUserToken = _apiUserRepository.GetToken();

            var columns = new List<string>
            {
                "Countries.[Country_ID]",
                "Countries.[Country]",
                "Countries.[Code3]",
            };

            return _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken)
                .Search<MpCountryDto>("1=1", columns);
        }


        /// <summary>
        /// Gets a list of the Congregations in MP
        /// </summary>
        /// <returns></returns>
        public List<MpCongregationDto> GetCongregations()
        {
            var apiUserToken = _apiUserRepository.GetToken();

            var congregationColumnList = new List<string>
            {
                "Congregation_ID",
                "Congregation_Name"
            };

            var congregations = _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken)
                .Search<MpCongregationDto>($"Available_Online = 1 AND (End_Date IS NULL OR End_Date > '{DateTime.Now:yyyy-MM-dd}')", congregationColumnList);

            return congregations;
        }
    }
}
