using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using SignInCheckIn.Exceptions.Models;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Security;
using SignInCheckIn.Services.Interfaces;
using Crossroads.ApiVersioning;
using Crossroads.Web.Common.Security;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using SignInCheckIn.Services;

namespace SignInCheckIn.Controllers
{
    public class FamilyController : MpAuth
    {
        private readonly IKioskRepository _kioskRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IFamilyService _familyService;
        private readonly IChildSigninService _childSigninService;

        public FamilyController(IAuthenticationRepository authenticationRepository,
                                IContactRepository contactRepository,
                                IKioskRepository kioskRepository,
                                IFamilyService familyService,
                                IChildSigninService childSigninService) : base(authenticationRepository)
        {
            _kioskRepository = kioskRepository;
            _contactRepository = contactRepository;
            _familyService = familyService;
            _childSigninService = childSigninService;
        }

        [HttpPost]
        [ResponseType(typeof(int))]
        [VersionedRoute(template: "signin/newfamily", minimumVersion: "1.0.0")]
        [Route("signin/newfamily")]
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

        [HttpPost]
        [ResponseType(typeof(int))]
        [VersionedRoute(template: "family/{householdid}/member", minimumVersion: "1.0.0")]
        [Route("family/{householdid}/member")]
        public IHttpActionResult AddNewFamilyMember([FromUri(Name = "householdid")] int householdId, [FromBody] ContactDto newFamilyContact)
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
                    List<ContactDto> newContacts = new List<ContactDto>
                    {
                        newFamilyContact
                    };

                    var newParticipants = _familyService.AddFamilyMembers(token, householdId, newContacts);
                    _childSigninService.CreateGroupParticipants(token, newParticipants);
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
        [VersionedRoute(template: "family/member/{contactId}", minimumVersion: "1.0.0")]
        [Route("family/member/{contactId}")]
        public IHttpActionResult UpdateFamilyMember(ContactDto newFamilyContactDto)
        {
            return Authorized(token =>
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
                    _contactRepository.Update(Mapper.Map<ContactDto, MpContactDto>(newFamilyContactDto), token);
                    _childSigninService.UpdateGradeGroupParticipant(token, newFamilyContactDto.ParticipantId, newFamilyContactDto.DateOfBirth, newFamilyContactDto.YearGrade, true);
                    return Ok();
                }
                catch (Exception e)
                {
                    var apiError = new ApiErrorDto("Update family member error: ", e);
                    throw new HttpResponseException(apiError.HttpResponseMessage);
                }
            });
        }
    }
}
