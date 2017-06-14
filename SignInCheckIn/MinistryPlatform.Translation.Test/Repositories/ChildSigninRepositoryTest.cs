﻿using System;
using System.Collections.Generic;
using Crossroads.Utilities.Services.Interfaces;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using NUnit.Framework;
using SignInCheckIn;

namespace MinistryPlatform.Translation.Test.Repositories
{
    public class ChildSigninRepositoryTest
    {
        private ChildSigninRepository _fixture;
        private Mock<IApiUserRepository> _apiUserRepository;
        private Mock<IGroupLookupRepository> _groupLookupRepository;
        private Mock<IMinistryPlatformRestRepository> _ministryPlatformRestRepository;
        private Mock<IApplicationConfiguration> _applicationConfiguration;

        private List<string> _householdColumns;
        private List<string> _primaryHouseChildGroupParticipantColumns;
        private List<string> _otherHouseChildParticipantColumns;
        private List<string> _groupChildParticipantColumns;
        private List<string> _eventGroupColumns;
        private List<string> _contactColumns;

        [SetUp]
        public void SetUp()
        {
            AutoMapperConfig.RegisterMappings();

            _apiUserRepository = new Mock<IApiUserRepository>(MockBehavior.Strict);
            _groupLookupRepository = new Mock<IGroupLookupRepository>(MockBehavior.Strict);
            _ministryPlatformRestRepository = new Mock<IMinistryPlatformRestRepository>(MockBehavior.Strict);
            _applicationConfiguration = new Mock<IApplicationConfiguration>(MockBehavior.Default);
            _fixture = new ChildSigninRepository(_apiUserRepository.Object, _groupLookupRepository.Object, _ministryPlatformRestRepository.Object, _applicationConfiguration.Object);

            _householdColumns = new List<string>
            {
                "Contact_ID",
                "Household_ID_Table.Household_ID",
                "Household_Position_ID_Table.Household_Position_ID",
                "Household_ID_Table.Home_Phone",
                "Mobile_Phone",
                "Nickname",
                "Last_Name"
            };

            _primaryHouseChildGroupParticipantColumns = new List<string>
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

            _otherHouseChildParticipantColumns = new List<string>
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

            _groupChildParticipantColumns = new List<string>
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

            _eventGroupColumns = new List<string>
            {
                "Event_Group_ID",
                "Event_ID_Table.Event_ID",
                "Group_ID_Table.Group_ID"
            };

            _contactColumns = new List<string>
            {
                "Contact_ID",
                "Household_ID_Table.Household_ID",
                "Household_Position_ID_Table.Household_Position_ID",
                "Household_ID_Table.Home_Phone",
                "Mobile_Phone",
                "Nickname",
                "Last_Name"
            };
        }

        [Test]
        public void TestGetChildrenByPhoneNumberHouseholdPhoneNotFound()
        {
            const string phoneNumber = "513-867-5309";
            const bool includeOtherHousehold = false;

            var parms = new Dictionary<string, object>
            {
                {"Phone_Number", phoneNumber},
                {"Include_Other_Household", includeOtherHousehold}
            };
            _apiUserRepository.Setup(mocked => mocked.GetToken()).Returns("token");
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken("token")).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.GetFromStoredProc<MpParticipantDto>("api_crds_Child_Signin_Search", parms))
                .Returns(new List<List<MpParticipantDto>>());

            var response = _fixture.GetChildrenByPhoneNumber(phoneNumber, includeOtherHousehold);
            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();
            Assert.IsNotNull(response);
            Assert.IsFalse(response.HasHousehold);
            Assert.IsFalse(response.HasParticipants);
        }

        [Test]
        public void TestGetChildrenByPhoneNumberNoParticipants()
        {
            const string phoneNumber = "513-867-5309";
            const bool includeOtherHousehold = false;

            var parms = new Dictionary<string, object>
            {
                {"Phone_Number", phoneNumber},
                {"Include_Other_Household", includeOtherHousehold}
            };
            _apiUserRepository.Setup(mocked => mocked.GetToken()).Returns("token");
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken("token")).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.GetFromStoredProc<MpParticipantDto>("api_crds_Child_Signin_Search", parms))
                .Returns(new List<List<MpParticipantDto>>
                {
                    new List<MpParticipantDto>
                    {
                        new MpParticipantDto
                        {
                            HouseholdId = 123
                        }
                    },
                    new List<MpParticipantDto>()
                });

            var response = _fixture.GetChildrenByPhoneNumber(phoneNumber, includeOtherHousehold);
            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();
            Assert.IsNotNull(response);
            Assert.IsTrue(response.HasHousehold);
            Assert.AreEqual(123, response.HouseholdId);
            Assert.IsFalse(response.HasParticipants);
        }

        [Test]
        public void TestGetChildrenByPhoneNumber()
        {
            const string phoneNumber = "513-867-5309";
            const bool includeOtherHousehold = false;
            int? groupTypeId = null;

            var parms = new Dictionary<string, object>
            {
                {"Phone_Number", phoneNumber},
                {"Include_Other_Household", includeOtherHousehold}
            };

            var repoResponse = new List<List<MpParticipantDto>>
            {
                new List<MpParticipantDto>
                {
                    new MpParticipantDto
                    {
                        HouseholdId = 123
                    }
                },
                new List<MpParticipantDto>
                {
                    new MpParticipantDto()
                }
            };
            _apiUserRepository.Setup(mocked => mocked.GetToken()).Returns("token");
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken("token")).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.GetFromStoredProc<MpParticipantDto>("api_crds_Child_Signin_Search", parms)).Returns(repoResponse);

            var response = _fixture.GetChildrenByPhoneNumber(phoneNumber, includeOtherHousehold);
            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();
            Assert.IsNotNull(response);
            Assert.IsTrue(response.HasHousehold);
            Assert.AreEqual(123, response.HouseholdId);
            Assert.IsTrue(response.HasParticipants);
            Assert.AreSame(repoResponse[1], response.Participants);
        }

        [Test]

        public void TestGetChildrenByHouseholdIdWithHouseholdPhone()
        {
            var phoneNumber = "812-812-8877";
            int? primaryHouseholdId = 123;
            int? otherHouseholdId = 1222;

            var eventDto = new MpEventDto
            {
                CongregationId = 1,
                CongregationName = "Oakley",
                EarlyCheckinPeriod = 30,
                EventEndDate = DateTime.Now.AddDays(1),
                EventId = 1234567,
                EventStartDate = DateTime.Now,
                EventTitle = "test event",
                EventType = "type test",
                LateCheckinPeriod = 30,
                LocationId = 3
            };

            var houseHold = new List<MpContactDto>
            {
                new MpContactDto
                {
                    ContactId = 11,
                    HouseholdId = 123,
                    HouseholdPositionId = 3,
                    HomePhone = phoneNumber,
                    MobilePhone = null,
                    LastName = "LastName",
                    Nickname = "Nickname"
                }
            };

            var primaryChild = new List<MpGroupParticipantDto>
            {
                new MpGroupParticipantDto
                {
                    ParticipantId = 12,
                    ContactId = 1443,
                    HouseholdId = primaryHouseholdId.GetValueOrDefault(),
                    HouseholdPositionId = 2,
                    FirstName = "First1",
                    LastName = "Last1",
                    DateOfBirth = new DateTime()
                }
            };

            var otherChild = new List<MpParticipantDto>
            {
                new MpParticipantDto
                {
                    ParticipantId = 13,
                    ContactId = 1444,
                    HouseholdId = otherHouseholdId.GetValueOrDefault(),
                    HouseholdPositionId = 2,
                    FirstName = "First2",
                    LastName = "Last2",
                    DateOfBirth = new DateTime()
                },
                new MpParticipantDto
                {
                    ParticipantId = 13,
                    ContactId = 1444,
                    HouseholdId = otherHouseholdId.GetValueOrDefault(),
                    HouseholdPositionId = 2,
                    FirstName = "First2",
                    LastName = "Last2",
                    DateOfBirth = new DateTime()
                }
            };

            var children = new List<MpParticipantDto>
            {
                new MpParticipantDto
                {
                    ParticipantId = 12,
                    ContactId = 1443,
                    HouseholdId = primaryHouseholdId.GetValueOrDefault(),
                    HouseholdPositionId = 2,
                    FirstName = "First1",
                    LastName = "Last1",
                    DateOfBirth = new DateTime(),
                    GroupId = 123
                },
                new MpParticipantDto
                {
                    ParticipantId = 13,
                    ContactId = 1444,
                    HouseholdId = otherHouseholdId.GetValueOrDefault(),
                    HouseholdPositionId = 2,
                    FirstName = "First2",
                    LastName = "Last2",
                    DateOfBirth = new DateTime(),
                    GroupId = 12345
                },
                // Add a duplicate - this one should not appear in the final list
                new MpParticipantDto
                {
                    ParticipantId = 13,
                    ContactId = 1444,
                    HouseholdId = otherHouseholdId.GetValueOrDefault(),
                    HouseholdPositionId = 2,
                    FirstName = "First2",
                    LastName = "Last2",
                    DateOfBirth = new DateTime(),
                    GroupId = 12345
                }
            };

            var mpEventGroupDtos = new List<MpEventGroupDto>
            {
                new MpEventGroupDto(),
                new MpEventGroupDto(),
                new MpEventGroupDto()
            };
            mpEventGroupDtos[0].GroupId = 123;
            mpEventGroupDtos[1].GroupId = 1234;
            mpEventGroupDtos[2].GroupId = 12345;
            var headsOfHousehold = new List<MpContactDto>();

            _apiUserRepository.Setup(mocked => mocked.GetToken()).Returns("auth");
            _applicationConfiguration.Setup(mocked => mocked.KidsClubGroupTypeId).Returns(4);
            _applicationConfiguration.Setup(mocked => mocked.KidsClubMinistryId).Returns(2);
            _applicationConfiguration.Setup(mocked => mocked.KidsClubCongregationId).Returns(5);
            _applicationConfiguration.Setup(mocked => mocked.HeadOfHouseholdId).Returns(1);
            _applicationConfiguration.Setup(mocked => mocked.OtherAdultId).Returns(3);
            _applicationConfiguration.Setup(mocked => mocked.AdultChildId).Returns(4);
            _applicationConfiguration.Setup(mocked => mocked.MinorChildId).Returns(2);
            _applicationConfiguration.Setup(mocked => mocked.StudentMinistryId).Returns(34);
            _applicationConfiguration.Setup(mocked => mocked.HouseHoldIdsThatCanCheckIn).Returns("3,4,2");
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken("auth")).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpGroupParticipantDto>(GetChildParticpantsByPrimaryHouseholdFilter(primaryHouseholdId.GetValueOrDefault()), _primaryHouseChildGroupParticipantColumns, null, false)).Returns(primaryChild);
            _ministryPlatformRestRepository.Setup(mocked => mocked.SearchTable<MpParticipantDto>("Contact_Households", GetChildParticpantsByOtherHouseholdFilter(primaryHouseholdId.GetValueOrDefault()), _otherHouseChildParticipantColumns, null, false)).Returns(otherChild);
            _ministryPlatformRestRepository.Setup(mocked => mocked.SearchTable<MpParticipantDto>("Group_Participants", GetChildParticpantsByGroupFilter("12,13"), _groupChildParticipantColumns, null, false)).Returns(children);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpEventGroupDto>($"Event_ID_Table.[Event_ID] = {eventDto.EventId}", _eventGroupColumns, null, false)).Returns(mpEventGroupDtos);

            var result = _fixture.GetChildrenByHouseholdId(primaryHouseholdId, eventDto.EventId);
            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();
            
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(result[0].ContactId, primaryChild[0].ContactId);
            Assert.AreEqual(result[1].ContactId, otherChild[0].ContactId);
        }

        [Test]
        public void TestGetChildrenByHouseholdIdWithHousholdPhoneNotAllInGroup()
        {
            var phoneNumber = "812-812-8877";
            int? primaryHouseholdId = 123;
            int? otherHouseholdId = 1222;

            var eventDto = new MpEventDto
            {
                CongregationId = 1,
                CongregationName = "Oakley",
                EarlyCheckinPeriod = 30,
                EventEndDate = DateTime.Now.AddDays(1),
                EventId = 1234567,
                EventStartDate = DateTime.Now,
                EventTitle = "test event",
                EventType = "type test",
                LateCheckinPeriod = 30,
                LocationId = 3
            };

            var houseHold = new List<MpContactDto>
            {
                new MpContactDto
                {
                    ContactId = 11,
                    HouseholdId = 123,
                    HouseholdPositionId = 3,
                    HomePhone = phoneNumber,
                    MobilePhone = null,
                    LastName = "LastName",
                    Nickname = "NickName"
                }
            };

            var primaryChild = new List<MpGroupParticipantDto>
            {
                new MpGroupParticipantDto
                {
                    ParticipantId = 12,
                    ContactId = 1443,
                    HouseholdId = primaryHouseholdId.GetValueOrDefault(),
                    HouseholdPositionId = 2,
                    FirstName = "First1",
                    LastName = "Last1",
                    DateOfBirth = new DateTime(),
                    GroupId = 123
                }
            };

            var otherChild = new List<MpParticipantDto>
            {
                new MpParticipantDto
                {
                    ParticipantId = 13,
                    ContactId = 1444,
                    HouseholdId = otherHouseholdId.GetValueOrDefault(),
                    HouseholdPositionId = 2,
                    FirstName = "First2",
                    LastName = "Last2",
                    DateOfBirth = new DateTime(),
                    GroupId = 567
                }
            };

            var children = new List<MpParticipantDto>
            {
                new MpParticipantDto
                {
                    ParticipantId = 12,
                    ContactId = 1443,
                    HouseholdId = primaryHouseholdId.GetValueOrDefault(),
                    HouseholdPositionId = 2,
                    FirstName = "First1",
                    LastName = "Last1",
                    DateOfBirth = new DateTime(),
                    GroupId = 123
                },
                new MpParticipantDto
                {
                    ParticipantId = 13,
                    ContactId = 1444,
                    HouseholdId = otherHouseholdId.GetValueOrDefault(),
                    HouseholdPositionId = 2,
                    FirstName = "First2",
                    LastName = "Last2",
                    DateOfBirth = new DateTime(),
                    GroupId = 567
                }
            };

            var mpEventGroupDtos = new List<MpEventGroupDto>
            {
                new MpEventGroupDto(),
                new MpEventGroupDto(),
                new MpEventGroupDto()
            };
            mpEventGroupDtos[0].GroupId = 123;
            mpEventGroupDtos[1].GroupId = 1234;
            mpEventGroupDtos[2].GroupId = 12345;
            var headsOfHousehold = new List<MpContactDto>();

            _apiUserRepository.Setup(mocked => mocked.GetToken()).Returns("auth");
            _applicationConfiguration.Setup(mocked => mocked.KidsClubGroupTypeId).Returns(4);
            _applicationConfiguration.Setup(mocked => mocked.KidsClubMinistryId).Returns(2);
            _applicationConfiguration.Setup(mocked => mocked.KidsClubCongregationId).Returns(5);
            _applicationConfiguration.Setup(mocked => mocked.HeadOfHouseholdId).Returns(1);
            _applicationConfiguration.Setup(mocked => mocked.OtherAdultId).Returns(3);
            _applicationConfiguration.Setup(mocked => mocked.AdultChildId).Returns(4);
            _applicationConfiguration.Setup(mocked => mocked.MinorChildId).Returns(2);
            _applicationConfiguration.Setup(mocked => mocked.HouseHoldIdsThatCanCheckIn).Returns("3,4,2");
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken("auth")).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpGroupParticipantDto>(GetChildParticpantsByPrimaryHouseholdFilter(primaryHouseholdId.GetValueOrDefault()), It.IsAny<List<string>>(), null, false)).Returns(primaryChild);
            _ministryPlatformRestRepository.Setup(mocked => mocked.SearchTable<MpParticipantDto>("Contact_Households", It.IsAny<string>(), _otherHouseChildParticipantColumns, null, false)).Returns(otherChild);
            _ministryPlatformRestRepository.Setup(mocked => mocked.SearchTable<MpParticipantDto>("Group_Participants", It.IsAny<string>(), _groupChildParticipantColumns, null, false)).Returns(children);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpEventGroupDto>($"Event_ID_Table.[Event_ID] = {eventDto.EventId}", _eventGroupColumns, null, false)).Returns(mpEventGroupDtos);
            _groupLookupRepository.Setup(mocked => mocked.GetGradeAttributeId(It.IsAny<int>())).Returns(9033);

            var result = _fixture.GetChildrenByHouseholdId(primaryHouseholdId, eventDto.EventId);
            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();
            
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(children[0].ContactId, result[0].ContactId);
        }
        
        [Test]
        public void TestGetChildrenByHouseholdIdWithNoPhone()
        {
            int? householdId = 1234567;

            var eventDto = new MpEventDto
            {
                CongregationId = 1,
                CongregationName = "Oakley",
                EarlyCheckinPeriod = 30,
                EventEndDate = DateTime.Now.AddDays(1),
                EventId = 1234567,
                EventStartDate = DateTime.Now,
                EventTitle = "test event",
                EventType = "type test",
                LateCheckinPeriod = 30,
                LocationId = 3
            };

            List<MpEventGroupDto> mpEventGroupDtos = new List<MpEventGroupDto>();

            var houseHold = new List<MpContactDto>();
            List<MpGroupParticipantDto> groupParticipants = new List<MpGroupParticipantDto>();
            List<MpParticipantDto> participants = new List<MpParticipantDto>();

            _apiUserRepository.Setup(mocked => mocked.GetToken()).Returns("auth");
            _applicationConfiguration.Setup(mocked => mocked.KidsClubGroupTypeId).Returns(4);
            _applicationConfiguration.Setup(mocked => mocked.KidsClubMinistryId).Returns(2);
            _applicationConfiguration.Setup(mocked => mocked.KidsClubCongregationId).Returns(5);
            _applicationConfiguration.Setup(mocked => mocked.HeadOfHouseholdId).Returns(1);
            _applicationConfiguration.Setup(mocked => mocked.OtherAdultId).Returns(3);
            _applicationConfiguration.Setup(mocked => mocked.AdultChildId).Returns(4);
            _applicationConfiguration.Setup(mocked => mocked.MinorChildId).Returns(2);
            _applicationConfiguration.Setup(mocked => mocked.HouseHoldIdsThatCanCheckIn).Returns("3,4,2");
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken("auth")).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(
                mocked => mocked.Search<MpGroupParticipantDto>(GetChildParticpantsByPrimaryHouseholdFilter(householdId.GetValueOrDefault()), It.IsAny<List<string>>(), null, false)).Returns(groupParticipants);

            _ministryPlatformRestRepository.Setup(
                mocked => mocked.Search<MpEventGroupDto>(It.IsAny<string>(), It.IsAny<List<string>>(), null, false)).Returns(mpEventGroupDtos);

            _ministryPlatformRestRepository.Setup(mocked => mocked.SearchTable<MpParticipantDto>
                (It.IsAny<string>(), GetChildParticpantsByOtherHouseholdFilter(householdId.GetValueOrDefault()), _otherHouseChildParticipantColumns, null, false)).Returns(participants);
            _groupLookupRepository.Setup(mocked => mocked.GetGradeAttributeId(It.IsAny<int>())).Returns(9033);

            var result = _fixture.GetChildrenByHouseholdId(householdId, eventDto.EventId);
            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        private string GetHousholdFilter(string phoneNumber)
        {
            var phoneNumberWithoutDashes = phoneNumber.Replace("-", "");
            return
                $"Household_Position_ID_Table.[Household_Position_ID] IN (3,4,2) AND ([Mobile_Phone] = '{phoneNumber}' OR [Mobile_Phone] = '{phoneNumberWithoutDashes}' OR Household_ID_Table.[Home_Phone] = '{phoneNumber}' OR Household_ID_Table.[Home_Phone] = '{phoneNumberWithoutDashes}')";
        }

	    private static string GetChildParticpantsByPrimaryHouseholdFilter(int householdId)
        {
            return $"Participant_ID_Table_Contact_ID_Table_Household_ID_Table.[Household_ID] = {householdId} AND Participant_ID_Table_Contact_ID_Table_Household_Position_ID_Table.[Household_Position_ID] = 2 and Group_ID_Table_Group_Type_ID_Table.[Group_Type_ID] = 4 AND Group_Participants.[End_Date] IS NULL";
        }

        private static string GetChildParticpantsByOtherHouseholdFilter(int householdId)
        {
            return $"Household_Position_ID_Table.[Household_Position_ID] = 2  AND Household_ID_Table.[Household_ID] = {householdId}";
        }

        private static string GetChildParticpantsByGroupFilter(string participantIds)
        {
            return $"Participant_ID_Table.[Participant_ID] IN ({participantIds}) AND Group_ID_Table_Congregation_ID_Table.[Congregation_ID] = 5 AND Group_ID_Table_Group_Type_ID_Table.[Group_Type_ID] = 4 AND Group_ID_Table_Ministry_ID_Table.[Ministry_ID] IN (2, 34) AND Group_Participants.[End_Date] IS NULL";
        }
    }
}

