using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using MinistryPlatform.Translation.Repositories.Interfaces;
using SignInCheckIn.Exceptions.Models;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Security;
using SignInCheckIn.Services.Interfaces;

namespace SignInCheckIn.Controllers
{
    public class RoomController : MpAuth
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService, IAuthenticationRepository authenticationRepository) : base(authenticationRepository)
        {
            _roomService = roomService;
        }

        [HttpGet]
        [ResponseType(typeof(List<RoomDto>))]
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
                    return Ok(_roomService.CreateOrUpdateEventRoom(token, eventRoom));
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
