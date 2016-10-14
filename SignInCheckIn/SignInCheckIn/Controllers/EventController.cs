using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Description;
using log4net;
using SignInCheckIn.Exceptions.Models;
using SignInCheckIn.Models.Authentication;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Models.Json;
using SignInCheckIn.Services.Interfaces;

namespace SignInCheckIn.Controllers
{
    public class EventController : ApiController
    {
        private readonly IEventService _eventService;
        private readonly IRoomService _roomService;

        public EventController(IEventService eventService, IRoomService roomService)
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
    }
}
