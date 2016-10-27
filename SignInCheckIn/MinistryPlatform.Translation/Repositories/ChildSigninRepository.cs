using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using System.Linq;
using Crossroads.Utilities.Services.Interfaces;

namespace MinistryPlatform.Translation.Repositories
{
    public class ChildSigninRepository : IChildSigninRepository
    {
        private readonly IApiUserRepository _apiUserRepository;
        private readonly IMinistryPlatformRestRepository _ministryPlatformRestRepository;
        private readonly IApplicationConfiguration _applicationConfiguration;

        public ChildSigninRepository(IApiUserRepository apiUserRepository,
            IMinistryPlatformRestRepository ministryPlatformRestRepository,
            IApplicationConfiguration applicationConfiguration)
        {
            _apiUserRepository = apiUserRepository;
            _ministryPlatformRestRepository = ministryPlatformRestRepository;
            _applicationConfiguration = applicationConfiguration;
        }

        public List<MpParticipantDto> GetChildrenByPhoneNumber(string phoneNumber)
        {
            var householdId = GetHouseholdIdByPhoneNumber(phoneNumber);

            if (householdId == -1) return new List<MpParticipantDto>();
            var children = GetChildParticpantsByPrimaryHousehold(householdId);
            GetChildParticpantsByOtherHousehold(householdId, children);
            children = GetOnlyKidsClubChildren(children);

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
                Search<MpContactDto>($"Household_Position_ID_Table.[Household_Position_ID] IN ({_applicationConfiguration.HouseHoldIdsThatCanCheckIn}) AND ([Mobile_Phone] = '{phoneNumber}' OR Household_ID_Table.[Home_Phone] = '{phoneNumber}')", columnList);

            if (household == null || !household.Any())
            {
                return -1;
            }

            return household.First().HouseholdId;
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
                        Search<MpParticipantDto>($"Contact_ID_Table_Household_ID_Table.[Household_ID] = {householdId} AND Contact_ID_Table_Household_Position_ID_Table.[Household_Position_ID] = {_applicationConfiguration.MinorChildId}", columnList);
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
                                    SearchTable<MpParticipantDto>("Contact_Households", $"Household_Position_ID_Table.[Household_Position_ID] = {_applicationConfiguration.MinorChildId}  AND Household_ID_Table.[Household_ID] = {householdId}", columnList);

            foreach (var child in otherChildren)
            {
                if (!children.Exists(x => x.ContactId == child.ContactId))
                {
                    children.Add(child);
                }
            }
        }

        private List<MpParticipantDto> GetOnlyKidsClubChildren(List<MpParticipantDto> children)
        {
            var apiUserToken = _apiUserRepository.GetToken();

            var columnList = new List<string>
            {
                "Participant_ID_Table.Participant_ID",
                "Participant_ID_Table_Contact_ID_Table.Contact_ID",
                "Participant_ID_Table_Contact_ID_Table_Household_ID_Table.Household_ID",
                "Participant_ID_Table_Contact_ID_Table_Household_Position_ID_Table.Household_Position_ID",
                "Participant_ID_Table_Contact_ID_Table.First_Name",
                "Participant_ID_Table_Contact_ID_Table.Last_Name",
                "Participant_ID_Table_Contact_ID_Table.Date_of_Birth",
                "Group_ID_Table_Congregation_ID_Table.Congregation_ID",
                "Group_ID_Table_Group_Type_ID_Table.Group_Type_ID",
                "Group_ID_Table_Ministry_ID_Table.Ministry_ID"
            };

            var participantIds = string.Join(",", children.Select(x => x.ParticipantId));

            return _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken).
                        SearchTable<MpParticipantDto>("Group_Participants", $"Participant_ID_Table.[Participant_ID] IN ({participantIds}) AND Group_ID_Table_Congregation_ID_Table.[Congregation_ID] = {_applicationConfiguration.KidsClubCongregationId} AND Group_ID_Table_Group_Type_ID_Table.[Group_Type_ID] = {_applicationConfiguration.KidsClubGroupTypeId} AND Group_ID_Table_Ministry_ID_Table.[Ministry_ID] = {_applicationConfiguration.KidsClubMinistryId}", columnList);
        }
    }
}
