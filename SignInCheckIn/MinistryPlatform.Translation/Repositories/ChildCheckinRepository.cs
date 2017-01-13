using System;
using System.Collections.Generic;
using System.Linq;
using Crossroads.Utilities.Services.Interfaces;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;

namespace MinistryPlatform.Translation.Repositories
{
    public class ChildCheckinRepository : IChildCheckinRepository
    {
        private readonly IApiUserRepository _apiUserRepository;
        private readonly IMinistryPlatformRestRepository _ministryPlatformRestRepository;
        private readonly IApplicationConfiguration _applicationConfiguration;

        public ChildCheckinRepository(IApiUserRepository apiUserRepository, IMinistryPlatformRestRepository ministryPlatformRestRepository, IApplicationConfiguration applicationConfiguration)
        {
            _apiUserRepository = apiUserRepository;
            _ministryPlatformRestRepository = ministryPlatformRestRepository;
            _applicationConfiguration = applicationConfiguration;
        }

        public List<MpParticipantDto> GetChildrenByEventAndRoom(int eventId, int roomId)
        {
            var apiUserToken = _apiUserRepository.GetToken();

            var columnList = new List<string>
            {
                "Event_ID_Table.Event_ID",
                "Room_ID_Table.Room_ID",
                "Event_Participants.Call_Number",
                "Event_Participants.Event_Participant_ID",
                "Participant_ID_Table.Participant_ID",
                "Participant_ID_Table_Contact_ID_Table.Contact_ID",
                "Participant_ID_Table_Contact_ID_Table.First_Name",
                "Participant_ID_Table_Contact_ID_Table.Last_Name",
                "Participation_Status_ID_Table.Participation_Status_ID"
            };

            return _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken).
                        SearchTable<MpParticipantDto>("Event_Participants", $"Event_ID_Table.[Event_ID] = {eventId} AND Room_ID_Table.[Room_ID] = {roomId} AND Event_Participants.End_Date IS NULL", columnList);
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

            List<MpEventParticipantDto> participants = _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken).
                        SearchTable<MpEventParticipantDto>("Event_Participants", $"Event_ID_Table.[Event_ID] = {eventId} AND Event_Participants.[Call_Number] = {callNumber}", columnList);
            if (participants.Any())
            {
                return participants.First();
            }
            return null;
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

        public void OverrideChildIntoRoom(int eventParticipantId, int roomId)
        {
            var apiUserToken = _apiUserRepository.GetToken();

            var updateObject = new Dictionary<string, object>
            {
                { "Event_Participant_ID", eventParticipantId },
                { "Participation_Status_ID", _applicationConfiguration.CheckedInParticipationStatusId },
                { "Room_ID", roomId }
            };

            _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken).UpdateRecord("Event_Participants", eventParticipantId, updateObject);
        }

        public List<MpEventParticipantDto> GetChildParticipantsByEvent(int eventId)
        {



            return null;
        }
    }
}
