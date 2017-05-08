using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using SignInCheckIn.Exceptions.Models;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Security;
using SignInCheckIn.Services.Interfaces;
using Crossroads.ApiVersioning;
using Crossroads.Web.Common.Security;
using MinistryPlatform.Translation.Models.DTO;

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
        [ResponseType(typeof(List<StateDto>))]
        [VersionedRoute(template: "getStates", minimumVersion: "1.0.0")]
        [Route("getStates")]
        public IHttpActionResult GetStates()
        {
            return Authorized(token =>
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
        [VersionedRoute(template: "getCountries", minimumVersion: "1.0.0")]
        [Route("getCountries")]
        public IHttpActionResult GetCountries()
        {
            return Authorized(token =>
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

        [HttpGet]
        [ResponseType(typeof(List<CongregationDto>))]
        [VersionedRoute(template: "sites", minimumVersion: "1.0.0")]
        [Route("sites")]
        public IHttpActionResult GetSites()
        {
            return Ok(_lookupService.GetCongregations());
        }

        [HttpGet]
        [ResponseType(typeof(List<MpCongregationDto>))]
        [VersionedRoute(template: "sites", minimumVersion: "1.0.0")]
        [Route("sites")]
        public IHttpActionResult GetLocations()
        {
            return Ok(_lookupService.GetLocations());
        }
    }
}
