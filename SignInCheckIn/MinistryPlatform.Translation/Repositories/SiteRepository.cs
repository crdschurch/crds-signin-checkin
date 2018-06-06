using System;
using System.Collections.Generic;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;

namespace MinistryPlatform.Translation.Repositories
{
    public class SiteRepository : ISiteRepository
    {
        private readonly IApiUserRepository _apiUserRepository;
        private readonly IMinistryPlatformRestRepository _ministryPlatformRestRepository;

        public SiteRepository(IApiUserRepository apiUserRepository,
            IMinistryPlatformRestRepository ministryPlatformRestRepository)
        {
            _apiUserRepository = apiUserRepository;
            _ministryPlatformRestRepository = ministryPlatformRestRepository;   
        }

        public List<MpCongregationDto> GetAll()
        {
            try
            {
                var apiUserToken = _apiUserRepository.GetDefaultApiClientToken();

                var congregationColumnList = new List<string>
                {
                    "Congregation_ID",
                    "Congregation_Name"
                };

                var congregations = _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken)
                    .Search<MpCongregationDto>($"Available_Online = 1 AND (End_Date IS NULL OR End_Date > '{DateTime.Now:yyyy-MM-dd}')", congregationColumnList);

                return congregations;
            }
            catch (Exception ex)
            {
                return null;
            }
            

            
        }
    }
}
