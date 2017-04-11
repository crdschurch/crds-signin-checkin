using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Script.Serialization;
using SignInCheckIn.Exceptions.Models;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Services.Interfaces;
using Crossroads.ApiVersioning;
using Crossroads.Utilities.Services.Interfaces;
using Crossroads.Web.Common.Security;
using Newtonsoft.Json.Linq;
using SignInCheckIn.Security;

namespace SignInCheckIn.Controllers
{
    public class ChildCheckinController : MpAuth
    {
        private readonly IChildCheckinService _childCheckinService;
        private readonly IWebsocketService _websocketService;
        private readonly IApplicationConfiguration _applicationConfiguration;

        public ChildCheckinController(IChildCheckinService childCheckinService, IApplicationConfiguration applicationConfiguration, IAuthenticationRepository authenticationRepository, IWebsocketService websocketService) : base(authenticationRepository)
        {
            _childCheckinService = childCheckinService;
            _applicationConfiguration = applicationConfiguration;
            _websocketService = websocketService;
        }

        [HttpGet]
        [ResponseType(typeof(ParticipantEventMapDto))]
        [VersionedRoute(template: "checkin/children/{roomId:int}", minimumVersion: "1.0.0")]
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

                string kioskId = "";

                if (Request.Headers.Contains("Crds-Kiosk-Identifier"))
                {
                    kioskId = Request.Headers.GetValues("Crds-Kiosk-Identifier").First();
                }

                var children = _childCheckinService.GetChildrenForCurrentEventAndRoom(roomId, siteId, eventId, kioskId);
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
        [VersionedRoute(template: "checkin/event/{eventId}/participant", minimumVersion: "1.0.0")]
        [Route("checkin/event/{eventId}/participant")]
        public IHttpActionResult CheckinChildrenForCurrentEventAndRoom([FromUri(Name = "eventId")] int eventId, ParticipantDto participant)
        {
            try
            {
                var child = _childCheckinService.CheckinChildrenForCurrentEventAndRoom(participant);
                _websocketService.PublishCheckinParticipantsCheckedIn(eventId, participant.AssignedRoomId.Value, child);
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
        [VersionedRoute(template: "checkin/events/{eventId}/child/{callNumber}/rooms/{roomId}", minimumVersion: "1.0.0")]
        [Route("checkin/events/{eventId}/child/{callNumber}/rooms/{roomId}")]
        public IHttpActionResult GetEventParticipantByCallNumber(
             [FromUri(Name = "eventId")] int eventId,
             [FromUri(Name = "callNumber")] int callNumber,
             [FromUri(Name = "roomId")] int roomId)
        {
            try
            {
                var child = _childCheckinService.GetEventParticipantByCallNumber(eventId, callNumber, roomId, true);
                return Ok(child);
            }
            catch (Exception e)
            {
                return NotFound();
            }
        }

        [HttpPut]
        [VersionedRoute(template: "checkin/events/{eventId}/child/{eventParticipantId}/rooms/{roomId}/override/{overRideRoomId}", minimumVersion: "1.0.0")]
        [Route("checkin/events/{eventId}/child/{eventParticipantId}/rooms/{roomId}/override/{overRideRoomId}")]
        public IHttpActionResult OverrideChildIntoRoom(
             [FromUri(Name = "eventId")] int eventId,
             [FromUri(Name = "eventParticipantId")] int eventParticipantId,
             [FromUri(Name = "roomId")] int roomId,
             [FromUri(Name = "overRideRoomId")] int overRideRoomId)
        {
            try
            {
                _childCheckinService.OverrideChildIntoRoom(eventId, eventParticipantId, overRideRoomId);

                //Publish the removal of the kid from the original room
                var data = new ParticipantDto();
                data.EventParticipantId = eventParticipantId;
                data.OriginalRoomId = roomId;
                data.OverRideRoomId = overRideRoomId;

                _websocketService.PublishCheckinParticipantsRemove(eventId, roomId, data);
                _websocketService.PublishCheckinParticipantsOverrideCheckin(eventId, overRideRoomId, data);

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
