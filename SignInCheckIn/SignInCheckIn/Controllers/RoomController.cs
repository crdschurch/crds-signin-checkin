using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using Crossroads.Utilities.Services.Interfaces;
using Microsoft.AspNet.SignalR;
using MinistryPlatform.Translation.Repositories.Interfaces;
using SignInCheckIn.Exceptions.Models;
using SignInCheckIn.Hubs;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Security;
using SignInCheckIn.Services.Interfaces;
using Crossroads.ApiVersioning;
using Crossroads.Web.Common.Security;

namespace SignInCheckIn.Controllers
{
    public class RoomController : MpAuth
    {
        private readonly IRoomService _roomService;
        private readonly IHubContext _context;
        private readonly IApplicationConfiguration _applicationConfiguration;

        public RoomController(IRoomService roomService, IAuthenticationRepository authenticationRepository, IApplicationConfiguration applicationConfiguration) : base(authenticationRepository)
        {
            _roomService = roomService;
            _context = GlobalHost.ConnectionManager.GetHubContext<EventHub>();
            _applicationConfiguration = applicationConfiguration;
        }

        [HttpGet]
        [ResponseType(typeof(List<RoomDto>))]
        [VersionedRoute(template: "events/{eventId}/rooms/{roomId}/bumping", minimumVersion: "1.0.0")]
        [Route("events/{eventId}/rooms/{roomId}/bumping")]
        public IHttpActionResult GetAvailableRooms(
            [FromUri(Name = "roomId")] int roomId,
            [FromUri(Name = "eventId")] int eventId)
        {
            return Authorized(token =>
            {
                try
                {
                    var roomList = _roomService.GetAvailableRooms(token, roomId, eventId);
                    return Ok(roomList);
                }
                catch (Exception e)
                {
                    var apiError = new ApiErrorDto("Get Rooms By Location ", e);
                    throw new HttpResponseException(apiError.HttpResponseMessage);
                }
            });
        }

        [HttpPost]
        [ResponseType(typeof(List<RoomDto>))]
        [VersionedRoute(template: "events/{eventId}/rooms/{roomId}/bumping", minimumVersion: "1.0.0")]
        [Route("events/{eventId}/rooms/{roomId}/bumping")]
        public IHttpActionResult UpdateAvailableRoomsByLocation(
        [FromUri(Name = "eventId")] int eventId, [FromUri(Name = "roomId")] int roomId, [FromBody] List<EventRoomDto> eventRooms)
        {
            return Authorized(token =>
            {
                try
                {
                    var roomList = _roomService.UpdateAvailableRooms(token, eventId, roomId, eventRooms);
                    return Ok(roomList);
                }
                catch (Exception e)
                {
                    var apiError = new ApiErrorDto("Get Rooms By Location ", e);
                    throw new HttpResponseException(apiError.HttpResponseMessage);
                }
            });
        }

        [HttpGet]
        [ResponseType(typeof(List<RoomDto>))]
        [VersionedRoute(template: "grade-groups", minimumVersion: "1.0.0")]
        [Route("grade-groups")]
        public IHttpActionResult GetGradeGroups()
        {
            try
            {
                var roomList = _roomService.GetGradeAttributes(null);
                return Ok(roomList);
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto("Get Rooms By Location ", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }

        [HttpPut]
        [ResponseType(typeof(EventRoomDto))]
        [Route("events/{eventId:int}/rooms/{roomId:int}")]
        public IHttpActionResult UpdateEventRoom([FromUri]int eventId, [FromUri]int roomId, [FromBody]EventRoomDto eventRoom)
        {
            return Authorized(token =>
            {
                try
                {
                    eventRoom.EventId = eventId;
                    eventRoom.RoomId = roomId;

                    var updatedEventRoom = _roomService.CreateOrUpdateEventRoom(token, eventRoom);

                    PublishToChannel(_context, new ChannelEvent {
                        ChannelName = $"{_applicationConfiguration.CheckinCapacityChannel}{eventId}{roomId}",
                        Data = updatedEventRoom
                    });

                    return Ok(updatedEventRoom);
                }
                catch (Exception e)
                {
                    var apiError = new ApiErrorDto($"Error updating event room for event {eventId}, room {roomId}", e);
                    throw new HttpResponseException(apiError.HttpResponseMessage);
                }
            });
        }

        [HttpGet]
        [ResponseType(typeof(EventRoomDto))]
        [Route("events/{eventId:int}/rooms/{roomId:int}")]
        public IHttpActionResult GetEventRoom([FromUri] int eventId, [FromUri] int roomId)
        {
            try
            {
                var room = _roomService.GetEventRoom(eventId, roomId);
                return Ok(room);
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto("Get Rooms By Location ", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }
    }
}
