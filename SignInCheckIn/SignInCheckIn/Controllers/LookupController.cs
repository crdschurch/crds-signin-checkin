using Crossroads.Web.Auth.Controllers;
//using Crossroads.ApiVersioning;
using Crossroads.Web.Common.Security;
using Crossroads.Web.Common.Services;
using SignInCheckIn.Exceptions.Models;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;

namespace SignInCheckIn.Controllers
{
    [RoutePrefix("api")]
    public class LookupController : AuthBaseController
    {
        private readonly ILookupService _lookupService;

        public LookupController(IAuthTokenExpiryService authTokenExpiryService, ILookupService lookupService, IAuthenticationRepository authenticationRepository) : base(authenticationRepository, authTokenExpiryService)
        {
            _lookupService = lookupService;
        }

        [HttpGet]
        [ResponseType(typeof(List<StateDto>))]
        //[VersionedRoute(template: "getStates", minimumVersion: "1.0.0")]
        [Route("getStates")]
        public IHttpActionResult GetStates()
        {
            return Authorized(authDto =>
            {
                try
                {
                    var states = _lookupService.GetStates();
                    return Ok(states);
                }
                catch (Exception e)
                {
                    var apiError = new ApiErrorDto("Error getting states", e);
                    throw new HttpResponseException(apiError.HttpResponseMessage);
                }
            });
        }

        [HttpGet]
        [ResponseType(typeof(List<CountryDto>))]
        //[VersionedRoute(template: "getCountries", minimumVersion: "1.0.0")]
        [Route("getCountries")]
        public IHttpActionResult GetCountries()
        {
            return Authorized(authDto =>
            {
                try
                {
                    var countries = _lookupService.GetCountries();
                    return Ok(countries);
                }
                catch (Exception e)
                {
                    var apiError = new ApiErrorDto("Error getting countries", e);
                    throw new HttpResponseException(apiError.HttpResponseMessage);
                }
            });
        }
    }
}
