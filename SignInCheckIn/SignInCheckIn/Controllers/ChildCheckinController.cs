using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using SignInCheckIn.ApiVersioning.Attributes;
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
        [ResponseType(typeof(ParticipantEventMapDto))]
        [Route("checkin/children/{roomId:int}")]
        public IHttpActionResult GetCheckedInChildrenForEventAndRoom(int roomId, [FromUri(Name = "eventId")] int? eventId = null)
        {
            try
            {
                var siteId = 0;
                if (Request.Headers.Contains("Crds-Site-Id"))
                {
                    siteId = int.Parse(Request.Headers.GetValues("Crds-Site-Id").First());
                }

                if (siteId == 0 && eventId == null)
                {
                    throw new Exception("Site Id or Event Id is required");
                }

                var children = _childCheckinService.GetChildrenForCurrentEventAndRoom(roomId, siteId, eventId);
                return Ok(children);
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto("Get Children", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }

        [HttpPut]
        [ResponseType(typeof(ParticipantEventMapDto))]
        [Route("checkin/event/participant")]
        public IHttpActionResult CheckinChildrenForCurrentEventAndRoom(ParticipantDto participant)
        {
            try
            {
                var child = _childCheckinService.CheckinChildrenForCurrentEventAndRoom(participant);
                return Ok(child);
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto("Checking in Children", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }

        [HttpGet]
        [ResponseType(typeof(ParticipantDto))]
        [Route("checkin/events/{eventId}/child/{callNumber}/rooms/{roomId}")]
        public IHttpActionResult GetEventParticipantByCallNumber(
             [FromUri(Name = "eventId")] int eventId,
             [FromUri(Name = "callNumber")] int callNumber,
             [FromUri(Name = "roomId")] int roomId)
        {
            try
            {
                var child = _childCheckinService.GetEventParticipantByCallNumber(eventId, callNumber, roomId, true);
                if (child != null)
                {
                    return Ok(child);
                }
                else
                {
                    return NotFound();
                }
                
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto("Get Event Participant by Call Number", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }

        [HttpPut]
        [Route("checkin/events/{eventId}/child/{eventParticipantId}/rooms/{roomId}/override")]
        public IHttpActionResult OverrideChildIntoRoom(
             [FromUri(Name = "eventId")] int eventId,
             [FromUri(Name = "eventParticipantId")] int eventParticipantId,
             [FromUri(Name = "roomId")] int roomId)
        {
            try
            {
                _childCheckinService.OverrideChildIntoRoom(eventId, eventParticipantId, roomId);
                return Ok();
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto("Checking in Children", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }
    }
}
