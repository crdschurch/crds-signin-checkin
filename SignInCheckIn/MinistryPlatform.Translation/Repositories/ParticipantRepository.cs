using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;

namespace MinistryPlatform.Translation.Repositories
{
    public class ParticipantRepository : IParticipantRepository
    {
        private readonly IApiUserRepository _apiUserRepository;
        private readonly IMinistryPlatformRestRepository _ministryPlatformRestRepository;
        private readonly IContactRepository _contactRepository;

        public ParticipantRepository(IApiUserRepository apiUserRepository, IMinistryPlatformRestRepository ministryPlatformRestRepository, IContactRepository contactRepository)
        {
            _apiUserRepository = apiUserRepository;
            _ministryPlatformRestRepository = ministryPlatformRestRepository;
            _contactRepository = contactRepository;
        }

        // this gets data we won't have with older participants
        public List<MpEventParticipantDto> GetChildParticipantsByEvent(string token, List<int> eventIds)
        {
            var columnList = new List<string>
            {
                "Event_ID_Table.Event_ID",
                "Event_Participant_ID",
                "Participation_Status_ID_Table.Participation_Status_ID",
                "Participant_ID_Table_Contact_ID_Table.First_Name",
                "Participant_ID_Table_Contact_ID_Table.Last_Name",
                "Event_Participants.Call_Number",
                "Room_ID_Table.Room_ID",
                "Room_ID_Table.Room_Name",
                "dp_Created.Date_Time as Time_In",
                "Event_Participants.Checkin_Household_ID",
                "Participant_ID_Table_Contact_ID_Table_Household_ID_Table.Household_ID"
            };

            var childPartipantsForEvent = _ministryPlatformRestRepository.UsingAuthenticationToken(token).
                Search<MpEventParticipantDto>($"Event_ID_Table.Event_ID in ({string.Join(",", eventIds)})", columnList);

            foreach (var child in childPartipantsForEvent)
            {
                if (child.CheckinHouseholdId.HasValue)
                {
                    child.HeadsOfHousehold = _contactRepository.GetHeadsOfHouseholdByHouseholdId(child.CheckinHouseholdId.Value);
                }
            }

            return childPartipantsForEvent;
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
    }
}
