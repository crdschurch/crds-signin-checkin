using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using NUnit.Framework;
using SignInCheckIn.App_Start;
using SignInCheckIn.Services;

namespace SignInCheckIn.Tests.Services
{
    public class KioskServiceTest
    {
        private Mock<IKioskRepository> _kioskRepository;

        private KioskService _fixture;

        [SetUp]
        public void SetUp()
        {
            AutoMapperConfig.RegisterMappings();

            _kioskRepository = new Mock<IKioskRepository>();
            _fixture = new KioskService(_kioskRepository.Object);
        }

        [Test]
        public void TestGetKioskConfig()
        {
            Guid testGuid = Guid.Parse("9c01a404-3ba8-4f4a-b7b1-2183e30addd1");

            var e = new MpKioskConfigDto
            {
                KioskIdentifier = testGuid
            };

            _kioskRepository.Setup(mocked => mocked.GetMpKioskConfigByIdentifier(It.IsAny<Guid>())).Returns(e);
            var result = _fixture.GetKioskConfigByIdentifier(testGuid);
            _kioskRepository.VerifyAll();
            Assert.AreEqual(e.KioskIdentifier, result.KioskIdentifier);
        }
    }
}
