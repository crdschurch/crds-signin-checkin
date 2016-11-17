﻿using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using SignInCheckIn.Exceptions.Models;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Services.Interfaces;

namespace SignInCheckIn.Controllers
{
    public class ChildSigninController : ApiController
    {
        private readonly IChildSigninService _childSigninService;

        public ChildSigninController(IChildSigninService childSigninService)
        {
            _childSigninService = childSigninService;
        }

        [HttpGet]
        [ResponseType(typeof(ParticipantEventMapDto))]
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

                var children = _childSigninService.GetChildrenAndEventByPhoneNumber(phoneNumber, siteId);
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
        [Route("participants/print")]
        public IHttpActionResult PrintParticipants(ParticipantEventMapDto participantEventMapDto)
        {
            try
            {
                string kioskIdentifier = String.Empty;

                if (Request.Headers.Contains("Crds-Kiosk-Identifier"))
                {
                    kioskIdentifier = (Request.Headers.GetValues("Crds-Kiosk-Identifier").First());
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
    }
}
