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

        public ParticipantRepository(IApiUserRepository apiUserRepository, IMinistryPlatformRestRepository ministryPlatformRestRepository)
        {
            _apiUserRepository = apiUserRepository;
            _ministryPlatformRestRepository = ministryPlatformRestRepository;
        }

        // this gets data we won't have with older participants
        public List<MpEventParticipantDto> GetChildParticipantsByEvent(int eventId)
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
                        SearchTable<MpEventParticipantDto>("Event_Participants", $"Event_ID_Table.[Event_ID] = {eventId}", columnList);
        }

        public List<MpContactDto> GetHeadsOfHouseholdByHouseholdId(int householdId)
        {
            var apiUserToken = _apiUserRepository.GetToken();

            var contactColumnList = new List<string>
            {
                "Contact_ID",
                "Contacts.Household_ID",
                "Contacts.Household_Position_ID",
                "Household_ID_Table.Home_Phone",
                "Mobile_Phone",
                "Nickname",
                "Last_Name"
            };

            var contacts = _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken)
                .Search<MpContactDto>($"Contacts.Household_ID={householdId} AND Contacts.Household_Position_ID IN (1, 7)", contactColumnList);

            return contacts;
        }

        public List<MpNewParticipantDto> CreateParticipantsWithContacts(string token, List<MpNewParticipantDto> mpNewParticipantDtos)
        {
            List<string> participantColumns = new List<string>
            {
                "Participants.Participant_ID",
                "Participants.Participant_Type_ID",
                "Participants.Participant_Start_Date"
            };

            return _ministryPlatformRestRepository.UsingAuthenticationToken(token).Create(mpNewParticipantDtos, participantColumns);
        }
    }
}
