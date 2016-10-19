using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace SignInCheckIn.Exceptions.Models
{
    public class ApiErrorDto
    {
        private readonly HttpStatusCode code;

        public ApiErrorDto()
        {
        }

        public ApiErrorDto(string message, Exception exception = null, HttpStatusCode code = HttpStatusCode.BadRequest, List<string> errors = null)
        {
            this.code = code;
            this.Message = message;

            if (exception == null) return;
            var privateErrors = errors ?? new List<string>();
            privateErrors.Add(exception.Message);
            if (exception.InnerException != null)
            {
                privateErrors.Add(exception.InnerException.Message);
            }
            this.Errors = privateErrors;
        }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "errors", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Errors { get; set; }

        [JsonIgnore]
        public HttpResponseMessage HttpResponseMessage
        {

            get
            {
                var json = JsonConvert.SerializeObject(this, Formatting.Indented);
                var resp = new HttpResponseMessage(code) { Content = new StringContent(json) };
                return resp;
            }
        }
    }
}