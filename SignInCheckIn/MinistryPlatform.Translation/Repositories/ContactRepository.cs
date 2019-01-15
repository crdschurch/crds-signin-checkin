using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;

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

        public List<MpContactDto> GetHeadsOfHouseholdByHouseholdId(int householdId)
        {
            var apiUserToken = _apiUserRepository.GetApiClientToken("CRDS.Service.SignCheckIn");

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
                .Search<MpContactDto>($"Contacts.Household_ID={householdId} AND Contacts.Household_Position_ID IN (1, 7) AND Contacts.Contact_Status_ID = 1", contactColumnList);

            return contacts;
        }

        public MpHouseholdDto CreateHousehold(MpHouseholdDto mpHouseholdDto)
        {
            List<string> householdColumns = new List<string>
            {
                "Households.Household_ID",
                "Households.Home_Phone",
                "Households.Household_Name"
            };

           return _ministryPlatformRestRepository.UsingAuthenticationToken(_apiUserRepository.GetDefaultApiClientToken()).Create(mpHouseholdDto, householdColumns);
        }

        public MpContactDto Update(MpContactDto contactDto)
        {
            List<string> columnList = new List<string>
            {
                "Contacts.[Last_Name]",
                "Contacts.[Nickname]",
                "Contacts.[Date_of_Birth]",
                "Gender_ID_Table.[Gender_ID]"
            };

            return _ministryPlatformRestRepository.UsingAuthenticationToken(_apiUserRepository.GetDefaultApiClientToken()).Update<MpContactDto>(contactDto, columnList);
        }

        public void CreateContactRelationships(List<MpContactRelationshipDto> contactRelationshipDtos)
         {
             List<string> columnList = new List<string>
             {
                 "Contact_ID",
                 "Related_Contact_ID",
                 "Relationship_ID",
                 "Start_Date"
             };
 
             _ministryPlatformRestRepository.UsingAuthenticationToken(_apiUserRepository.GetDefaultApiClientToken()).Create(contactRelationshipDtos, columnList);
         }

        public MpContactDto GetContactById(int contactId)
        {
            var contactColumnList = new List<string>
            {
                "Contact_ID",
                "Contacts.Household_ID",
                "Contacts.Household_Position_ID",
                "Household_ID_Table.Home_Phone",
                "Contacts.Household_ID",
                "Mobile_Phone",
                "Nickname",
                "Last_Name",
                "Email_Address",
                "Gender_ID"
            };

            var contact = _ministryPlatformRestRepository.UsingAuthenticationToken(_apiUserRepository.GetDefaultApiClientToken())
                .Search<MpContactDto>($"Contacts.Contact_ID={contactId}", contactColumnList).FirstOrDefault();

            return contact;
        }

        public MpUserDto CreateUserRecord(MpUserDto mpUserDto)
        {
            var columnList = new List<string>
            {
                "User_ID",
                "User_Email",
                "Password",
                "Display_Name",
                "Domain_ID",
                "User_Name",
                "Contact_ID",
                "PasswordResetToken"
            };

            return _ministryPlatformRestRepository.UsingAuthenticationToken(_apiUserRepository.GetDefaultApiClientToken()).Create(mpUserDto, columnList);
        }

        public void CreateUserRoles(List<MpUserRoleDto> mpUserRoleDtos)
        {
            var columnList = new List<string>
            {
                "User_ID",
                "Role_ID"
            };

            _ministryPlatformRestRepository.UsingAuthenticationToken(_apiUserRepository.GetDefaultApiClientToken()).Create(mpUserRoleDtos, columnList);
        }

        public void CreateContactPublications(List<MpContactPublicationDto> contactPublicationDtos)
        {
            var columnList = new List<string>
            {
                "Contact_ID",
                "Publication_ID",
                "Unsubscribed"
            };

            _ministryPlatformRestRepository.UsingAuthenticationToken(_apiUserRepository.GetDefaultApiClientToken()).Create(contactPublicationDtos, columnList);
        }

        public List<MpUserDto> GetUserByEmailAddress(string emailAddress)
        {
            var columnList = new List<string>
            {
                "User_ID",
                "User_Email",
                "Password",
                "dp_Users.Display_Name",
                "dp_Users.Domain_ID",
                "dp_Users.User_Name",
                "dp_Users.Contact_ID",
                "PasswordResetToken",
                "Contact_ID_Table_Household_ID_Table.[Household_ID]"
            };

            var users = _ministryPlatformRestRepository.UsingAuthenticationToken(_apiUserRepository.GetDefaultApiClientToken())
                .Search<MpUserDto>($"dp_Users.User_Name='{emailAddress}'", columnList);

            return users;
        }
    }
}
