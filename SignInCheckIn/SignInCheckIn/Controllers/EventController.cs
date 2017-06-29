using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using SignInCheckIn.Exceptions.Models;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Security;
using SignInCheckIn.Services.Interfaces;
using Crossroads.ApiVersioning;
using Crossroads.Web.Common.Security;

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
        [VersionedRoute(template: "events", minimumVersion: "1.0.0")]
        [Route("events")]
        public IHttpActionResult GetEvents(
            [FromUri(Name = "startDate")] DateTime startDate,
            [FromUri(Name = "endDate")] DateTime endDate,
            [FromUri(Name = "site")] int site )
        {
            try
            {
                string kioskIdentifier = "";
                if (Request.Headers.Contains("Crds-Kiosk-Identifier"))
                {
                    kioskIdentifier = Request.Headers.GetValues("Crds-Kiosk-Identifier").First();
                }

                var eventList = _eventService.GetCheckinEvents(startDate, endDate, site, kioskIdentifier);
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
        [VersionedRoute(template: "events/{eventid}", minimumVersion: "1.0.0")]
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
        [VersionedRoute(template: "events/{eventid}/rooms", minimumVersion: "1.0.0")]
        [Route("events/{eventid}/rooms")]
        [RequiresAuthorization]
        public IHttpActionResult GetRoomsByEvent(int eventid)
        {
            return Authorized(token =>
            {
                try
                {
                    var roomList = _roomService.GetLocationRoomsByEventId(token, eventid);
                    return Ok(roomList);
                }
                catch (Exception e)
                {
                    var apiError = new ApiErrorDto("Get Room for Event " + eventid, e);
                    throw new HttpResponseException(apiError.HttpResponseMessage);
                }
            });
        }

        [HttpGet]
        [ResponseType(typeof (EventRoomDto))]
        [VersionedRoute(template: "events/{eventId:int}/rooms/{roomId:int}/groups", minimumVersion: "1.0.0")]
        [Route("events/{eventId:int}/rooms/{roomId:int}/groups")]
        [RequiresAuthorization]
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

        [HttpGet]
        [ResponseType(typeof(EventRoomDto))]
        [VersionedRoute(template: "events/{eventId:int}/groups/unassigned", minimumVersion: "1.0.0")]
        [Route("events/{eventId:int}/groups/unassigned")]
        [RequiresAuthorization]
        public IHttpActionResult GetEventUnassignedGroups([FromUri] int eventId)
        {
            try
            {
                return Authorized(token => Ok(_roomService.GetEventUnassignedGroups(token, eventId)));
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto($"Error getting unassigned groups for event {eventId}", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }

        [HttpPut]
        [ResponseType(typeof(EventRoomDto))]
        [VersionedRoute(template: "events/{eventId:int}/rooms/{roomId:int}/groups", minimumVersion: "1.0.0")]
        [Route("events/{eventId:int}/rooms/{roomId:int}/groups")]
        [RequiresAuthorization]
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
        [VersionedRoute(template: "events/{destinationEventId:int}/import/{sourceEventId:int}", minimumVersion: "1.0.0")]
        [Route("events/{destinationEventId:int}/import/{sourceEventId:int}")]
        [RequiresAuthorization]
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
        [VersionedRoute(template: "events/{eventId:int}/reset", minimumVersion: "1.0.0")]
        [Route("events/{eventId:int}/reset")]
        [RequiresAuthorization]
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
        [VersionedRoute(template: "events/{eventId:int}/maps", minimumVersion: "1.0.0")]
        [Route("events/{eventId:int}/maps")]
        [RequiresAuthorization]
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
        [VersionedRoute(template: "events/{eventId:int}/children", minimumVersion: "1.0.0")]
        [Route("events/{eventId:int}/children")]
        public IHttpActionResult GetChildrenForEvent([FromUri] int eventId, [FromUri] string search = null)
        {
            try
            {

                return Authorized(token => Ok(_eventService.GetListOfChildrenForEvent(token, eventId, search)));
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto($"Error getting event maps for event {eventId}", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }

        [HttpGet]
        [ResponseType(typeof(ParticipantDto))]
        [VersionedRoute(template: "findFamily", minimumVersion: "1.0.0")]
        [Route("findFamily")]
        public IHttpActionResult FindFamilies([FromUri] string search)
        {
            try
            {

                return Authorized(token => Ok(_eventService.GetFamiliesForSearch(token, search)));
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto($"Error getting families for search {search}", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }

        [HttpGet]
        [ResponseType(typeof(HouseholdDto))]
        [VersionedRoute(template: "household/{householdId}", minimumVersion: "1.0.0")]
        [Route("household/{householdId}")]
        public IHttpActionResult GetHouseholdById([FromUri] int householdId)
        {
            try
            {

                return Authorized(token => Ok(_eventService.GetHouseholdByHouseholdId(token, householdId)));
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto($"Error updating family ID of {householdId}", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }

        [HttpPut]
        [VersionedRoute(template: "updateFamily", minimumVersion: "1.0.0")]
        [Route("updateFamily")]
        public IHttpActionResult UpdateFamily(HouseholdDto householdDto)
        {
            try
            {
                return Authorized(token =>
                {
                    var updatedHousehold = _eventService.UpdateHouseholdInformation(token, householdDto);
                    return Ok(updatedHousehold);
                });
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto($"Error updating Household ID of {householdDto.HouseholdId}", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }

        [HttpGet]
        [VersionedRoute(template: "events/getCapacity/{siteId}", minimumVersion: "1.0.0")]
        [Route("events/getCapacity/{siteId}")]
        public IHttpActionResult GetCapacity([FromUri] int siteId)
        {
            try
            {
                var capacities = _eventService.GetCapacityBySite(siteId);
                return Ok(capacities);
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto($"Error getting Capacity information. Site: {siteId}", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }
    }
}
