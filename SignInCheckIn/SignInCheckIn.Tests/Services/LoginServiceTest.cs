using System.Collections.Generic;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using NUnit.Framework;
using SignInCheckIn.Services;

namespace SignInCheckIn.Tests.Services
{
    [TestFixture]
    public class LoginServiceTest
    {
        private Mock<IAuthenticationRepository> _authenticationRepository;

        private LoginService _fixture;

        [SetUp]
        public void SetUp()
        {
            _authenticationRepository = new Mock<IAuthenticationRepository>();

            _fixture = new LoginService(_authenticationRepository.Object);
        }

        [Test]
        public void ShouldAuthenticateUserSuccess()
        {
            // Arrange
            string username = "test@test.com";
            string password = "12345678";

            Dictionary<string, object> authData = new Dictionary<string, object>
            {
                {"token", "123"},
                {"exp", 123},
                {"refreshToken", "456"}
            };

            List<string> authRoles = new List<string>
            {
                "Kids Club Tools - CRDS"
            };

            _authenticationRepository.Setup(m => m.Authenticate(username, password)).Returns(authData);
            _authenticationRepository.Setup(m => m.GetUserRolesFromToken(authData["token"].ToString())).Returns(authRoles);

            // Act
            _fixture.Login(username, password);

            // Assert
            _authenticationRepository.VerifyAll();
        }
    }
}
