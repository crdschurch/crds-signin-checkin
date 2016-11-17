using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Printing.Utilities.Services.Interfaces;
using RestSharp;
using System.Configuration;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Printing.Utilities.Models;
using RestSharp.Authenticators;

namespace Printing.Utilities.Services
{
    public class PrintNodeService : IPrintingService
    {
        private string _printingUrl;
        private string _printingApiKey;

        private readonly IRestClient _printingRestClient;

        public PrintNodeService(IConfigRepository configRepository, IRestClient printingRestClient)
        {
            _printingApiKey = configRepository.GetMpConfigByKey("PrinterAPIKey").Value;

            _printingRestClient = printingRestClient;

            // printnode only asks for the api key in the username, no pw needed
            _printingRestClient.Authenticator = new HttpBasicAuthenticator(_printingApiKey, String.Empty);
        }

        public int SendPrintRequest(PrintRequestDto printRequestDto)
        {
            var request = new RestRequest("/printjobs", Method.POST);

            request.AddJsonBody(printRequestDto);

            // TODO: verify this is properly deserializing
            var response = _printingRestClient.Execute<int>(request);
            return response.Data;
        }
    }
}
