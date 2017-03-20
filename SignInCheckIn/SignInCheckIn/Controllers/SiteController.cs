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
using MinistryPlatform.Translation.Models.DTO;

namespace SignInCheckIn.Controllers
{
    public class SiteController : ApiController
    {
        private readonly ISiteRepository _siteRepository;

        public SiteController(ISiteRepository siteRepository)
        {
            _siteRepository = siteRepository;
        }

        [HttpGet]
        [ResponseType(typeof(List<MpCongregationDto>))]
        [VersionedRoute(template: "sites", minimumVersion: "1.0.0")]
        [Route("sites")]
        public IHttpActionResult GetSites()
        {
            return Ok(_siteRepository.GetAll());
        }
    }
}
