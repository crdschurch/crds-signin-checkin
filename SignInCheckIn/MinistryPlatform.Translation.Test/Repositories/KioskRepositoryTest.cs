using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using NUnit.Framework;

namespace MinistryPlatform.Translation.Test.Repositories
{
    public class KioskRepositoryTest
    {
        private KioskRepository _fixture;

        private Mock<IApiUserRepository> _apiUserRepository;
        private Mock<IMinistryPlatformRestRepository> _ministryPlatformRestRepository;
        private List<string> _kioskConfigColumns;

        [SetUp]
        public void SetUp()
        {
            _apiUserRepository = new Mock<IApiUserRepository>(MockBehavior.Strict);
            _ministryPlatformRestRepository = new Mock<IMinistryPlatformRestRepository>(MockBehavior.Strict);

            _kioskConfigColumns = new List<string>
            {
                "[Kiosk_Config_ID]",
                "[_Kiosk_IDentifier]",
                "[Kiosk_Name]",
                "[Kiosk_Description]",
                "[Kiosk_Type_ID]",
                "[Location_ID]",
                "[Congregation_ID]",
                "cr_Kiosk_Configs.[Room_ID]",
                "Room_ID_Table.Room_Name",
                "[Start_Date]",
                "[End_Date]"
            };

            _fixture = new KioskRepository(_apiUserRepository.Object, _ministryPlatformRestRepository.Object);
        }

        [Test]
        public void TestGetKioskConfigsForIdentifier()
        {
            var testGuid = Guid.Parse("9c01a404-3ba8-4f4a-b7b1-2183e30addd1");
            const string token = "tok 123";
            var kioskConfigs = new List<MpKioskConfigDto>();

            var testDto = new MpKioskConfigDto
            {
                KioskIdentifier = testGuid
            };

            kioskConfigs.Add(testDto);

            _apiUserRepository.Setup(mocked => mocked.GetToken()).Returns(token);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpKioskConfigDto>($"[_Kiosk_Identifier]='{testGuid}' AND [End_Date] IS NULL", _kioskConfigColumns)).Returns(kioskConfigs);
            var result = _fixture.GetMpKioskConfigByIdentifier(testGuid);
            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();
            result.Should().BeSameAs(kioskConfigs.First());
        }
    }
}
