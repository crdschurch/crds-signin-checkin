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

            _authenticationRepository.Setup(m => m.Authenticate(username, password)).Returns(authData);

            // Act
            _fixture.Login(username, password);

            // Assert
            _authenticationRepository.VerifyAll();
        }

        // temporarily commented out
        //[Test]
        //public void ShouldAuthenticateUserFail()
        //{
        //    // Arrange
        //    string username = "test@test.com";
        //    string password = "12345678";

        //    Dictionary<string, object> authData = new Dictionary<string, object>
        //    {
        //        {"token", "123"},
        //        {"exp", 123},
        //        {"refreshToken", "456"}
        //    };

        //    _authenticationRepository.Setup(m => m.Authenticate(username, password)).Returns(null);

        //    // Act
        //    _fixture.Login(username, password);

        //    // Assert
        //    _authenticationRepository.VerifyAll();
        //}
    }
}
