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
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json.Linq;
using SignInCheckIn.Hubs;
using SignInCheckIn.Security;

namespace SignInCheckIn.Controllers
{
    public class ChildCheckinController : MpAuth
    {
        private readonly IChildCheckinService _childCheckinService;
        private readonly IHubContext _context;
        private readonly IApplicationConfiguration _applicationConfiguration;

        public ChildCheckinController(IChildCheckinService childCheckinService, IApplicationConfiguration applicationConfiguration, IAuthenticationRepository authenticationRepository) : base(authenticationRepository)
        {
            _context = GlobalHost.ConnectionManager.GetHubContext<EventHub>();
            _childCheckinService = childCheckinService;
            _applicationConfiguration = applicationConfiguration;
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
        [VersionedRoute(template: "checkin/event/participant", minimumVersion: "1.0.0")]
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

                dynamic data = new JObject();
                data.EventParticipantId = eventParticipantId;
                data.OriginalRoomId = roomId;
                data.OverRideRoomId = overRideRoomId;

                PublishToChannel(_context, new ChannelEvent
                {
                    ChannelName = $"{_applicationConfiguration.CheckinParticipantsChannel}{eventId}{roomId}",
                    Name = "Remove",
                    Data = data
                });
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
