using System;
using System.Collections.Generic;
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
        [ResponseType(typeof(List<ParticipantDto>))]
        [Route("signin/children/{phoneNumber}")]
        public IHttpActionResult GetEvents(string phoneNumber)
        {
            try
            {
                var children = _childSigninService.GetChildrenByPhoneNumber(phoneNumber);
                return Ok(children);
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto("Get Children", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }
    }
}
