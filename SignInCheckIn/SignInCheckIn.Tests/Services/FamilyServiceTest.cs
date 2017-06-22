using System.Collections.Generic;
using Crossroads.Utilities.Services.Interfaces;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using NUnit.Framework;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Services;
using SignInCheckIn.Services.Interfaces;

namespace SignInCheckIn.Tests.Services
{
    public class FamilyServiceTest
    {
        private Mock<IContactRepository> _contactRepository;
        private Mock<IParticipantRepository> _participantRepository;
        private Mock<IApplicationConfiguration> _applicationConfiguration;
        private Mock<IPasswordService> _passwordService;
        private Mock<IChildSigninService> _childSigninSerivce;

        private FamilyService _fixture;

        [SetUp]
        public void SetUp()
        {
            AutoMapperConfig.RegisterMappings();

            _contactRepository = new Mock<IContactRepository>(MockBehavior.Strict);
            _participantRepository = new Mock<IParticipantRepository>(MockBehavior.Strict);
            _applicationConfiguration = new Mock<IApplicationConfiguration>(MockBehavior.Strict);
            _passwordService = new Mock<IPasswordService>(MockBehavior.Strict);
            _childSigninSerivce = new Mock<IChildSigninService>(MockBehavior.Strict);

            _applicationConfiguration.Setup(m => m.KidsClubRegistrationSourceId).Returns(48);
            _applicationConfiguration.Setup(m => m.AttendeeParticipantType).Returns(2);
            _applicationConfiguration.Setup(m => m.HeadOfHouseholdId).Returns(1);
            _applicationConfiguration.Setup(m => m.KidsClubPublicationId).Returns(3);
            _applicationConfiguration.Setup(m => m.AllPlatformUsersRoleId).Returns(39);
            _applicationConfiguration.Setup(m => m.GeneralPublicationId).Returns(2);

            _fixture = new FamilyService(_contactRepository.Object, _participantRepository.Object, _applicationConfiguration.Object, _passwordService.Object, _childSigninSerivce.Object);
        }

        [Test]
        public void ShouldSaveNewFamily()
        {
            // Arrange
            var token = "123abc";
            var kioskId = "aaa";
            var passwordResetToken = "abcdefgh12345678";

            var mpHouseholdDto = new MpHouseholdDto
            {
                HouseholdId = 1234567
            };

            var newParentDtos = new List<NewParentDto>
            {
                new NewParentDto
                {
                    CongregationId = 1,
                    FirstName = "first",
                    LastName = "last",
                    PhoneNumber = "555-555-0987",
                    EmailAddress = "test@test.com"
                }
            };

            var mpNewParticipantDtoFromRepo = new MpNewParticipantDto
            {
                FirstName = "first",
                LastName = "last",
                Contact = new MpContactDto
                {
                    HouseholdId = 1234567
                }
            };

            var mpNewContactDto = new MpContactDto
            {
                ContactId = 5544555
            };

            var mpUserDto = new MpUserDto
            {
                FirstName = "first", // contact?
                LastName = "last", // contact?
                UserEmail = "test@test.com",
                Password = "abcdefghijklmnopq",
                Company = false, //contact?
                DisplayName = "last, first",
                DomainId = 1,
                UserName = "test@test.com",
                ContactId = 5544555,
                PasswordResetToken = "abcdefgh12345678"
            };

            var mpNewUserDto = new MpUserDto
            {
                UserId = 6677667,
                FirstName = "first", // contact?
                LastName = "last", // contact?
                UserEmail = "test@test.com",
                Password = "abcdefghijklmnopq",
                Company = false, //contact?
                DisplayName = "last, first",
                DomainId = 1,
                UserName = "test@test.com",
                ContactId = 5544555,
                PasswordResetToken = "abcdefgh12345678"
            };

            _passwordService.Setup(r => r.GetNewUserPassword(16, 2)).Returns("abcdefghijklmnopq");
            _passwordService.Setup(r => r.GeneratorPasswordResetToken("test@test.com")).Returns("abcdefgh12345678");
            _contactRepository.Setup(r => r.GetUserByEmailAddress(token, "test@test.com")).Returns(new List<MpUserDto>());
            _contactRepository.Setup(m => m.CreateHousehold(token, It.IsAny<MpHouseholdDto>())).Returns(mpHouseholdDto);
            _contactRepository.Setup(m => m.GetContactById(token, It.IsAny<int>())).Returns(mpNewContactDto);
            _contactRepository.Setup(m => m.CreateUserRecord(token, It.IsAny<MpUserDto>())).Returns(mpNewUserDto);
            _contactRepository.Setup(m => m.CreateUserRoles(token, It.IsAny<List<MpUserRoleDto>>()));
            _contactRepository.Setup(m => m.CreateContactPublications(token, It.IsAny<List<MpContactPublicationDto>>()));
            _participantRepository.Setup(m => m.CreateParticipantWithContact(It.IsAny<MpNewParticipantDto>(), token)).Returns(mpNewParticipantDtoFromRepo);

            // Act
            var result = _fixture.CreateNewFamily(token, newParentDtos, kioskId);

            // Assert
            Assert.IsNotNull(result[0].HouseholdId);
            Assert.AreNotEqual(0, result.Count);
        }

        [Test]
        public void ShouldNotSaveExistingFamily()
        {
            // Arrange
            var token = "123abc";
            var kioskId = "aaa";
            var passwordResetToken = "abcdefgh12345678";

            var mpHouseholdDto = new MpHouseholdDto
            {
                HouseholdId = 1234567
            };

            var newParentDtos = new List<NewParentDto>
            {
                new NewParentDto
                {
                    CongregationId = 1,
                    FirstName = "first",
                    LastName = "last",
                    PhoneNumber = "555-555-0987",
                    EmailAddress = "test@test.com"
                }
            };

            var mpNewParticipantDtoFromRepo = new MpNewParticipantDto
            {
                FirstName = "first",
                LastName = "last",
                Contact = new MpContactDto
                {
                    HouseholdId = 1234567
                }
            };

            var mpNewContactDto = new MpContactDto
            {
                ContactId = 5544555
            };

            var mpUserDto = new MpUserDto
            {
                FirstName = "first", // contact?
                LastName = "last", // contact?
                UserEmail = "test@test.com",
                Password = "abcdefghijklmnopq",
                Company = false, //contact?
                DisplayName = "last, first",
                DomainId = 1,
                UserName = "test@test.com",
                ContactId = 5544555,
                PasswordResetToken = "abcdefgh12345678"
            };

            var mpNewUserDto = new MpUserDto
            {
                UserId = 6677667,
                FirstName = "first", // contact?
                LastName = "last", // contact?
                UserEmail = "test@test.com",
                Password = "abcdefghijklmnopq",
                Company = false, //contact?
                DisplayName = "last, first",
                DomainId = 1,
                UserName = "test@test.com",
                ContactId = 5544555,
                PasswordResetToken = "abcdefgh12345678"
            };

            _passwordService.Setup(r => r.GetNewUserPassword(16, 2)).Returns("abcdefghijklmnopq");
            _passwordService.Setup(r => r.GeneratorPasswordResetToken("test@test.com")).Returns("abcdefgh12345678");
            _contactRepository.Setup(r => r.GetUserByEmailAddress(token, "test@test.com")).Returns(new List<MpUserDto> { new MpUserDto() });
            _contactRepository.Setup(m => m.CreateHousehold(token, It.IsAny<MpHouseholdDto>())).Returns(mpHouseholdDto);
            _contactRepository.Setup(m => m.GetContactById(token, It.IsAny<int>())).Returns(mpNewContactDto);
            _contactRepository.Setup(m => m.CreateUserRecord(token, It.IsAny<MpUserDto>())).Returns(mpNewUserDto);
            _contactRepository.Setup(m => m.CreateUserRoles(token, It.IsAny<List<MpUserRoleDto>>()));
            _contactRepository.Setup(m => m.CreateContactPublications(token, It.IsAny<List<MpContactPublicationDto>>()));
            _participantRepository.Setup(m => m.CreateParticipantWithContact(It.IsAny<MpNewParticipantDto>(), token)).Returns(mpNewParticipantDtoFromRepo);

            // Act
            var result = _fixture.CreateNewFamily(token, newParentDtos, kioskId);

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void ShouldNotCreateNewUserWithoutEmailAddress()
        {
            // Arrange
            var token = "123abc";
            var kioskId = "aaa";
            var passwordResetToken = "abcdefgh12345678";

            var mpHouseholdDto = new MpHouseholdDto
            {
                HouseholdId = 1234567
            };

            var newParentDtos = new List<NewParentDto>
            {
                new NewParentDto
                {
                    CongregationId = 1,
                    FirstName = "first",
                    LastName = "last",
                    PhoneNumber = "555-555-0987",
                }
            };

            var mpNewParticipantDtoFromRepo = new MpNewParticipantDto
            {
                FirstName = "first",
                LastName = "last",
                Contact = new MpContactDto
                {
                    HouseholdId = 1234567
                }
            };

            var mpNewContactDto = new MpContactDto
            {
                ContactId = 5544555
            };

            var mpUserDto = new MpUserDto
            {
                FirstName = "first", // contact?
                LastName = "last", // contact?
                UserEmail = "test@test.com",
                Password = "abcdefghijklmnopq",
                Company = false, //contact?
                DisplayName = "last, first",
                DomainId = 1,
                UserName = "test@test.com",
                ContactId = 5544555,
                PasswordResetToken = "abcdefgh12345678"
            };

            var mpNewUserDto = new MpUserDto
            {
                UserId = 6677667,
                FirstName = "first", // contact?
                LastName = "last", // contact?
                UserEmail = "test@test.com",
                Password = "abcdefghijklmnopq",
                Company = false, //contact?
                DisplayName = "last, first",
                DomainId = 1,
                UserName = "test@test.com",
                ContactId = 5544555,
                PasswordResetToken = "abcdefgh12345678"
            };

            _passwordService.Setup(r => r.GetNewUserPassword(16, 2)).Returns("abcdefghijklmnopq");
            _passwordService.Setup(r => r.GeneratorPasswordResetToken("test@test.com")).Returns("abcdefgh12345678");
            _contactRepository.Setup(r => r.GetUserByEmailAddress(token, "test@test.com")).Returns(new List<MpUserDto> { new MpUserDto() });
            _contactRepository.Setup(m => m.CreateHousehold(token, It.IsAny<MpHouseholdDto>())).Returns(mpHouseholdDto);
            _contactRepository.Setup(m => m.GetContactById(token, It.IsAny<int>())).Returns(mpNewContactDto);
            _contactRepository.Setup(m => m.CreateContactPublications(token, It.IsAny<List<MpContactPublicationDto>>()));
            _participantRepository.Setup(m => m.CreateParticipantWithContact(It.IsAny<MpNewParticipantDto>(), token)).Returns(mpNewParticipantDtoFromRepo);

            // Act
            var result = _fixture.CreateNewFamily(token, newParentDtos, kioskId);

            // Assert
            Assert.AreEqual(1, result.Count);
        }
    }
}
