using System;
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
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json.Linq;
using SignInCheckIn.Hubs;

namespace SignInCheckIn.Controllers
{
    public class ChildSigninController : MpAuth
    {
        private readonly IChildSigninService _childSigninService;
        private readonly IChildCheckinService _childCheckinService;
        private readonly IKioskRepository _kioskRepository;
        private readonly IHubContext _context;
        private readonly IApplicationConfiguration _applicationConfiguration;

        public ChildSigninController(IChildSigninService childSigninService, IChildCheckinService childCheckinService, IAuthenticationRepository authenticationRepository, IKioskRepository kioskRepository, IApplicationConfiguration applicationConfiguration) : base(authenticationRepository)
        {
            _context = GlobalHost.ConnectionManager.GetHubContext<EventHub>();
            _childSigninService = childSigninService;
            _childCheckinService = childCheckinService;
            _kioskRepository = kioskRepository;
            _applicationConfiguration = applicationConfiguration;
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
                        dynamic data = new JObject();
                        data.EventParticipantId = eventparticipantId;
                        data.OriginalRoomId = roomId;

                        PublishToChannel(_context, new ChannelEvent
                        {
                            ChannelName = GetChannelNameCheckinParticipants(_applicationConfiguration, eventId, roomId),
                            Name = "Remove",
                            Data = data
                        });
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
                    // if this is the first event and it is an AC event, we need to set the publish event id to
                    // the parent id (not kc Subevent ID)
                    // we dont need to do this for the AssignedSecondaryRoom (below) because we are already 
                    // setting the EventIdSecondary to the Parent Event ID
                    if (participants.CurrentEvent.EventTypeId == _applicationConfiguration.AdventureClubEventTypeId)
                    {
                        p.EventId = participants.CurrentEvent.ParentEventId.Value;
                    }

                    // ignores the site id if there is an event id so therefore we can put a random 0 here
                    var updatedParticipants = participants.Participants.Where(pp => pp.AssignedRoomId == p.AssignedRoomId);
                    PublishToChannel(_context, new ChannelEvent
                    {
                        ChannelName = GetChannelNameCheckinParticipants(_applicationConfiguration, p.EventId, p.AssignedRoomId.Value),
                        Name = "Add",
                        Data = updatedParticipants
                    });
                }

                if (p.AssignedSecondaryRoomId != null)
                {
                    // ignores the site id if there is an event id so therefore we can put a random 0 here
                    var updatedParticipants = participants.Participants.Where(pp => pp.AssignedSecondaryRoomId == p.AssignedSecondaryRoomId);
                    PublishToChannel(_context, new ChannelEvent
                    {
                        ChannelName = GetChannelNameCheckinParticipants(_applicationConfiguration, p.EventIdSecondary, p.AssignedSecondaryRoomId.Value),
                        Name = "Add",
                        Data = updatedParticipants
                    });
                }
            }
        }
    }
}
