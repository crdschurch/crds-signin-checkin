using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Printing.Utilities.Services.Interfaces;
using RestSharp;
using System.Configuration;
using System.Runtime.InteropServices.WindowsRuntime;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Printing.Utilities.Models;
using Printing.Utilities.Serializers;
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
            //var request = new RestSharp.RestRequest("https://api.printnode.com/printjobs", RestSharp.Method.POST) { RequestFormat = RestSharp.DataFormat.Json }
            //    .AddBody(printRequestDto);
            //var request = new RestRequest("/printjobs", Method.POST);
            var request = new RestRequest("https://api.printnode.com/printjobs", Method.POST);

            //var x = Jso
            //request.RequestFormat = DataFormat.Json;
            var x = Newtonsoft.Json.JsonConvert.SerializeObject(printRequestDto);
            //request.AddParameter("body", x);
            //request.AddJsonBody(printRequestDto);

            request.AddParameter("content", printRequestDto.content);

            // TODO: verify this is properly deserializing
            //var response = _printingRestClient.Execute<int>(request);
            var response = _printingRestClient.Execute(request);
            //return response.Data;
            return int.Parse(response.Content);
        }
    }
}
