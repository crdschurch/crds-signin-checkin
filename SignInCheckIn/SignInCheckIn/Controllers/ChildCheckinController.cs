using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using SignInCheckIn.Exceptions.Models;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Services.Interfaces;

namespace SignInCheckIn.Controllers
{
    public class ChildCheckinController : ApiController
    {
        private readonly IChildCheckinService _childCheckinService;

        public ChildCheckinController(IChildCheckinService childCheckinService)
        {
            _childCheckinService = childCheckinService;
        }

        [HttpGet]
        [ResponseType(typeof(List<ParticipantDto>))]
        [Route("checkin/children/{phoneNumber}")]
        public IHttpActionResult GetEvents(string phoneNumber)
        {
            try
            {
                var children = _childCheckinService.GetChildrenByPhoneNumber(phoneNumber);
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
