using System;
using System.Collections.Generic;
using System.Linq;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;

namespace MinistryPlatform.Translation.Repositories
{
    public class ChildCheckinRepository : IChildCheckinRepository
    {
        private readonly IApiUserRepository _apiUserRepository;
        private readonly IMinistryPlatformRestRepository _ministryPlatformRestRepository;

        public ChildCheckinRepository(IApiUserRepository apiUserRepository, IMinistryPlatformRestRepository ministryPlatformRestRepository)
        {
            _apiUserRepository = apiUserRepository;
            _ministryPlatformRestRepository = ministryPlatformRestRepository;
        }

        public List<MpParticipantDto> GetChildrenByEventAndRoom(int eventId, int roomId)
        {
            var apiUserToken = _apiUserRepository.GetToken();

            var columnList = new List<string>
            {
                "Event_ID_Table.Event_ID",
                "Room_ID_Table.Room_ID",
                "Event_Participants.Event_Participant_ID",
                "Participant_ID_Table.Participant_ID",
                "Participant_ID_Table_Contact_ID_Table.Contact_ID",
                "Participant_ID_Table_Contact_ID_Table.First_Name",
                "Participant_ID_Table_Contact_ID_Table.Last_Name",
                "Participation_Status_ID_Table.Participation_Status_ID"
            };

            return _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken).
                        SearchTable<MpParticipantDto>("Event_Participants", $"Event_ID_Table.[Event_ID] = {eventId} AND Room_ID_Table.[Room_ID] = {roomId}", columnList);
        }

        public MpEventParticipantDto GetEventParticipantByCallNumber(int eventId, int callNumber)
        {
            var apiUserToken = _apiUserRepository.GetToken();

            var columnList = new List<string>
            {
                "Event_ID_Table.Event_ID",
                "Room_ID_Table.Room_ID",
                "Room_ID_Table.Room_Name",
                "Event_Participants.Event_Participant_ID",
                "Event_Participants.Call_Number",
                "Event_Participants.Time_In",
                "Event_Participants.Time_Confirmed",
                "Event_Participants.Checkin_Phone",
                "Event_Participants.Checkin_Household_ID",
                "Participant_ID_Table.Participant_ID",
                "Participant_ID_Table_Contact_ID_Table.Contact_ID",
                "Participant_ID_Table_Contact_ID_Table.First_Name",
                "Participant_ID_Table_Contact_ID_Table.Last_Name",
                "Participant_ID_Table_Contact_ID_Table.Date_of_Birth",
                "Participant_ID_Table_Contact_ID_Table_Household_ID_Table.Household_ID",
                "Participation_Status_ID_Table.Participation_Status_ID",
                "Group_ID_Table.Group_Name"
            };
            /* TODO
             Age / Grade group assignment
             Head of Household first & last name(s)
             Head of Household phone number(s)
            */

            List<MpEventParticipantDto> participants = _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken).
                        SearchTable<MpEventParticipantDto>("Event_Participants", $"Event_ID_Table.[Event_ID] = {eventId} AND Event_Participants.[Call_Number] = {callNumber}", columnList);
            if (participants.Any())
            {
                return participants.First();
            }

            throw new Exception("No Event Participants by call number: " + callNumber);
        }

        public void CheckinChildrenForCurrentEventAndRoom(int checkinStatusId, int eventParticipantId)
        {
            var apiUserToken = _apiUserRepository.GetToken();
            
            var updateObject = new Dictionary<string, object>
            {
                { "Event_Participant_ID", eventParticipantId },
                { "Participation_Status_ID", checkinStatusId }
            };

            _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken).UpdateRecord("Event_Participants", eventParticipantId, updateObject);
        }

        public List<MpEventParticipantDto> GetChildParticipantsByEvent(int eventId)
        {



            return null;
        }
    }
}
