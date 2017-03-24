using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Newtonsoft.Json.Linq;

namespace MinistryPlatform.Translation.Repositories
{
    public class ParticipantRepository : IParticipantRepository
    {
        private readonly IApiUserRepository _apiUserRepository;
        private readonly IMinistryPlatformRestRepository _ministryPlatformRestRepository;
        private readonly IContactRepository _contactRepository;
        private List<string> _eventParticipantColumns; 

        public ParticipantRepository(IApiUserRepository apiUserRepository, IMinistryPlatformRestRepository ministryPlatformRestRepository, IContactRepository contactRepository)
        {
            _apiUserRepository = apiUserRepository;
            _ministryPlatformRestRepository = ministryPlatformRestRepository;
            _contactRepository = contactRepository;

            _eventParticipantColumns = new List<string>
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
                "Checkin_Household_ID"
            };
        }

        // this gets data we won't have with older participants
        public List<MpEventParticipantDto> GetChildParticipantsByEvent(string token, int eventId, string search = null)
        {
            var parameters = new Dictionary<string, object>
            {
                {"EventId", eventId}
            };

            if (search != null)
            {
                parameters.Add("Search", search);
            }

            var results = _ministryPlatformRestRepository.UsingAuthenticationToken(token).GetFromStoredProc<JObject>("api_crds_Get_Manage_Children_data", parameters);

            // This check indicates that no household was found
            if (results == null || !results.Any() || results.Count < 2)
            {
                return new List<MpEventParticipantDto>();
            }

            var children = results[0].Select(r => r.ToObject<MpEventParticipantDto>()).ToList();
            var headHouseholds = results[1].Select(r => r.ToObject<MpContactDto>()).ToList();

            foreach (var child in children)
            {
                child.HeadsOfHousehold = headHouseholds.Where(hoh => hoh.HouseholdId == child.CheckinHouseholdId).ToList();
            }

            return children;
        }

        public MpNewParticipantDto CreateParticipantWithContact(string authenticationToken, MpNewParticipantDto mpNewParticipantDto)
        {
            var token = authenticationToken ?? _apiUserRepository.GetToken();

            List<string> participantColumns = new List<string>
            {
                "Participants.Participant_ID",
                "Participants.Participant_Type_ID",
                "Participants.Participant_Start_Date"
            };

            return _ministryPlatformRestRepository.UsingAuthenticationToken(token).Create(mpNewParticipantDto, participantColumns);
        }

        public List<MpGroupParticipantDto> CreateGroupParticipants(string authenticationToken, List<MpGroupParticipantDto> mpGroupParticipantDtos)
        {
            var token = authenticationToken ?? _apiUserRepository.GetToken();

            List<string> groupParticipantColumns = new List<string>
            {
                "Group_Participant_ID",
                "Group_ID",
                "Participant_ID",
                "Group_Role_ID",
                "Start_Date",
                "Employee_Role",
                "Auto_Promote"
            };

            return _ministryPlatformRestRepository.UsingAuthenticationToken(token).Create(mpGroupParticipantDtos, groupParticipantColumns);
        }

        public void UpdateEventParticipants(List<MpEventParticipantDto> mpEventParticipantDtos)
        {
            var apiUserToken = _apiUserRepository.GetToken();

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
                "Checkin_Household_ID"
            };

            _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken).Update<MpEventParticipantDto>(mpEventParticipantDtos, columnList);
        }

        public MpEventParticipantDto GetEventParticipantByEventParticipantId(string token, int eventParticipantId)
        {
            return _ministryPlatformRestRepository.UsingAuthenticationToken(token).Get<MpEventParticipantDto>(eventParticipantId, _eventParticipantColumns);
        }

        // this returns only "valid" participants in the system - not ones that could not get in, or were reversed
        public List<MpEventParticipantDto> GetEventParticipantsByEventAndParticipant(int eventId, List<int> participantIds)
        {
            var apiUserToken = _apiUserRepository.GetToken();

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
                "Checkin_Household_ID"
            };

            var eventParticipantsForEvent = _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken).
                Search<MpEventParticipantDto>($"Event_Participants.Event_ID = {eventId} AND Event_Participants.Participant_ID in " +
                                              $"({string.Join(",", participantIds)}) AND End_Date IS NULL AND Participation_Status_ID IN (2, 3, 4)", columnList);

            return eventParticipantsForEvent;
        }

        public List<MpGroupParticipantDto> GetGroupParticipantsByParticipantAndGroupId(int groupId, List<int> participantIds)
        {
            var apiUserToken = _apiUserRepository.GetToken();

            List<string> groupParticipantColumns = new List<string>
            {
                "Group_Participant_ID",
                "Group_ID",
                "Participant_ID",
                "Group_Role_ID",
                "Start_Date",
                "Employee_Role",
                "Auto_Promote"
            };

            var mpGroupParticipantDtos = _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken).
                 Search<MpGroupParticipantDto>(
                    $"Group_Participants.Participant_ID IN ({string.Join(",", participantIds)}) AND Group_Participants.Group_ID = ({groupId}) " +
                    $"AND End_Date IS NULL AND Participation_Status_ID IN (2, 3, 4)", groupParticipantColumns);

            return mpGroupParticipantDtos;
        }
    }
}
