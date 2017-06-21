using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using SignInCheckIn.Exceptions.Models;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Security;
using SignInCheckIn.Services.Interfaces;
using Crossroads.ApiVersioning;
using Crossroads.Web.Common.Security;
using MinistryPlatform.Translation.Repositories.Interfaces;
using SignInCheckIn.Services;

namespace SignInCheckIn.Controllers
{
    public class FamilyController : MpAuth
    {
        private readonly IKioskRepository _kioskRepository;
        private readonly IFamilyService _familyService;

        public FamilyController(IAuthenticationRepository authenticationRepository,
                                IKioskRepository kioskRepository,
                                IFamilyService familyService) : base(authenticationRepository)
        {
            _kioskRepository = kioskRepository;
            _familyService = familyService;
        }

        [HttpPost]
        [ResponseType(typeof(int))]
        [VersionedRoute(template: "family", minimumVersion: "1.0.0")]
        [Route("family")]
        public IHttpActionResult CreateNewFamily(List<NewParentDto> newParents)
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
                    var participants = _familyService.CreateNewFamily(token, newParents, kioskIdentifier);
                    //TODO: Figure out if this still needs to be in here for the websockets stuff
                    //PublishSignedInParticipantsToRooms(participants);
                    return Ok(participants);
                }
                catch (Exception e)
                {
                    var apiError = new ApiErrorDto("Create new family error: ", e);
                    throw new HttpResponseException(apiError.HttpResponseMessage);
                }
            });
        }

    }
}
