using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using SignInCheckIn.Exceptions.Models;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Services.Interfaces;

namespace SignInCheckIn.Controllers
{
    public class ChildSigninController : ApiController
    {
        private readonly IChildSigninService _childSigninService;

        public ChildSigninController(IChildSigninService childSigninService)
        {
            _childSigninService = childSigninService;
        }

        [HttpGet]
        [ResponseType(typeof(ParticipantEventMapDto))]
        [Route("signin/children/{phoneNumber}")]
        public IHttpActionResult GetChildrenAndEvent(string phoneNumber)
        {
            try
            {
                IEnumerable<string> headerValues;
                int siteId = 0;
                if (Request.Headers.TryGetValues("Site_Id", out headerValues))
                {
                    siteId = Int32.Parse(headerValues.FirstOrDefault());
                }

                if (siteId == 0)
                {
                    throw new Exception("Site Id is Invalid");
                }

                var children = _childSigninService.GetChildrenAndEventByPhoneNumber(phoneNumber, siteId);
                return Ok(children);
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto("Get Children", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }

        [HttpPost]
        //[ResponseType(typeof(List<ParticipantDto>))]
        [Route("signin/children")]
        public IHttpActionResult SigninParticipants(ParticipantEventMapDto participantEventMapDto)
        {
            try
            {
                _childSigninService.SigninParticipants(participantEventMapDto);
                return Ok();
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto("Sign In Participants", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }
    }
}
