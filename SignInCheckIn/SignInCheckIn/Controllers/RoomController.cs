using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
            try
            {
                var roomList = _roomService.GetAvailableRooms(roomId, eventId);
                return Ok(roomList);
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto("Get Rooms By Location ", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }

        [HttpPut]
        [ResponseType(typeof(List<RoomDto>))]
        [Route("events/{eventId}/rooms/{roomId}/bumping")]
        public IHttpActionResult UpdateAvailableRoomsByLocation(
        [FromUri(Name = "eventId")] int eventId, [FromUri(Name = "roomId")] int roomId)
        {
            try
            {
                //var roomList = _roomService.GetAvailableRooms(locationId);
                //return Ok(roomList);
                return Ok();
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto("Get Rooms By Location ", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }
    }
}
