using AutoMapper;
using Crossroads.Web.Auth.Controllers;
//using Crossroads.ApiVersioning;
using Crossroads.Web.Common.Security;
using Crossroads.Web.Common.Services;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using SignInCheckIn.Exceptions.Models;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace SignInCheckIn.Controllers
{
    [RoutePrefix("api")]
    public class FamilyController : AuthBaseController
    {
        private readonly IKioskRepository _kioskRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IFamilyService _familyService;
        private readonly IChildSigninService _childSigninService;
        private const int KidsClubTools = 112;

        public FamilyController(IAuthTokenExpiryService authTokenExpiryService,
                                IAuthenticationRepository authenticationRepository,
                                IContactRepository contactRepository,
                                IKioskRepository kioskRepository,
                                IFamilyService familyService,
                                IChildSigninService childSigninService) : base(authenticationRepository, authTokenExpiryService)
        {
            _kioskRepository = kioskRepository;
            _contactRepository = contactRepository;
            _familyService = familyService;
            _childSigninService = childSigninService;
        }

        [HttpPost]
        [ResponseType(typeof(int))]
        //[VersionedRoute(template: "family", minimumVersion: "1.0.0")]
        [Route("family")]
        public IHttpActionResult CreateNewFamily(List<NewParentDto> newParents)
        {
            return Authorized(authDto =>
            {
                string kioskIdentifier;

                // make sure kiosk is admin type and configured for printing
                if (Request.Headers.Contains("Crds-Kiosk-Identifier"))
                {
                    kioskIdentifier = Request.Headers.GetValues("Crds-Kiosk-Identifier").First();
                    var kioskConfig = _kioskRepository.GetMpKioskConfigByIdentifier(Guid.Parse(kioskIdentifier));
                    // must be kiosk type admin
                    if (kioskConfig.KioskTypeId != 3)
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
                    if (!authDto.Authorization.MpRoles.ContainsKey(KidsClubTools))
                    {
                        throw new UnauthorizedAccessException();
                    }

                    var participants = _familyService.CreateNewFamily(newParents, kioskIdentifier);
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

        [HttpPost]
        [ResponseType(typeof(int))]
        //[VersionedRoute(template: "family/{householdid}/member", minimumVersion: "1.0.0")]
        [Route("family/{householdid}/member")]
        public IHttpActionResult AddNewFamilyMember([FromUri(Name = "householdid")] int householdId, [FromBody] List<ContactDto> newFamilyContacts)
        {
            return Authorized(authDto =>
            {
                string kioskIdentifier;

                // make sure kiosk is admin type and configured for printing
                if (Request.Headers.Contains("Crds-Kiosk-Identifier"))
                {
                    kioskIdentifier = Request.Headers.GetValues("Crds-Kiosk-Identifier").First();
                    var kioskConfig = _kioskRepository.GetMpKioskConfigByIdentifier(Guid.Parse(kioskIdentifier));
                    // must be kiosk type admin
                    if (kioskConfig.KioskTypeId != 3)
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
                    var newParticipants = _familyService.AddFamilyMembers(householdId, newFamilyContacts);
                    _childSigninService.CreateGroupParticipants(newParticipants);
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
        //[VersionedRoute(template: "family/member/{contactId}", minimumVersion: "1.0.0")]
        [Route("family/member/{contactId}")]
        public IHttpActionResult UpdateFamilyMember(ContactDto newFamilyContactDto)
        {
            return Authorized(authDto =>
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
                    _contactRepository.Update(Mapper.Map<ContactDto, MpContactDto>(newFamilyContactDto));
                    _childSigninService.UpdateGradeGroupParticipant(newFamilyContactDto.ParticipantId, newFamilyContactDto.DateOfBirth, newFamilyContactDto.YearGrade, true);
                    return Ok();
                }
                catch (Exception e)
                {
                    var apiError = new ApiErrorDto("Update family member error: ", e);
                    throw new HttpResponseException(apiError.HttpResponseMessage);
                }
            });
        }

        [HttpGet]
        [ResponseType(typeof(UserDto))]
        //[VersionedRoute(template: "user", minimumVersion: "1.0.0")]
        [Route("user")]
        public IHttpActionResult CreateNewFamily([FromUri] string email)
        {
            return Authorized(authDto =>
            {
                return Ok(_familyService.GetUserByEmailAddress(email));
            });
        }
    }
}
