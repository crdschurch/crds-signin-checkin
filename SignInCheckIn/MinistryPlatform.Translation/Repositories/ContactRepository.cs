﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;

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

        public MpHouseholdDto CreateHousehold(string token, MpHouseholdDto mpHouseholdDto)
        {
            List<string> householdColumns = new List<string>
            {
                "Households.Household_ID",
                "Households.Home_Phone",
                "Households.Household_Name"
            };

           return _ministryPlatformRestRepository.UsingAuthenticationToken(token).Create(mpHouseholdDto, householdColumns);
        }

        public MpContactDto Update(MpContactDto contactDto, string token)
        {
            List<string> columnList = new List<string>
            {
                "Contacts.[Last_Name]",
                "Contacts.[Nickname]",
                "Contacts.[Date_of_Birth]",
                "Gender_ID_Table.[Gender_ID]"
            };

            return _ministryPlatformRestRepository.UsingAuthenticationToken(token).Update<MpContactDto>(contactDto, columnList);
        }

        public void CreateContactRelationships(string token, List<MpContactRelationshipDto> contactRelationshipDtos)
         {
             List<string> columnList = new List<string>
             {
                 "Contact_ID",
                 "Related_Contact_ID",
                 "Relationship_ID",
                 "Start_Date"
             };
 
             _ministryPlatformRestRepository.UsingAuthenticationToken(token).Create(contactRelationshipDtos, columnList);
         }

        public MpContactDto GetContactById(string token, int contactId)
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

            var contact = _ministryPlatformRestRepository.UsingAuthenticationToken(token)
                .Search<MpContactDto>($"Contacts.Contact_ID={contactId}", contactColumnList).FirstOrDefault();

            return contact;
        }

        public MpUserDto CreateUserRecord(string token, MpUserDto mpUserDto)
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

            return _ministryPlatformRestRepository.UsingAuthenticationToken(token).Create(mpUserDto, columnList);
        }

        public void CreateUserRoles(string token, List<MpUserRoleDto> mpUserRoleDtos)
        {
            var columnList = new List<string>
            {
                "User_ID",
                "Role_ID"
            };

            _ministryPlatformRestRepository.UsingAuthenticationToken(token).Create(mpUserRoleDtos, columnList);
        }

        public void CreateContactPublications(string token, List<MpContactPublicationDto> contactPublicationDtos)
        {
            var columnList = new List<string>
            {
                "Contact_ID",
                "Publication_ID",
                "Unsubscribed"
            };

            _ministryPlatformRestRepository.UsingAuthenticationToken(token).Create(contactPublicationDtos, columnList);
        }

        public List<MpUserDto> GetUserByEmailAddress(string token, string emailAddress)
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

            var users = _ministryPlatformRestRepository.UsingAuthenticationToken(token)
                .Search<MpUserDto>($"dp_Users.User_Name='{emailAddress}'", columnList);

            return users;
        }
    }
}
