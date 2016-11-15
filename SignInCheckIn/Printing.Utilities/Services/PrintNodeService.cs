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

        public PrintNodeService(IConfigRepository configRepository)
        {
            _printingUrl = ConfigurationManager.AppSettings["PrintingURL"];
            _printingApiKey = configRepository.GetMpConfigByKey("PrinterAPIKey").Value;
        }

        public int SendPrintRequest(PrintRequestDto printRequestDto)
        {
            var client = new RestClient(_printingUrl);

            // printnode only asks for the api key in the username, no pw needed
            client.Authenticator = new HttpBasicAuthenticator(_printingApiKey, String.Empty);

            //var request = new RestRequest("resource/{id}", Method.POST);
            var request = new RestRequest("/printjobs", Method.POST);

            request.AddJsonBody(printRequestDto);

            //request.AddParameter("name", "value"); // adds to POST or URL querystring based on Method
            //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

            //// easily add HTTP Headers
            //request.AddHeader("header", "value");

            //// execute the request
            //IRestResponse response = client.Execute(request);
            //var content = response.Content; // raw content as string

            // or automatically deserialize result
            // return content type is sniffed but can be explicitly set via RestClient.AddHandler();
            //RestResponse<int> response = client.Execute<int>(request);

            // TODO: verify this is properly deserializing
            var response = client.Execute<int>(request);
            return response.Data;
        }
    }
}
