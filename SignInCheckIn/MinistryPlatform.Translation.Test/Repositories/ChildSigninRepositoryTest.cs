using System;
using System.Collections.Generic;
using System.Linq;
using Crossroads.Utilities.Services.Interfaces;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using NUnit.Framework;

namespace MinistryPlatform.Translation.Test.Repositories
{
    public class ChildSigninRepositoryTest
    {
        private ChildSigninRepository _fixture;
        private Mock<IApiUserRepository> _apiUserRepository;
        private Mock<IMinistryPlatformRestRepository> _ministryPlatformRestRepository;
        private Mock<IApplicationConfiguration> _applicationConfiguration;

        private List<string> _householdColumns;
        private List<string> _primaryHouseChildParticipantColumns;
        private List<string> _otherHouseChildParticipantColumns;
        private List<string> _groupChildParticipantColumns;
        private List<String> _eventGroupColumns;

        [SetUp]
        public void SetUp()
        {
            _apiUserRepository = new Mock<IApiUserRepository>(MockBehavior.Strict);
            _ministryPlatformRestRepository = new Mock<IMinistryPlatformRestRepository>(MockBehavior.Strict);
            _applicationConfiguration = new Mock<IApplicationConfiguration>(MockBehavior.Default);
            _fixture = new ChildSigninRepository(_apiUserRepository.Object, _ministryPlatformRestRepository.Object, _applicationConfiguration.Object);

            _householdColumns = new List<string>
            {
                "Contact_ID",
                "Household_ID_Table.Household_ID",
                "Household_Position_ID_Table.Household_Position_ID",
                "Household_ID_Table.Home_Phone",
                "Mobile_Phone",
            };

            _primaryHouseChildParticipantColumns = new List<string>
            {
                "Participant_ID",
                "Contact_ID_Table.Contact_ID",
                "Contact_ID_Table_Household_ID_Table.Household_ID",
                "Contact_ID_Table_Household_Position_ID_Table.Household_Position_ID",
                "Contact_ID_Table.First_Name",
                "Contact_ID_Table.Last_Name",
                "Contact_ID_Table.Date_of_Birth",
            };

            _otherHouseChildParticipantColumns = new List<string>
            {
                "Contact_ID_Table_Participant_Record_Table.Participant_ID",
                "Contact_ID_Table.Contact_ID",
                "Household_ID_Table.Household_ID",
                "Household_Position_ID_Table.Household_Position_ID",
                "Contact_ID_Table.First_Name",
                "Contact_ID_Table.Last_Name",
                "Contact_ID_Table.Date_of_Birth",
            };

            _groupChildParticipantColumns = new List<string>
            {
                "Participant_ID_Table.Participant_ID",
                "Participant_ID_Table_Contact_ID_Table.Contact_ID",
                "Participant_ID_Table_Contact_ID_Table_Household_ID_Table.Household_ID",
                "Participant_ID_Table_Contact_ID_Table_Household_Position_ID_Table.Household_Position_ID",
                "Participant_ID_Table_Contact_ID_Table.First_Name",
                "Participant_ID_Table_Contact_ID_Table.Last_Name",
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
        }

        [Test]
        public void TestGetChildrenByPhoneNumberWithHouseholdPhone()
        {
            var phoneNumber = "812-812-8877";
            var primaryHouseholdId = 123;
            var otherHouseholdId = 1222;

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
                    MobilePhone = null
                }
            };

            var primaryChild = new List<MpParticipantDto>
            {
                new MpParticipantDto
                {
                    ParticipantId = 12,
                    ContactId = 1443,
                    HouseholdId = primaryHouseholdId,
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
                    HouseholdId = otherHouseholdId,
                    HouseholdPositionId = 2,
                    FirstName = "First2",
                    LastName = "Last2",
                    DateOfBirth = new DateTime()
                },
                new MpParticipantDto
                {
                    ParticipantId = 13,
                    ContactId = 1444,
                    HouseholdId = otherHouseholdId,
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
                    HouseholdId = primaryHouseholdId,
                    HouseholdPositionId = 2,
                    FirstName = "First1",
                    LastName = "Last1",
                    DateOfBirth = new DateTime()
                },
                new MpParticipantDto
                {
                    ParticipantId = 13,
                    ContactId = 1444,
                    HouseholdId = otherHouseholdId,
                    HouseholdPositionId = 2,
                    FirstName = "First2",
                    LastName = "Last2",
                    DateOfBirth = new DateTime()
                },
                // Add a duplicate - this one should not appear in the final list
                new MpParticipantDto
                {
                    ParticipantId = 13,
                    ContactId = 1444,
                    HouseholdId = otherHouseholdId,
                    HouseholdPositionId = 2,
                    FirstName = "First2",
                    LastName = "Last2",
                    DateOfBirth = new DateTime()
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
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpContactDto>(GetHousholdFilter(phoneNumber), _householdColumns)).Returns(houseHold);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpParticipantDto>(GetChildParticpantsByPrimaryHouseholdFilter(primaryHouseholdId), _primaryHouseChildParticipantColumns)).Returns(primaryChild);
            _ministryPlatformRestRepository.Setup(mocked => mocked.SearchTable<MpParticipantDto>("Contact_Households", GetChildParticpantsByOtherHouseholdFilter(primaryHouseholdId), _otherHouseChildParticipantColumns)).Returns(otherChild);
            _ministryPlatformRestRepository.Setup(mocked => mocked.SearchTable<MpParticipantDto>("Group_Participants", GetChildParticpantsByGroupFilter("12,13"), _groupChildParticipantColumns)).Returns(children);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpEventGroupDto>($"Event_ID_Table.[Event_ID] = {eventDto.EventId}", _eventGroupColumns)).Returns(mpEventGroupDtos);

            var result = _fixture.GetChildrenByPhoneNumber(phoneNumber, eventDto);
            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();
            
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 2);
            Assert.AreEqual(result[0].ContactId, primaryChild[0].ContactId);
            Assert.AreEqual(result[1].ContactId, otherChild[0].ContactId);
        }

        [Test]
        public void TestGetChildrenByPhoneNumberWithHousholdPhoneNotAllInGroup()
        {
            var phoneNumber = "812-812-8877";
            var primaryHouseholdId = 123;
            var otherHouseholdId = 1222;

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
                    MobilePhone = null
                }
            };

            var primaryChild = new List<MpParticipantDto>
            {
                new MpParticipantDto
                {
                    ParticipantId = 12,
                    ContactId = 1443,
                    HouseholdId = primaryHouseholdId,
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
                    HouseholdId = otherHouseholdId,
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
                    HouseholdId = primaryHouseholdId,
                    HouseholdPositionId = 2,
                    FirstName = "First1",
                    LastName = "Last1",
                    DateOfBirth = new DateTime()
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
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpContactDto>(GetHousholdFilter(phoneNumber), _householdColumns)).Returns(houseHold);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpParticipantDto>(GetChildParticpantsByPrimaryHouseholdFilter(primaryHouseholdId), _primaryHouseChildParticipantColumns)).Returns(primaryChild);
            _ministryPlatformRestRepository.Setup(mocked => mocked.SearchTable<MpParticipantDto>("Contact_Households", GetChildParticpantsByOtherHouseholdFilter(primaryHouseholdId), _otherHouseChildParticipantColumns)).Returns(otherChild);
            _ministryPlatformRestRepository.Setup(mocked => mocked.SearchTable<MpParticipantDto>("Group_Participants", GetChildParticpantsByGroupFilter("12,13"), _groupChildParticipantColumns)).Returns(children);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpEventGroupDto>($"Event_ID_Table.[Event_ID] = {eventDto.EventId}", _eventGroupColumns)).Returns(mpEventGroupDtos);

            var result = _fixture.GetChildrenByPhoneNumber(phoneNumber, eventDto);
            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();
            
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(result[0].ContactId, children[0].ContactId);
        }
        
        [Test]
        public void TestGetChildrenByPhoneNumberWithNoPhone()
        {
            var phoneNumber = "812-812-8877";

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

            var houseHold = new List<MpContactDto>();

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
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpContactDto>(GetHousholdFilter(phoneNumber), _householdColumns)).Returns(houseHold);
 
            var result = _fixture.GetChildrenByPhoneNumber(phoneNumber, eventDto);
            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 0);
        }

        private string GetHousholdFilter(string phoneNumber)
        {
            return $"Household_Position_ID_Table.[Household_Position_ID] IN (3,4,2) AND ([Mobile_Phone] = '{phoneNumber}' OR Household_ID_Table.[Home_Phone] = '{phoneNumber}')";
        }

        private string GetChildParticpantsByPrimaryHouseholdFilter(int householdId)
        {
            return $"Contact_ID_Table_Household_ID_Table.[Household_ID] = {householdId} AND Contact_ID_Table_Household_Position_ID_Table.[Household_Position_ID] = 2";
        }

        private string GetChildParticpantsByOtherHouseholdFilter(int householdId)
        {
            return $"Household_Position_ID_Table.[Household_Position_ID] = 2  AND Household_ID_Table.[Household_ID] = {householdId}";
        }

        private string GetChildParticpantsByGroupFilter(string participantIds)
        {
            return $"Participant_ID_Table.[Participant_ID] IN ({participantIds}) AND Group_ID_Table_Congregation_ID_Table.[Congregation_ID] = 5 AND Group_ID_Table_Group_Type_ID_Table.[Group_Type_ID] = 4 AND Group_ID_Table_Ministry_ID_Table.[Ministry_ID] = 2 AND Group_ID_Table.[Group_ID] in (123,1234,12345)";
        }
    }
}