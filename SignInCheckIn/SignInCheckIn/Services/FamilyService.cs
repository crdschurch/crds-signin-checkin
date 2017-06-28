

using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Crossroads.Utilities.Services.Interfaces;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Services.Interfaces;

namespace SignInCheckIn.Services
{
    public class FamilyService : IFamilyService
    {
        private readonly IContactRepository _contactRepository;
        private readonly IParticipantRepository _participantRepository;
        private readonly IApplicationConfiguration _applicationConfiguration;
        private readonly IPasswordService _passwordService;
        private readonly IChildSigninService _childSigninService;

        public FamilyService(IContactRepository contactRepository,
                             IParticipantRepository participantRepository,
                             IApplicationConfiguration applicationConfiguration,
                             IPasswordService passwordService,
                             IChildSigninService childSigninService)
        {
            _contactRepository = contactRepository;
            _participantRepository = participantRepository;
            _applicationConfiguration = applicationConfiguration;
            _passwordService = passwordService;
            _childSigninService = childSigninService;
        }

        public List<MpNewParticipantDto> AddFamilyMembers(string token, int householdId, List<ContactDto> newContacts)
        {
            // get the adult contacts on the household to create the parent-child relationships
            var headsOfHousehold = _contactRepository.GetHeadsOfHouseholdByHouseholdId(householdId);

            // create the children contacts
            List<MpNewParticipantDto> mpNewChildParticipantDtos = new List<MpNewParticipantDto>();

            foreach (var childContactDto in newContacts)
            {
                var newParticipant = _childSigninService.CreateNewParticipantWithContact(childContactDto.FirstName,
                                                                     childContactDto.LastName,
                                                                     childContactDto.DateOfBirth,
                                                                     childContactDto.YearGrade,
                                                                     householdId,
                                                                     _applicationConfiguration.MinorChildId,
                                                                     childContactDto.IsSpecialNeeds,
                                                                     childContactDto.GenderId
                );

                mpNewChildParticipantDtos.Add(newParticipant);

            }

            foreach (var parent in headsOfHousehold)
            {
                List<MpContactRelationshipDto> mpContactRelationshipDtos = mpNewChildParticipantDtos.Select(item => new MpContactRelationshipDto
                {
                    ContactId = item.ContactId.GetValueOrDefault(),
                    RelationshipId = _applicationConfiguration.ChildOfRelationshipId,
                    RelatedContactId = parent.ContactId,
                    StartDate = System.DateTime.Now
                }).ToList();

                _contactRepository.CreateContactRelationships(token, mpContactRelationshipDtos);
            }

            return mpNewChildParticipantDtos;
        }

        public List<ContactDto> CreateNewFamily(string token, List<NewParentDto> newParentDtos, string kioskIdentifier)
        {
            // check to see if either parent already exists as a user - if so, don't create them. This is to match
            // logic on the family finder
            foreach (var parent in newParentDtos.Where(r => !String.IsNullOrEmpty(r.EmailAddress)))
            {
                var existingParents = _contactRepository.GetUserByEmailAddress(token, parent.EmailAddress);

                if (existingParents.Any())
                {
                    return new List<ContactDto>();
                }
            }

            // Step 1 - create the household
            MpHouseholdDto mpHouseholdDto = new MpHouseholdDto
            {
                HouseholdName = newParentDtos.First(r => !string.IsNullOrEmpty(r.LastName)).LastName,
                HomePhone = newParentDtos.First(r => !string.IsNullOrEmpty(r.PhoneNumber)).PhoneNumber,
                CongregationId = newParentDtos.First().CongregationId, // could be set in the backend, too, based on the kiosk config
                HouseholdSourceId = _applicationConfiguration.KidsClubRegistrationSourceId
            };

            mpHouseholdDto = _contactRepository.CreateHousehold(token, mpHouseholdDto);

            // Step 2 - create the parent contacts w/participants
            List<ContactDto> parentContactDtos = new List<ContactDto>();

            foreach (var parent in newParentDtos)
            {
                MpNewParticipantDto parentNewParticipantDto = new MpNewParticipantDto
                {
                    ParticipantTypeId = _applicationConfiguration.AttendeeParticipantType,
                    ParticipantStartDate = DateTime.Now,
                    Contact = new MpContactDto
                    {
                        FirstName = parent.FirstName,
                        Nickname = parent.FirstName,
                        LastName = parent.LastName,
                        EmailAddress = parent.EmailAddress,
                        MobilePhone = parent.PhoneNumber,
                        GenderId = parent.GenderId,
                        DisplayName = parent.LastName + ", " + parent.FirstName,
                        HouseholdId = mpHouseholdDto.HouseholdId,
                        HouseholdPositionId = _applicationConfiguration.HeadOfHouseholdId,
                        Company = false
                    }
                };

                var newParticipant = _participantRepository.CreateParticipantWithContact(parentNewParticipantDto, token);
                var newContact = _contactRepository.GetContactById(token, newParticipant.ContactId.GetValueOrDefault());

                // by default, new contacts get subscribed to these lists
                var mpContactPublicationDtos = new List<MpContactPublicationDto>
                {
                    new MpContactPublicationDto
                    {
                        ContactId = newContact.ContactId,
                        PublicationId = _applicationConfiguration.GeneralPublicationId,
                        Unsubscribed = false
                    },
                    new MpContactPublicationDto
                    {
                        ContactId = newContact.ContactId,
                        PublicationId = _applicationConfiguration.KidsClubPublicationId,
                        Unsubscribed = false
                    }
                };

                _contactRepository.CreateContactPublications(token, mpContactPublicationDtos);

                // since the username is an email address, don't create a new user if the new parent doesn't
                // provide one
                if (!string.IsNullOrEmpty(parent.EmailAddress))
                {
                    var newUserPassword = _passwordService.GetNewUserPassword(16, 2);
                    var newUserPasswordResetToken = _passwordService.GeneratorPasswordResetToken(parent.EmailAddress);

                    var mpUserDto = new MpUserDto
                    {
                        FirstName = parent.FirstName, // contact?
                        LastName = parent.LastName, // contact?
                        UserEmail = parent.EmailAddress,
                        Password = newUserPassword,
                        Company = false, //contact?
                        DisplayName = parent.LastName + ", " + parent.FirstName, // contact?
                        DomainId = 1,
                        UserName = parent.EmailAddress,
                        ContactId = newContact.ContactId,
                        PasswordResetToken = newUserPasswordResetToken
                    };

                    var newUserRecord = _contactRepository.CreateUserRecord(token, mpUserDto);

                    var mpUserRoleDtos = new List<MpUserRoleDto>
                    {
                        new MpUserRoleDto
                        {
                            UserId = newUserRecord.UserId,
                            RoleId = _applicationConfiguration.AllPlatformUsersRoleId
                        }
                    };

                    _contactRepository.CreateUserRoles(token, mpUserRoleDtos);
                }

                parentContactDtos.Add(Mapper.Map<ContactDto>(newContact));
            }

            return parentContactDtos;
        }

        public UserDto GetUserByEmailAddress(string token, string emailAddress)
        {
            var mpUser = _contactRepository.GetUserByEmailAddress(token, emailAddress);
            return Mapper.Map<UserDto>(mpUser.FirstOrDefault());
        }
    }
}
