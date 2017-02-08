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

namespace SignInCheckIn.Controllers
{
    public class KioskController : MpAuth
    {
        private readonly IKioskService _kioskService;
        
        public KioskController(IKioskService kioskService, IAuthenticationRepository authenticationRepository) : base(authenticationRepository)
        {
            _kioskService = kioskService;
        }

        [HttpGet]
        [ResponseType(typeof(List<KioskConfigDto>))]
        [VersionedRoute(template: "kiosks/{kioskid}", minimumVersion: "1.0.0")]
        [Route("kiosks/{kioskid}")]
        public IHttpActionResult GetEvents(
            [FromUri(Name = "kioskid")] Guid kioskId)
        {
            try
            {
                var kioskList = _kioskService.GetKioskConfigByIdentifier(kioskId);
                return Ok(kioskList);
            }
            catch (Exception e)
            {
                var apiError = new ApiErrorDto("Get Kiosks", e);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }
    }
}
