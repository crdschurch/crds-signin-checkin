using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
//using Crossroads.ApiVersioning;
using MinistryPlatform.Translation.Models.DTO;
using SignInCheckIn.Services.Interfaces;

namespace SignInCheckIn.Controllers
{
    [Route("api")]
    public class SiteController : ApiController
    {
        private readonly ISiteService _siteService;

        public SiteController(ISiteService siteService)
        {
            _siteService = siteService;
        }

        [HttpGet]
        [ResponseType(typeof(List<MpCongregationDto>))]
        //[VersionedRoute(template: "sites", minimumVersion: "1.0.0")]
        [Route("sites")]
        public IHttpActionResult GetSites()
        {
            try
            {
                return Ok(_siteService.GetAll());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
