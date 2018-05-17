using System;
using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using System.Linq;
using AutoMapper;
using Crossroads.Utilities.Services.Interfaces;
using Crossroads.Web.Common.MinistryPlatform;

namespace MinistryPlatform.Translation.Repositories
{
    public class ChildSigninRepository : IChildSigninRepository
    {
        private readonly IApiUserRepository _apiUserRepository;
        private readonly IMinistryPlatformRestRepository _ministryPlatformRestRepository;
        private readonly IApplicationConfiguration _applicationConfiguration;
        private readonly IGroupLookupRepository _groupLookupRepository;

        private const string ChildSigninSearchStoredProcName = "api_crds_Child_Signin_Search";
        private const string MSMSigninSearchStoredProcName = "api_crds_Groups_Signin_Search";

        public ChildSigninRepository(IApiUserRepository apiUserRepository,
            IGroupLookupRepository groupLookupRepository,
            IMinistryPlatformRestRepository ministryPlatformRestRepository,
            IApplicationConfiguration applicationConfiguration)
        {
            _apiUserRepository = apiUserRepository;
            _groupLookupRepository = groupLookupRepository;
            _ministryPlatformRestRepository = ministryPlatformRestRepository;
            _applicationConfiguration = applicationConfiguration;
        }

        public MpHouseholdParticipantsDto GetChildrenByPhoneNumber(string phoneNumber, bool includeOtherHousehold = true)
        {
            var parms = new Dictionary<string, object>
            {
                {"Phone_Number", phoneNumber},
                {"Include_Other_Household", includeOtherHousehold},
            };

            var spResults =
                _ministryPlatformRestRepository.UsingAuthenticationToken(_apiUserRepository.GetDefaultApiUserToken()).GetFromStoredProc<MpParticipantDto>(ChildSigninSearchStoredProcName, parms);
            var result = new MpHouseholdParticipantsDto();

            // This check indicates that no household was found
            if (spResults == null || !spResults.Any() || spResults.Count < 2)
            {
                return result;
            }

            // The first result is the household ID for the given phone number
            result.HouseholdId = spResults[0] != null && spResults[0].Any() ? spResults[0].First().HouseholdId : (int?)null;

            // The second result is the list of kids
            result.Participants = spResults[1];

            return result;
        }

        public List<MpParticipantDto> GetChildrenByHouseholdId(int? householdId, int eventId)
        {
            if (householdId == null)
            {
                return new List<MpParticipantDto>();
            }
                
            var children = GetChildParticipantsByPrimaryHousehold(householdId);
            GetChildParticipantsByOtherHousehold(householdId, children);
            var eventGroups = GetEventGroups(eventId);
            children = GetKidsClubAndStudentMinistryChildren(children, eventGroups);

            if (children.Count == 0)
            {
                return new List<MpParticipantDto>();
            }

            return children.Distinct(new MpParticipantDtoComparer()).ToList();
        }

        [Obsolete("This should not be used, and should eventually be removed.  It was only needed when calling GetChildrenByHouseholdId, which is Obsolete.")]
        public int? GetHouseholdIdByPhoneNumber(string phoneNumber)
        {
            var apiUserToken = _apiUserRepository.GetToken();

            // Phone number comes in with dashes, but we need to search for the number without dashes as well
            var phoneNumberWithoutDashes = phoneNumber.Replace("-", "");

            var columnList = new List<string>
            {
                "Contact_ID",
                "Household_ID_Table.Household_ID",
                "Household_Position_ID_Table.Household_Position_ID",
                "Household_ID_Table.Home_Phone",
                "Mobile_Phone",
            };

            var household =
                _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken)
                    .Search<MpContactDto>(
                        $"Household_Position_ID_Table.[Household_Position_ID] IN ({_applicationConfiguration.HouseHoldIdsThatCanCheckIn}) AND ([Mobile_Phone] = '{phoneNumber}' OR [Mobile_Phone] = '{phoneNumberWithoutDashes}' OR Household_ID_Table.[Home_Phone] = '{phoneNumber}' OR Household_ID_Table.[Home_Phone] = '{phoneNumberWithoutDashes}')",
                        columnList);

            if (household == null || !household.Any())
            {
                return null;
            }

            return household.First().HouseholdId;
        }

        private List<MpParticipantDto> GetChildParticipantsByPrimaryHousehold(int? householdId)
        {
            var apiUserToken = _apiUserRepository.GetDefaultApiUserToken();

            var columnList = new List<string>
            {
                "Group_ID_Table.[Group_ID]",
                "Group_ID_Table_Group_Type_ID_Table.[Group_Type_ID]",
                "Participant_ID_Table.[Participant_ID]",
                "Participant_ID_Table_Contact_ID_Table.[Contact_ID]",
                "Participant_ID_Table_Contact_ID_Table_Gender_ID_Table.[Gender_ID]",
                "Participant_ID_Table_Contact_ID_Table_Household_ID_Table.[Household_ID]",
                "Participant_ID_Table_Contact_ID_Table_Household_Position_ID_Table.Household_Position_ID",
                "Participant_ID_Table_Contact_ID_Table.[First_Name]",
                "Participant_ID_Table_Contact_ID_Table.[Last_Name]",
                "Participant_ID_Table_Contact_ID_Table.[Nickname]",
                "Participant_ID_Table_Contact_ID_Table.[Date_of_Birth]"
            };

            var groupParticipants = _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken).
                        Search<MpGroupParticipantDto>($"Participant_ID_Table_Contact_ID_Table_Household_ID_Table.[Household_ID] = {householdId} AND Participant_ID_Table_Contact_ID_Table_Household_Position_ID_Table.[Household_Position_ID] = {_applicationConfiguration.MinorChildId} and Group_ID_Table_Group_Type_ID_Table.[Group_Type_ID] = {_applicationConfiguration.KidsClubGroupTypeId} AND Group_Participants.[End_Date] IS NULL AND Participant_ID_Table_Contact_ID_Table.[Contact_Status_ID] = 1", columnList).ToList();

            foreach (var g in groupParticipants)
            {
                if (g.GroupId != null) { 
                    g.YearGrade = _groupLookupRepository.GetGradeAttributeId(g.GroupId.Value);
                }
            }
            return Mapper.Map<List<MpGroupParticipantDto>, List<MpParticipantDto>>(groupParticipants);
        }

        private void GetChildParticipantsByOtherHousehold(int? householdId, List<MpParticipantDto> children)
        {
            var apiUserToken = _apiUserRepository.GetDefaultApiUserToken();

            var columnList = new List<string>
            {
                "Contact_ID_Table_Participant_Record_Table.Participant_ID",
                "Contact_ID_Table.Contact_ID",
                "Household_ID_Table.Household_ID",
                "Household_Position_ID_Table.Household_Position_ID",
                "Contact_ID_Table.First_Name",
                "Contact_ID_Table.Last_Name",
                "Contact_ID_Table.Nickname",
                "Contact_ID_Table.Date_of_Birth"
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

        private List<MpEventGroupDto> GetEventGroups(int eventId)
        {
            var apiUserToken = _apiUserRepository.GetDefaultApiUserToken();

            var columnList = new List<string>
            {
                "Event_Group_ID",
                "Event_ID_Table.Event_ID",
                "Group_ID_Table.Group_ID"
            };

            return _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken).
                    Search<MpEventGroupDto>($"Event_ID_Table.[Event_ID] = {eventId}", columnList);
            
        }

        private List<MpParticipantDto> GetKidsClubAndStudentMinistryChildren(List<MpParticipantDto> children, List<MpEventGroupDto> eventGroups)
        {
            if (children.Count == 0) return new List<MpParticipantDto>();
            var apiUserToken = _apiUserRepository.GetDefaultApiUserToken();

            var columnList = new List<string>
            {
                "Participant_ID_Table.Participant_ID",
                "Participant_ID_Table_Contact_ID_Table.Contact_ID",
                "Participant_ID_Table_Contact_ID_Table_Household_ID_Table.Household_ID",
                "Participant_ID_Table_Contact_ID_Table_Household_Position_ID_Table.Household_Position_ID",
                "Participant_ID_Table_Contact_ID_Table.First_Name",
                "Participant_ID_Table_Contact_ID_Table.Last_Name",
                "Participant_ID_Table_Contact_ID_Table.Nickname",
                "Participant_ID_Table_Contact_ID_Table.Date_of_Birth",
                "Group_ID_Table_Congregation_ID_Table.Congregation_ID",
                "Group_ID_Table_Group_Type_ID_Table.Group_Type_ID",
                "Group_ID_Table_Ministry_ID_Table.Ministry_ID",
                "Group_ID_Table.Group_ID"
            };

            var participantIds = string.Join(",", children.Select(x => x.ParticipantId));
            var participants = _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken).
                        SearchTable<MpParticipantDto>("Group_Participants", $"Participant_ID_Table.[Participant_ID] IN ({participantIds}) AND Group_ID_Table_Congregation_ID_Table.[Congregation_ID] = {_applicationConfiguration.KidsClubCongregationId} AND Group_ID_Table_Group_Type_ID_Table.[Group_Type_ID] = {_applicationConfiguration.KidsClubGroupTypeId} AND Group_ID_Table_Ministry_ID_Table.[Ministry_ID] IN ({_applicationConfiguration.KidsClubMinistryId}, {_applicationConfiguration.StudentMinistryId}) AND Group_Participants.[End_Date] IS NULL", columnList);

            children.ForEach(child =>
            {
                // Assign the KC Group ID on the child if they have a participant with a Kids Club group
                child.GroupId = participants.Find(p => p.ParticipantId == child.ParticipantId)?.GroupId;
            });

            return children;
        }

        private class MpParticipantDtoComparer : IEqualityComparer<MpParticipantDto>
        {
            // Consider them equal if participant id and contact id are the same
            public bool Equals(MpParticipantDto x, MpParticipantDto y)
            {
                return x.ParticipantId == y.ParticipantId && x.ContactId == y.ContactId;
            }

            // Hash code is a hash of participant id and contact id
            public int GetHashCode(MpParticipantDto obj)
            {
                return $"{obj.ParticipantId}{obj.ContactId}".GetHashCode();
            }
        }

	    public List<MpEventParticipantDto> CreateEventParticipants(List<MpEventParticipantDto> mpEventParticipantDtos)
        {
	        if (mpEventParticipantDtos == null || !mpEventParticipantDtos.Any())
	        {
	            return new List<MpEventParticipantDto>();
	        }

            var token = _apiUserRepository.GetDefaultApiUserToken();

            var columnList = new List<string>
            {
                "Event_Participant_ID",
                "Event_ID",
                "Participant_ID_Table_Contact_ID_Table.[First_Name]",
                "Participant_ID_Table_Contact_ID_Table.[Last_Name]",
                "Participant_ID_Table_Contact_ID_Table.[Nickname]",
                "Participant_ID_Table.[Participant_ID]",
                "Participation_Status_ID",
                "Time_In",
                "Time_Confirmed",
                "Time_Out",
                "Event_Participants.[Notes]",
                "Group_Participant_ID",
                "[Check-in_Station]",
                "Group_ID",
                "Room_ID_Table.[Room_ID]",
                "Room_ID_Table.[Room_Name]",
                "Call_Parents",
                "Group_Role_ID",
                "Response_ID",
                "Opportunity_ID",
                "Registrant_Message_Sent",
                "Call_Number",
                "Checkin_Phone",
                "Checkin_Household_ID",
                "Guest_Sign_In"
            };

            var participants = _ministryPlatformRestRepository.UsingAuthenticationToken(token).Create(mpEventParticipantDtos, columnList);
	        return participants;
        }

        public MpHouseholdParticipantsDto GetChildrenByPhoneNumberAndGroupIds(string phoneNumber, List<int> groupIds, bool includeOtherHousehold = true)
        {
            var parms = new Dictionary<string, object>
            {
                {"Phone_Number", phoneNumber},
                {"GroupIds", string.Join(",", groupIds.Select(x => x)) },
                {"Include_Other_Household", includeOtherHousehold}
            };

            var spResults =
                _ministryPlatformRestRepository.UsingAuthenticationToken(_apiUserRepository.GetDefaultApiUserToken()).GetFromStoredProc<MpParticipantDto>(MSMSigninSearchStoredProcName, parms);
            var result = new MpHouseholdParticipantsDto();

            // This check indicates that no household was found
            if (spResults == null || !spResults.Any() || spResults.Count < 2)
            {
                return result;
            }

            // The first result is the household ID for the given phone number
            result.HouseholdId = spResults[0] != null && spResults[0].Any() ? spResults[0].First().HouseholdId : (int?)null;

            // The second result is the list of kids
            result.Participants = spResults[1];

            return result;
        }
    }
}
