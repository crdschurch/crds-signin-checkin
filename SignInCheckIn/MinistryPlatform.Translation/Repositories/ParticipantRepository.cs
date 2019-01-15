using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

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
                "Checkin_Household_ID",
                "Guest_Sign_In"
            };
        }

        // this gets data we won't have with older participants
        public List<MpEventParticipantDto> GetChildParticipantsByEvent(int eventId, string search = null)
        {
            var token = _apiUserRepository.GetApiClientToken("CRDS.Service.SignCheckIn");
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

        public MpNewParticipantDto CreateParticipantWithContact(MpNewParticipantDto mpNewParticipantDto, string userToken = null)
        {
            var token = userToken ?? _apiUserRepository.GetApiClientToken("CRDS.Service.SignCheckIn");

            List<string> participantColumns = new List<string>
            {
                "Participants.Participant_ID",
                "Participants.Participant_Type_ID",
                "Participants.Participant_Start_Date",
                "Contact_ID_Table.[Contact_ID] AS [Participant_Contact_ID]"
            };

            return _ministryPlatformRestRepository.UsingAuthenticationToken(token).Create(mpNewParticipantDto, participantColumns);
        }

        public List<MpGroupParticipantDto> CreateGroupParticipants(List<MpGroupParticipantDto> mpGroupParticipantDtos)
        {            
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

            return _ministryPlatformRestRepository.UsingAuthenticationToken(_apiUserRepository.GetDefaultApiClientToken()).Create(mpGroupParticipantDtos, groupParticipantColumns);
        }

        public void DeleteGroupParticipants(List<MpGroupParticipantDto> groupParticipants)
        {
            _ministryPlatformRestRepository.UsingAuthenticationToken(_apiUserRepository.GetDefaultApiClientToken()).Delete<MpGroupParticipantDto>(groupParticipants.Select(gp => gp.GroupParticipantId));
        }

        public void UpdateEventParticipants(List<MpEventParticipantDto> mpEventParticipantDtos)
        {
            var apiUserToken = _apiUserRepository.GetApiClientToken("CRDS.Service.SignCheckIn");

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

            _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken).Update<MpEventParticipantDto>(mpEventParticipantDtos, columnList);
        }

        public MpEventParticipantDto GetEventParticipantByEventParticipantId(int eventParticipantId)
        {
            var token = _apiUserRepository.GetApiClientToken("CRDS.Service.SignCheckIn");
            return _ministryPlatformRestRepository.UsingAuthenticationToken(token).Get<MpEventParticipantDto>(eventParticipantId, _eventParticipantColumns);
        }

        // this returns only "valid" participants in the system - not ones that could not get in, or were reversed
        public List<MpEventParticipantDto> GetEventParticipantsByEventAndParticipant(int eventId, List<int> participantIds)
        {
            var apiUserToken = _apiUserRepository.GetApiClientToken("CRDS.Service.SignCheckIn");

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

            var eventParticipantsForEvent = _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken).
                Search<MpEventParticipantDto>($"Event_Participants.Event_ID = {eventId} AND Event_Participants.Participant_ID in " +
                                              $"({string.Join(",", participantIds)}) AND End_Date IS NULL AND Participation_Status_ID IN (3, 4)", columnList);

            return eventParticipantsForEvent;
        }

        public List<MpGroupParticipantDto> GetGroupParticipantsByParticipantAndGroupId(int groupId, List<int> participantIds)
        {
            var apiUserToken = _apiUserRepository.GetApiClientToken("CRDS.Service.SignCheckIn");

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
                    "AND End_Date IS NULL", groupParticipantColumns);

            return mpGroupParticipantDtos;
        }

        public List<MpGroupParticipantDto> GetGroupParticipantsByParticipantId(int participantId)
        {
            var apiUserToken = _apiUserRepository.GetApiClientToken("CRDS.Service.SignCheckIn");

            List<string> groupParticipantColumns = new List<string>
            {
                "Group_Participant_ID",
                "Group_Participants.Group_ID",
                "Group_ID_Table.[Group_Type_ID]",
                "Participant_ID",
                "Group_Role_ID",
                "Group_Participants.[Start_Date]",
                "Employee_Role",
                "Auto_Promote"
            };

            var mpGroupParticipantDtos = _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken).
                 Search<MpGroupParticipantDto>(
                    $"Group_Participants.Participant_ID = {participantId}" +
                    "AND Group_Participants.End_Date IS NULL", groupParticipantColumns);

            return mpGroupParticipantDtos;
        }

        public List<MpContactDto> GetFamiliesForSearch(string search)
        {
            var token = _apiUserRepository.GetApiClientToken("CRDS.Service.SignCheckIn");
            var columns = new List<string>
            {
                "Contacts.Contact_ID",
                "Contacts.Nickname",
                "Contacts.First_Name",
                "Contacts.Last_Name",
                "Household_ID_Table.Household_ID",
                "Household_ID_Table_Address_ID_Table.Address_Line_1",
                "Household_ID_Table_Address_ID_Table.City",
                "Household_ID_Table_Address_ID_Table.[State/Region] as State",
                "Household_ID_Table_Address_ID_Table.Postal_Code",
                "Household_ID_Table.Home_Phone",
                "Contacts.Mobile_Phone",
                "Household_ID_Table_Congregation_ID_Table.Congregation_Name"
            };

            var contacts = _ministryPlatformRestRepository.UsingAuthenticationToken(token).
                 Search<MpContactDto>($"(Contacts.[Display_Name] LIKE '%{search}%' OR Contacts.[Email_Address] = '{search}' OR Household_ID_Table.Home_Phone = '{search}' OR Contacts.[Mobile_Phone] = '{search}') AND Household_ID_Table.[Household_ID] IS NOT NULL AND Household_Position_ID_Table.[Household_Position_ID] IN (1,7)", columns, "Contacts.Last_Name ASC, Contacts.Nickname ASC");

            return contacts;
        }

        public MpHouseholdDto GetHouseholdByHouseholdId(int householdID)
        {
            var token = _apiUserRepository.GetApiClientToken("CRDS.Service.SignCheckIn");
            var columns = new List<string>
            {
                "Households.[Household_ID]",
                "Households.[Household_Name]",
                "Household_Source_ID_Table.[Household_Source_ID]",
                "Congregation_ID_Table.[Congregation_ID]",
                "Address_ID_Table.[Address_ID]",
                "Address_ID_Table.[Address_Line_1]",
                "Address_ID_Table.[Address_Line_2]",
                "Address_ID_Table.[City]",
                "Address_ID_Table.[State/Region] as State",
                "Address_ID_Table.[Postal_Code]",
                "Address_ID_Table.[County]",
                "Address_ID_Table.[Country_Code]",
                "Households.[Home_Phone]"
            };

            var household = _ministryPlatformRestRepository.UsingAuthenticationToken(token).
                 Get<MpHouseholdDto>(householdID, columns);

            return household;
        }

        public void UpdateHouseholdInformation(MpHouseholdDto householdDto)
        {
            var token = _apiUserRepository.GetApiClientToken("CRDS.Service.SignCheckIn");
            var householdIdColumns = new List<string>
            {
                "Households.[Household_ID]"
            };

            var addressIdColumns = new List<string>
            {
                "Addresses.[Address_ID]"
            };

            var address = new MpAddressDto
            {
                AddressLine1 = householdDto.AddressLine1,
                AddressLine2 = householdDto.AddressLine2,
                City = householdDto.City,
                State = householdDto.State,
                ZipCode = householdDto.ZipCode,
                County = householdDto.County,
                CountryCode = householdDto.CountryCode,
            };

            if (householdDto.AddressId == null)
            {
                // new address
                var result = _ministryPlatformRestRepository.UsingAuthenticationToken(token).Create<MpAddressDto>(address, addressIdColumns);
                householdDto.AddressId = result.AddressId;
            }
            else
            {
                // existing address
                address.AddressId = householdDto.AddressId.Value;
                _ministryPlatformRestRepository.UsingAuthenticationToken(token).Update<MpAddressDto>(address, addressIdColumns);
            }

            _ministryPlatformRestRepository.UsingAuthenticationToken(token).Update<MpHouseholdDto>(householdDto, householdIdColumns);
        }
    }
}
