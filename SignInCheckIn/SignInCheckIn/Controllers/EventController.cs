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

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        [ResponseType(typeof(List<EventDto>))]
        [Route("checkinevents")]
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
    }
}
