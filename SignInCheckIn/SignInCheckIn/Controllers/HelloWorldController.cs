using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using SignInCheckIn.Models;
using SignInCheckIn.Services.Interfaces;
//using Crossroads.ApiVersioning;

namespace SignInCheckIn.Controllers
{
    [RoutePrefix("api")]
    public class HelloWorldController : ApiController
    {
        private readonly IHelloWorldService _helloWorldService;
        public HelloWorldController(IHelloWorldService helloWorldService)
        {
            _helloWorldService = helloWorldService;
        }

        [HttpPost]
        [ResponseType(typeof(HelloWorldOutputDto))]
        //[VersionedRoute(template: "hello/greet", minimumVersion: "1.0.0", maximumVersion: "2.0.0", deprecated: true)]
        [Route("hello/greet")]
        public IHttpActionResult Greet([FromBody] HelloWorldInputDto input)
        {
            if (input == null)
            {
                ModelState.Add(new KeyValuePair<string, ModelState>("input", new ModelState()));
                ModelState["input"].Errors.Add("input cannot be null");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(_helloWorldService.Greet(input));
        }

        [HttpPost]
        [ResponseType(typeof(HelloWorldOutputDto))]
        //[VersionedRoute("hello/greet", "2.0.0")]
        public IHttpActionResult Greet([FromBody] HelloWorldV2InputDto input)
        {
            if (input == null)
            {
                ModelState.Add(new KeyValuePair<string, ModelState>("input", new ModelState()));
                ModelState["input"].Errors.Add("input cannot be null");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(input.FirstName))
                {
                    ModelState.Add(new KeyValuePair<string, ModelState>("input.FirstName", new ModelState()));
                    ModelState["input.FirstName"].Errors.Add("FirstName is required");
                }
                if (string.IsNullOrWhiteSpace(input.LastName))
                {
                    ModelState.Add(new KeyValuePair<string, ModelState>("input.LastName", new ModelState()));
                    ModelState["input.LastName"].Errors.Add("LastName is required");
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(_helloWorldService.Greet(input));
        }
    }
}