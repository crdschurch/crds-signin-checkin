using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using crds_angular.Security;
using MinistryPlatform.Translation.Repositories.Interfaces;
using SignInCheckIn.Exceptions.Models;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Services.Interfaces;

namespace SignInCheckIn.Controllers
{
    public class EventController : MPAuth
    {
        private readonly IEventService _eventService;
        private readonly IRoomService _roomService;

        public EventController(IEventService eventService, IRoomService roomService, IAuthenticationRepository authenticationRepository) : base(authenticationRepository)
        {
            _eventService = eventService;
            _roomService = roomService;
        }

        [HttpGet]
        [ResponseType(typeof(List<EventDto>))]
        [Route("events")]
        public IHttpActionResult GetEvents()
        {
            try
            {
                var eventList = _eventService.GetCheckinEvents();
                return this.Ok(eventList);
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto("Get Events", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }

        [HttpGet]
        [ResponseType(typeof(List<EventRoomDto>))]
        [Route("events/{eventid}/rooms")]
        public IHttpActionResult GetRoomsByEvent(int eventid)
        {
            try
            {
                var roomList = _roomService.GetLocationRoomsByEventId(eventid);
                return this.Ok(roomList);
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto("Get Room for Event " + eventid, e);
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
    }
}
