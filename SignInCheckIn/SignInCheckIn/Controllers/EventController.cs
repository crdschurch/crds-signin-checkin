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
    public class EventController : MpAuth
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
        public IHttpActionResult GetEvents(
            [FromUri(Name = "startDate")] DateTime startDate,
            [FromUri(Name = "endDate")] DateTime endDate,
            [FromUri(Name = "site")] int site )
        {
            try
            {
                var eventList = _eventService.GetCheckinEvents(startDate, endDate, site);
                return Ok(eventList);
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto("Get Events", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }

        [HttpGet]
        [ResponseType(typeof (EventRoomDto))]
        [Route("events/{eventid}")]
        public IHttpActionResult GetEvent([FromUri] int eventId)
        {
            try
            {
                return Ok(_eventService.GetEvent(eventId));
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto($"Could not get event by ID {eventId}", e);
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
                return Ok(roomList);
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

        [HttpGet]
        [ResponseType(typeof (EventRoomDto))]
        [Route("events/{eventId:int}/rooms/{roomId:int}/groups")]
        public IHttpActionResult GetEventRoomAgesAndGrades([FromUri] int eventId, [FromUri] int roomId)
        {
            try
            {
                return Authorized(token => Ok(_roomService.GetEventRoomAgesAndGrades(token, eventId, roomId)),
                                  () => Ok(_roomService.GetEventRoomAgesAndGrades(null, eventId, roomId)));
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto($"Error getting ages and grades for event {eventId}, room {roomId}", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }

        [HttpPut]
        [ResponseType(typeof(EventRoomDto))]
        [Route("events/{eventId:int}/rooms/{roomId:int}/groups")]
        public IHttpActionResult UpdateEventRoomAgesAndGrades([FromUri] int eventId, [FromUri] int roomId, [FromBody] EventRoomDto eventRoom)
        {
            try
            {
                return Authorized(token => Ok(_roomService.UpdateEventRoomAgesAndGrades(token, eventId, roomId, eventRoom)));
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto($"Error updating ages and grades for event {eventId}, room {roomId}", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }

        [HttpPut]
        [ResponseType(typeof(List<EventRoomDto>))]
        [Route("events/{destinationEventId:int}/import/{sourceEventId:int}")]
        public IHttpActionResult ImportEventSetup(int destinationEventId, int sourceEventId)
        {
            try
            {
                return Authorized(token => Ok(_eventService.ImportEventSetup(token, destinationEventId, sourceEventId)));
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto($"Error importing source event #{sourceEventId} into destination event #{destinationEventId}", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }

        [HttpPut]
        [ResponseType(typeof(List<EventRoomDto>))]
        [Route("events/{destinationEventId:int}/reset")]
        public IHttpActionResult ResetEventSetup(int eventId)
        {
            try
            {
                return Authorized(token => Ok(_eventService.ResetEventSetup(token, eventId)));
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto($"Error resetting event #{eventId}", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }

        [HttpGet]
        [ResponseType(typeof(EventRoomDto))]
        [Route("events/{eventId:int}/maps")]
        public IHttpActionResult GetEventsAndSubEvents([FromUri] int eventId)
        {
            try
            {
                return Authorized(token => Ok(_eventService.GetEventMaps(token, eventId)));
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto($"Error getting event maps for event {eventId}", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }

        [HttpGet]
        [ResponseType(typeof(ParticipantDto))]
        [Route("events/{eventId:int}/children")]
        public IHttpActionResult GetChildrenForEvent([FromUri] int eventId)
        {
            try
            {
                return Authorized(token => Ok(_eventService.GetListOfChildrenForEvent(token, eventId)));
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto($"Error getting event maps for event {eventId}", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }
    }
}
