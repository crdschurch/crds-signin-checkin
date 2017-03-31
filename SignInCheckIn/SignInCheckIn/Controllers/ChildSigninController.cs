using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using MinistryPlatform.Translation.Repositories.Interfaces;
using SignInCheckIn.Exceptions.Models;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Security;
using SignInCheckIn.Services.Interfaces;
using Crossroads.ApiVersioning;
using Crossroads.Utilities.Services.Interfaces;
using Crossroads.Web.Common.Security;
using Newtonsoft.Json.Linq;

namespace SignInCheckIn.Controllers
{
    public class ChildSigninController : MpAuth
    {
        private readonly IWebsocketService _websocketService;
        private readonly IChildSigninService _childSigninService;
        private readonly IChildCheckinService _childCheckinService;
        private readonly IKioskRepository _kioskRepository;
        private readonly IApplicationConfiguration _applicationConfiguration;

        public ChildSigninController(IChildSigninService childSigninService, IWebsocketService websocketService, IChildCheckinService childCheckinService, IAuthenticationRepository authenticationRepository, IKioskRepository kioskRepository, IApplicationConfiguration applicationConfiguration) : base(authenticationRepository)
        {
            _websocketService = websocketService;
            _childSigninService = childSigninService;
            _childCheckinService = childCheckinService;
            _kioskRepository = kioskRepository;
            _applicationConfiguration = applicationConfiguration;
        }

        [HttpGet]
        [ResponseType(typeof(ParticipantEventMapDto))]
        [VersionedRoute(template: "signin/children/household/{householdId}", minimumVersion: "1.0.0")]
        [Route("signin/children/household/{householdId}")]
        public IHttpActionResult GetChildrenAndEventByHousehold(int householdId)
        {
            try
            {
                var siteId = 0;
                if (Request.Headers.Contains("Crds-Site-Id"))
                {
                    siteId = int.Parse(Request.Headers.GetValues("Crds-Site-Id").First());
                }

                if (siteId == 0)
                {
                    throw new Exception("Site Id is Invalid");
                }

                var children = _childSigninService.GetChildrenAndEventByHouseholdId(householdId, siteId);
                return Ok(children);
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto("Get Children", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }

        [HttpGet]
        [ResponseType(typeof(ParticipantEventMapDto))]
        [VersionedRoute(template: "signin/children/{phoneNumber}", minimumVersion: "1.0.0")]
        [Route("signin/children/{phoneNumber}")]
        public IHttpActionResult GetChildrenAndEvent(string phoneNumber)
        {
            try
            {
                var siteId = 0;
                if (Request.Headers.Contains("Crds-Site-Id"))
                {
                    siteId = int.Parse(Request.Headers.GetValues("Crds-Site-Id").First());
                }

                if (siteId == 0)
                {
                    throw new Exception("Site Id is Invalid");
                }

                var children = _childSigninService.GetChildrenAndEventByPhoneNumber(phoneNumber, siteId, null);
                return Ok(children);
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto("Get Children", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }

        [HttpPost]
        [ResponseType(typeof(ParticipantEventMapDto))]
        [VersionedRoute(template: "signin/children", minimumVersion: "1.0.0")]
        [Route("signin/children")]
        public IHttpActionResult SigninParticipants(ParticipantEventMapDto participantEventMapDto)
        {
            try
            {
                var participants = _childSigninService.SigninParticipants(participantEventMapDto);
                PublishSignedInParticipantsToRooms(participants);
                return Ok(participants);
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto("Sign In Participants", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }

        [HttpPost]
        [ResponseType(typeof(ParticipantEventMapDto))]
        [VersionedRoute(template: "signin/participants/print", minimumVersion: "1.0.0")]
        [Route("signin/participants/print")]
        public IHttpActionResult PrintParticipants(ParticipantEventMapDto participantEventMapDto)
        {
            try
            {
                string kioskIdentifier;

                if (Request.Headers.Contains("Crds-Kiosk-Identifier"))
                {
                    kioskIdentifier = Request.Headers.GetValues("Crds-Kiosk-Identifier").First();
                }
                else
                {
                    throw new Exception("No kiosk identifier");
                }

                return Ok(_childSigninService.PrintParticipants(participantEventMapDto, kioskIdentifier));
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto("Print Participants", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }

        [HttpPost]
        [ResponseType(typeof(ParticipantEventMapDto))]
        [VersionedRoute(template: "signin/participant/{eventParticipantId}/print", minimumVersion: "1.0.0")]
        [Route("signin/participant/{eventParticipantId}/print")]
        public IHttpActionResult PrintParticipant(int eventParticipantId)
        {
            return Authorized(token =>
            {
                string kioskIdentifier;

                // make sure kiosk is admin type and configured for printing
                if (Request.Headers.Contains("Crds-Kiosk-Identifier"))
                {
                    kioskIdentifier = Request.Headers.GetValues("Crds-Kiosk-Identifier").First();
                    var kioskConfig = _kioskRepository.GetMpKioskConfigByIdentifier(Guid.Parse(kioskIdentifier));
                    // must be kiosk type admin and have a printer set up
                    if (kioskConfig.PrinterMapId == null || kioskConfig.KioskTypeId != 3)
                    {
                        throw new HttpResponseException(System.Net.HttpStatusCode.PreconditionFailed);
                    }
                }
                else
                {
                    throw new HttpResponseException(System.Net.HttpStatusCode.PreconditionFailed);
                }

                try
                {
                    return Ok(_childSigninService.PrintParticipant(eventParticipantId, kioskIdentifier, token));
                }
                catch (Exception e)
                {
                    var apiError = new ApiErrorDto("Print Participants", e);
                    throw new HttpResponseException(apiError.HttpResponseMessage);
                }
            });
        }

        [HttpPost]
        [ResponseType(typeof(int))]
        [VersionedRoute(template: "signin/newfamily", minimumVersion: "1.0.0")]
        [Route("signin/newfamily")]
        public IHttpActionResult CreateNewFamily(NewFamilyDto newFamilyDto)
        {
            return Authorized(token =>
            {
                string kioskIdentifier;

                // make sure kiosk is admin type and configured for printing
                if (Request.Headers.Contains("Crds-Kiosk-Identifier"))
                {
                    kioskIdentifier = Request.Headers.GetValues("Crds-Kiosk-Identifier").First();
                    var kioskConfig = _kioskRepository.GetMpKioskConfigByIdentifier(Guid.Parse(kioskIdentifier));
                    // must be kiosk type admin and have a printer set up
                    if (kioskConfig.PrinterMapId == null || kioskConfig.KioskTypeId != 3)
                    {
                        throw new HttpResponseException(System.Net.HttpStatusCode.PreconditionFailed);
                    }
                }
                else
                {
                    throw new HttpResponseException(System.Net.HttpStatusCode.PreconditionFailed);
                }

                try
                {
                    var participants = _childSigninService.CreateNewFamily(token, newFamilyDto, kioskIdentifier);

                    PublishSignedInParticipantsToRooms(participants);
                    return Ok();
                }
                catch (Exception e)
                {
                    var apiError = new ApiErrorDto("Create new family error: ", e);
                    throw new HttpResponseException(apiError.HttpResponseMessage);
                }
            });
        }

        [HttpPut]
        [ResponseType(typeof(ParticipantEventMapDto))]
        [VersionedRoute(template: "signin/event/{eventId}/room/{roomId}/reverse/{eventparticipantId}", minimumVersion: "1.0.0")]
        [Route("signin/event/{eventId}/room/{roomId}/reverse/{eventparticipantId}")]
        public IHttpActionResult ReverseSignin(int eventId, int roomId, int eventparticipantId)
        {
            return Authorized(token =>
            {
                try
                {
                    var reverseSuccess = _childSigninService.ReverseSignin(token, eventparticipantId);

                    if (reverseSuccess == true)
                    {
                        var data = new ParticipantDto();
                        data.EventParticipantId = eventparticipantId;
                        data.OriginalRoomId = roomId;

                        _websocketService.PublishCheckinParticipantsRemove(eventId, roomId, data);
                        return Ok();
                    }
                    else
                    {
                        return Conflict();
                    }
                }
                catch (Exception e)
                {
                    var apiError = new ApiErrorDto("Error reversing signin for event participant ", e);
                    throw new HttpResponseException(apiError.HttpResponseMessage);
                }
            });
        }

        private void PublishSignedInParticipantsToRooms(ParticipantEventMapDto participants)
        {
            foreach (var p in participants.Participants)
            {
                if (p.AssignedRoomId != null)
                {
                    // ignores the site id if there is an event id so therefore we can put a random 0 here
                    var updatedParticipants = participants.Participants.Where(pp => pp.AssignedRoomId == p.AssignedRoomId);
                    _websocketService.PublishCheckinParticipantsAdd(p.EventId, p.AssignedRoomId.Value, new List<ParticipantDto>() {p});
                }

                if (p.AssignedSecondaryRoomId != null)
                {
                    // ignores the site id if there is an event id so therefore we can put a random 0 here
                    var updatedParticipants = participants.Participants.Where(pp => pp.AssignedSecondaryRoomId == p.AssignedSecondaryRoomId);
                    _websocketService.PublishCheckinParticipantsAdd(p.EventIdSecondary, p.AssignedSecondaryRoomId.Value, new List<ParticipantDto>() { p });
                }
            }
        }
    }
}
