using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using System.Linq;

namespace MinistryPlatform.Translation.Repositories
{
    public class CheckinRepository : ICheckinRepository
    {
        private readonly IApiUserRepository _apiUserRepository;
        private readonly IMinistryPlatformRestRepository _ministryPlatformRestRepository;

        public CheckinRepository(IApiUserRepository apiUserRepository,
            IMinistryPlatformRestRepository ministryPlatformRestRepository)
        {
            _apiUserRepository = apiUserRepository;
            _ministryPlatformRestRepository = ministryPlatformRestRepository;
        }

        public List<MpParticipantDto> GetChildrenByPhoneNumber(string phoneNumber)
        {
            var householdId = GetHouseholdIdByPhoneNumber(phoneNumber);

            if (householdId == -1) return new List<MpParticipantDto>();
            var children = GetChildParticpantsByPrimaryHousehold(householdId);
            GetChildParticpantsByOtherHousehold(householdId, children);

            return children;
        }

        private int GetHouseholdIdByPhoneNumber(string phoneNumber)
        {
            var apiUserToken = _apiUserRepository.GetToken();

            var columnList = new List<string>
            {
                "Contact_ID",
                "Household_ID_Table.Household_ID",
                "Household_Position_ID_Table.Household_Position_ID",
                "Household_ID_Table.Home_Phone",
                "Mobile_Phone",
            };

            var household = _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken).
                Search<MpContactDto>($"Household_Position_ID_Table.[Household_Position_ID] IN (1,3,4) AND ([Mobile_Phone] = '{phoneNumber}' OR Household_ID_Table.[Home_Phone] = '{phoneNumber}')", columnList);

            return household?.First().HouseholdId ?? -1;
        }

        private List<MpParticipantDto> GetChildParticpantsByPrimaryHousehold(int householdId)
        {
            var apiUserToken = _apiUserRepository.GetToken();

            var columnList = new List<string>
            {
                "Participant_ID",
                "Contact_ID_Table.Contact_ID",
                "Contact_ID_Table_Household_ID_Table.Household_ID",
                "Contact_ID_Table_Household_Position_ID_Table.Household_Position_ID",
                "Contact_ID_Table.First_Name",
                "Contact_ID_Table.Last_Name",
                "Contact_ID_Table.Date_of_Birth",
            };

            return _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken).
                        Search<MpParticipantDto>($"Contact_ID_Table_Household_ID_Table.[Household_ID] = {householdId} AND Contact_ID_Table_Household_Position_ID_Table.[Household_Position_ID] = 2", columnList);
        }

        private void GetChildParticpantsByOtherHousehold(int householdId, List<MpParticipantDto> children)
        {
            var apiUserToken = _apiUserRepository.GetToken();

            var columnList = new List<string>
            {
                "Contact_ID_Table_Participant_Record_Table.Participant_ID",
                "Contact_ID_Table.Contact_ID",
                "Household_ID_Table.Household_ID",
                "Household_Position_ID_Table.Household_Position_ID",
                "Contact_ID_Table.First_Name",
                "Contact_ID_Table.Last_Name",
                "Contact_ID_Table.Date_of_Birth",
            };

            var otherChildren = _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken).
                                    SearchTable<MpParticipantDto>("Contact_Households", $"Household_Position_ID_Table.[Household_Position_ID] = 2  AND Household_ID_Table.[Household_ID] = {householdId}", columnList);

            foreach (var child in otherChildren)
            {
                if (children.Where(x => x.ContactId == child.ContactId).Any())
                {
                    children.Add(child);
                }
            }
        }
    }
}
