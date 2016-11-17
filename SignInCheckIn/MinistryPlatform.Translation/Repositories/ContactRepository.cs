using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;

namespace MinistryPlatform.Translation.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly IApiUserRepository _apiUserRepository;
        private readonly IMinistryPlatformRestRepository _ministryPlatformRestRepository;

        public ContactRepository(IApiUserRepository apiUserRepository, IMinistryPlatformRestRepository ministryPlatformRestRepository)
        {
            _apiUserRepository = apiUserRepository;
            _ministryPlatformRestRepository = ministryPlatformRestRepository;
        }

        public List<MpContactDto> GetHeadsOfHouseholdByHouseholdId(int? householdId)
        {
            var apiUserToken = _apiUserRepository.GetToken();

            var contactColumnList = new List<string>
            {
                "Contact_ID",
                "Household_ID_Table.Household_ID",
                "Household_Position_ID_Table.Household_Position_ID",
                "Household_ID_Table.Home_Phone",
                "Mobile_Phone",
                "Nickname",
                "Last_Name"
            };

            var contacts = _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken)
                .Search<MpContactDto>($"[Household_ID]='{householdId}' AND Household_Position_ID IN (1, 7)", contactColumnList);

            return contacts;
        }
    }
}
