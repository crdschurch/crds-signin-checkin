using System;
using System.Collections.Generic;
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
    public class LookupController : MpAuth
    {
        private readonly ILookupService _lookupService;

        public LookupController(ILookupService lookupService, IAuthenticationRepository authenticationRepository) : base(authenticationRepository)
        {
            _lookupService = lookupService;
        }

        [HttpGet]
        [ResponseType(typeof(StateDto))]
        [VersionedRoute(template: "geStates", minimumVersion: "1.0.0")]
        [Route("geStates")]
        public IHttpActionResult GeStates()
        {
            try
            {
                return Authorized(token => Ok(_lookupService.GetStates()));
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto($"Error getting states", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }

        [HttpGet]
        [ResponseType(typeof(HouseholdDto))]
        [VersionedRoute(template: "getCountries", minimumVersion: "1.0.0")]
        [Route("getCountries")]
        public IHttpActionResult GetCountries()
        {
            try
            {
                return Authorized(token => Ok(_lookupService.GetCountries()));
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto($"Error getting countries", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }
    }
}
