﻿using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using MinistryPlatform.Translation.Repositories.Interfaces;
using SignInCheckIn.Exceptions.Models;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Security;
using SignInCheckIn.Services.Interfaces;
using Crossroads.ApiVersioning;
using Crossroads.Web.Common.Security;

namespace SignInCheckIn.Controllers
{
    public class ChildSigninController : MpAuth
    {

        //private readonly IRoomService _roomService;

        //public RoomController(IRoomService roomService, IAuthenticationRepository authenticationRepository) : base(authenticationRepository)
        //{
        //    _roomService = roomService;
        //}

        private readonly IChildSigninService _childSigninService;
        private readonly IKioskRepository _kioskRepository;

        public ChildSigninController(IChildSigninService childSigninService, IAuthenticationRepository authenticationRepository, IKioskRepository kioskRepository) : base(authenticationRepository)
        {
            _childSigninService = childSigninService;
            _kioskRepository = kioskRepository;
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
                return Ok(_childSigninService.SigninParticipants(participantEventMapDto));
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
                    _childSigninService.CreateNewFamily(token, newFamilyDto, kioskIdentifier);
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
        [VersionedRoute(template: "signin/reverse/{eventparticipantid}", minimumVersion: "1.0.0")]
        [Route("signin/reverse/{eventparticipantid}")]
        public IHttpActionResult ReverseSignin(int eventparticipantid)
        {
            return Authorized(token =>
            {
                try
                {
                    var reverseSuccess = _childSigninService.ReverseSignin(token, eventparticipantid);

                    if (reverseSuccess == true)
                    {
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
    }
}
