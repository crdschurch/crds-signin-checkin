using System;
using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using NUnit.Framework;

namespace MinistryPlatform.Translation.Test.Repositories
{
    public class ChildCheckinRepositoryTest
    {
        private ChildCheckinRepository _fixture;
        private Mock<IApiUserRepository> _apiUserRepository;
        private Mock<IMinistryPlatformRestRepository> _ministryPlatformRestRepository;
        private List<string> _householdColumns;
        private List<string> _primaryHouseChildParticipantColumns;
        private List<string> _otherHouseChildParticipantColumns;

        [SetUp]
        public void SetUp()
        {
            _apiUserRepository = new Mock<IApiUserRepository>(MockBehavior.Strict);
            _ministryPlatformRestRepository = new Mock<IMinistryPlatformRestRepository>(MockBehavior.Strict);
            _fixture = new ChildCheckinRepository(_apiUserRepository.Object, _ministryPlatformRestRepository.Object);

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
        }

        [Test]
        public void TestGetChildrenByPhoneNumberWithHousholdPhone()
        {
            var phoneNumber = "812-812-8877";
            var primaryHouseholdId = 123;
            var otherHouseholdId = 1222;

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

            _apiUserRepository.Setup(mocked => mocked.GetToken()).Returns("auth");
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken("auth")).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpContactDto>(GetHousholdFilter(phoneNumber), _householdColumns)).Returns(houseHold);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpParticipantDto>(GetChildParticpantsByPrimaryHouseholdFilter(primaryHouseholdId), _primaryHouseChildParticipantColumns)).Returns(primaryChild);
            _ministryPlatformRestRepository.Setup(mocked => mocked.SearchTable<MpParticipantDto>("Contact_Households", GetChildParticpantsByOtherHouseholdFilter(primaryHouseholdId), _otherHouseChildParticipantColumns)).Returns(otherChild);

            var result = _fixture.GetChildrenByPhoneNumber(phoneNumber);
            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();
            
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 2);
            Assert.AreEqual(result[0].ContactId, primaryChild[0].ContactId);
            Assert.AreEqual(result[1].ContactId, otherChild[0].ContactId);
        }
        
        [Test]
        public void TestGetChildrenByPhoneNumberWithNoPhone()
        {
            var phoneNumber = "812-812-8877";

            var houseHold = new List<MpContactDto>();

            _apiUserRepository.Setup(mocked => mocked.GetToken()).Returns("auth");
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken("auth")).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpContactDto>(GetHousholdFilter(phoneNumber), _householdColumns)).Returns(houseHold);
 
            var result = _fixture.GetChildrenByPhoneNumber(phoneNumber);
            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 0);
        }

        private string GetHousholdFilter(string phoneNumber)
        {
            return $"Household_Position_ID_Table.[Household_Position_ID] IN (1,3,4) AND ([Mobile_Phone] = '{phoneNumber}' OR Household_ID_Table.[Home_Phone] = '{phoneNumber}')";
        }

        private string GetChildParticpantsByPrimaryHouseholdFilter(int householdId)
        {
            return $"Contact_ID_Table_Household_ID_Table.[Household_ID] = {householdId} AND Contact_ID_Table_Household_Position_ID_Table.[Household_Position_ID] = 2";
        }

        private string GetChildParticpantsByOtherHouseholdFilter(int householdId)
        {
            return $"Household_Position_ID_Table.[Household_Position_ID] = 2  AND Household_ID_Table.[Household_ID] = {householdId}";
        }
    }
}