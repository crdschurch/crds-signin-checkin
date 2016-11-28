using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Moq;
using NUnit.Framework;
using Printing.Utilities.Models;
using Printing.Utilities.Services;
using Printing.Utilities.Services.Interfaces;
using RestSharp;

namespace Printing.Utilities.Tests.Services
{
    public class PrintNodeServiceTest
    {
        private Mock<IRestClient> _printingRestClient;
        private Mock<IConfigRepository> _configRepository;

        private IPrintingService _fixture;

        private MpConfigDto mpConfigDto;

        [SetUp]
        public void SetUp()
        {
            _configRepository = new Mock<IConfigRepository>();
            _printingRestClient = new Mock<IRestClient>();

            mpConfigDto = new MpConfigDto
            {
                Value = "abcde12345"
            };

            _configRepository.Setup(m => m.GetMpConfigByKey(It.IsAny<string>())).Returns(mpConfigDto);

            _fixture = new PrintNodeService(_configRepository.Object, _printingRestClient.Object);
        }

        [Test]
        public void ShouldCallPrintNode()
        {
            // Arrange
            string configKey = "PrinterAPIKey";
            string printingApiKey = "abcde12345";
            int printJobId = 123;

            PrintRequestDto printRequestDto = new PrintRequestDto();

            RestResponse<int> restReponse = new RestResponse<int>
            {
                Content = "123"
            };

            _printingRestClient.Setup(m => m.Execute<int>(It.IsAny<IRestRequest>())).Returns(restReponse);

            // Act
            _fixture.SendPrintRequest(printRequestDto);

            // Assert
            _configRepository.VerifyAll();
            _printingRestClient.VerifyAll();
            Assert.AreEqual(mpConfigDto.Value, printingApiKey);
            Assert.AreEqual(printJobId.ToString(), restReponse.Content);
        }
    }
}
