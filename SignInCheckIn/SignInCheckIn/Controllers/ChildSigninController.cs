using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using MinistryPlatform.Translation.Repositories.Interfaces;
using SignInCheckIn.Exceptions.Models;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Security;
using SignInCheckIn.Services.Interfaces;
using Crossroads.ApiVersioning;
using Crossroads.Utilities.Services.Interfaces;
using Crossroads.Web.Common.Security;
using MinistryPlatform.Translation.Models.DTO;
using Newtonsoft.Json.Linq;

namespace SignInCheckIn.Controllers
{
    public class ChildSigninController : MpAuth
    {
        private readonly IWebsocketService _websocketService;
        private readonly IChildSigninService _childSigninService;
        private readonly IKioskRepository _kioskRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IFamilyService _familyService;

        public ChildSigninController(IChildSigninService childSigninService, IWebsocketService websocketService, IAuthenticationRepository authenticationRepository, IKioskRepository kioskRepository, IContactRepository contactRepository, IFamilyService familyService) : base(authenticationRepository)
        {
            _websocketService = websocketService;
            _childSigninService = childSigninService;
            _kioskRepository = kioskRepository;
            _contactRepository = contactRepository;
            _familyService = familyService;
        }

        [HttpGet]
        [ResponseType(typeof(ParticipantEventMapDto))]
        [VersionedRoute(template: "events/{eventId}/signin/children/household/{householdId}", minimumVersion: "1.0.0")]
        [Route("events/{eventId}/signin/children/household/{householdId}")]
        public IHttpActionResult GetChildrenAndEventByHousehold(int eventId, int householdId)
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

                string kioskId = "";

                if (Request.Headers.Contains("Crds-Kiosk-Identifier"))
                {
                    kioskId = Request.Headers.GetValues("Crds-Kiosk-Identifier").First();
                }

                var children = _childSigninService.GetChildrenAndEventByHouseholdId(householdId, eventId, siteId, kioskId);
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

                string kioskId = "";

                if (Request.Headers.Contains("Crds-Kiosk-Identifier"))
                {
                    kioskId = Request.Headers.GetValues("Crds-Kiosk-Identifier").First();
                }

                var children = _childSigninService.GetChildrenAndEventByPhoneNumber(phoneNumber, siteId, null, false, kioskId);
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
        [VersionedRoute(template: "signin/familyfinder", minimumVersion: "1.0.0")]
        [Route("signin/familyfinder")]
        public IHttpActionResult SigninFamilyFinder(ParticipantEventMapDto participantEventMapDto)
        {
            try
            {
                string kioskIdentifier = "";
                if (Request.Headers.Contains("Crds-Kiosk-Identifier"))
                {
                    kioskIdentifier = Request.Headers.GetValues("Crds-Kiosk-Identifier").First();
                }

                var participants = _childSigninService.SigninParticipants(participantEventMapDto, true);
                PublishSignedInParticipantsToRooms(participants);
                participantEventMapDto.Participants = participants.Participants;
                _childSigninService.PrintParticipants(participantEventMapDto, kioskIdentifier);
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
        [VersionedRoute(template: "signin/family/{householdid}/member", minimumVersion: "1.0.0")]
        [Route("signin/family/{householdid}/member")]
        public IHttpActionResult AddNewFamilyMember([FromUri(Name = "householdid")] int householdId, [FromBody] ContactDto newFamilyContact)
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
                    List<ContactDto> newContacts = new List<ContactDto>
                    {
                        newFamilyContact
                    };

                    var newParticipants = _familyService.AddFamilyMembers(token, householdId, newContacts);
                    _childSigninService.CreateGroupParticipants(token, newParticipants);
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
        [ResponseType(typeof(int))]
        [VersionedRoute(template: "signin/family/member/{contactId}", minimumVersion: "1.0.0")]
        [Route("signin/family/member/{contactId}")]
        public IHttpActionResult UpdateFamilyMember(ContactDto newFamilyContactDto)
        {
            return Authorized(token =>
            {
                 // make sure kiosk is admin type and configured for printing
                if (Request.Headers.Contains("Crds-Kiosk-Identifier"))
                {
                    string kioskIdentifier = Request.Headers.GetValues("Crds-Kiosk-Identifier").First();
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
                    _contactRepository.Update(Mapper.Map<ContactDto, MpContactDto>(newFamilyContactDto), token);
                    _childSigninService.UpdateGradeGroupParticipant(token, newFamilyContactDto.ParticipantId, newFamilyContactDto.DateOfBirth, newFamilyContactDto.YearGrade, true);
                    return Ok();
                }
                catch (Exception e)
                {
                    var apiError = new ApiErrorDto("Update family member error: ", e);
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

                        _websocketService.PublishCheckinParticipantsSignedInRemove(eventId, roomId, data);
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
