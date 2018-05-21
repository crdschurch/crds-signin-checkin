using System;
using System.Collections.Generic;
using System.Net.Cache;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http.Results;
using Crossroads.Web.Common.Security;
using FluentAssertions;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using NUnit.Framework;
using SignInCheckIn.Controllers;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Services.Interfaces;
using Crossroads.Web.Common.Services;

namespace SignInCheckIn.Tests.Controllers
{
    public class FamilyControllerTest
    {
        private FamilyController _fixture;
        
        private Mock<IAuthenticationRepository> _authenticationRepository;
        private Mock<IContactRepository> _contactRepository;
        private Mock<IKioskRepository> _kioskRepository;
        private Mock<IFamilyService> _familyService;
        private Mock<IChildSigninService> _childSigninService;
        private Mock<IAuthTokenExpiryService> _authTokenExpiryService;

        private Mock<HttpRequestMessage> _httpRequest;

        private const string AuthType = "abc";
        private const string AuthToken = "123";
        private readonly string _auth = $"{AuthType} {AuthToken}";

        [SetUp]
        public void SetUp()
        {
            _authenticationRepository = new Mock<IAuthenticationRepository>(MockBehavior.Strict);
            _authTokenExpiryService = new Mock<IAuthTokenExpiryService>();
            _contactRepository = new Mock<IContactRepository>();
            _kioskRepository = new Mock<IKioskRepository>();
            _familyService = new Mock<IFamilyService>(MockBehavior.Strict);
            _childSigninService = new Mock<IChildSigninService>();

            _fixture = new FamilyController(_authTokenExpiryService.Object, _authenticationRepository.Object, _contactRepository.Object, _kioskRepository.Object,
                _familyService.Object, _childSigninService.Object);
            _fixture.SetupAuthorization(AuthType, AuthToken);
        }

        [Test]
        public void ShouldCallCreateNewFamily()
        {
            // Arrange
            var newParentDtos = new List<NewParentDto>();

            var mpKioskConfigDto = new MpKioskConfigDto
            {
                KioskTypeId = 3
            };

            _kioskRepository.Setup(r => r.GetMpKioskConfigByIdentifier(It.IsAny<Guid>())).Returns(mpKioskConfigDto);
            _familyService.Setup(r => r.CreateNewFamily(It.IsAny<string>(), It.IsAny<List<NewParentDto>>(), It.IsAny<string>()))
                .Returns(new List<ContactDto>());

            _fixture.Request.Headers.Add("Crds-Kiosk-Identifier", Guid.NewGuid().ToString());


            // Act
            var result = _fixture.CreateNewFamily(newParentDtos);

            // Assert
            _familyService.VerifyAll();
            result.Should().BeOfType<OkNegotiatedContentResult<List<ContactDto>>>();
        }

        [Test]
        public void ShouldNotCallCreateNewFamilyWithWrongKioskId()
        {
            // Arrange
            var newParentDtos = new List<NewParentDto>();

            var mpKioskConfigDto = new MpKioskConfigDto
            {
                KioskTypeId = 1
            };

            _kioskRepository.Setup(r => r.GetMpKioskConfigByIdentifier(It.IsAny<Guid>())).Returns(mpKioskConfigDto);
            _familyService.Setup(r => r.CreateNewFamily(It.IsAny<string>(), It.IsAny<List<NewParentDto>>(), It.IsAny<string>()))
                .Returns(new List<ContactDto>());

            _fixture.Request.Headers.Add("Crds-Kiosk-Identifier", Guid.NewGuid().ToString());


            // Act
            try
            {
                var result = _fixture.CreateNewFamily(newParentDtos);
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(ex);
                return;
            }
            
            // fail the test if an exception is not thrown
            Assert.AreEqual(1, 2);
        }

        
    }
}
