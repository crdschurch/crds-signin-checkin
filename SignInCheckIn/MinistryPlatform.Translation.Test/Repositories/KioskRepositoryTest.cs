using System;
using System.Collections.Generic;
using System.Linq;
using Crossroads.Web.Common.MinistryPlatform;
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
        private List<string> _printerMapColumns;

        [SetUp]
        public void SetUp()
        {
            _apiUserRepository = new Mock<IApiUserRepository>(MockBehavior.Strict);
            _ministryPlatformRestRepository = new Mock<IMinistryPlatformRestRepository>(MockBehavior.Strict);

            _kioskConfigColumns = new List<string>
            {
                "[Kiosk_Config_ID]",
                "[_Kiosk_Identifier]",
                "[Kiosk_Name]",
                "[Kiosk_Description]",
                "[Kiosk_Type_ID]",
                "cr_Kiosk_Configs.[Location_ID]",
                "cr_Kiosk_Configs.[Congregation_ID]",
                "Congregation_ID_Table.[Congregation_Name]",
                "cr_Kiosk_Configs.[Room_ID]",
                "Room_ID_Table.Room_Name",
                "cr_Kiosk_Configs.[Start_Date]",
                "cr_Kiosk_Configs.[End_Date]",
                "Printer_Map_ID"
            };

            _printerMapColumns = new List<string>
            {
                "[Printer_Map_ID]",
                "[Printer_ID]",
                "[Printer_Name]",
                "[Computer_ID]",
                "[Computer_Name]"
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
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpKioskConfigDto>($"[_Kiosk_Identifier]='{testGuid}' AND cr_Kiosk_Configs.[End_Date] IS NULL", _kioskConfigColumns, null, false)).Returns(kioskConfigs);
            var result = _fixture.GetMpKioskConfigByIdentifier(testGuid);
            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();
            result.Should().BeSameAs(kioskConfigs.First());
        }


        [Test]
        public void TestGetKioskPrinterMapById()
        {
            int printerMapId = 1234567;
            const string token = "tok 123";

            var mpPrinterMapDtos = new List<MpPrinterMapDto>();

            var mpPrinterMapDto = new MpPrinterMapDto
            {
                PrinterMapId = 1234567
            };

            mpPrinterMapDtos.Add(mpPrinterMapDto);

            _apiUserRepository.Setup(mocked => mocked.GetToken()).Returns(token);
            _ministryPlatformRestRepository.Setup(mocked => mocked.UsingAuthenticationToken(token)).Returns(_ministryPlatformRestRepository.Object);
            _ministryPlatformRestRepository.Setup(mocked => mocked.Search<MpPrinterMapDto>($"[Printer_Map_ID]={printerMapId}", _printerMapColumns, null, false)).Returns(mpPrinterMapDtos);
            var result = _fixture.GetPrinterMapById(printerMapId);
            _apiUserRepository.VerifyAll();
            _ministryPlatformRestRepository.VerifyAll();
            result.Should().BeSameAs(mpPrinterMapDto);
        }
    }
}
