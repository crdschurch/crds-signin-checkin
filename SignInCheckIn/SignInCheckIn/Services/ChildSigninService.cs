using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Services.Interfaces;

namespace SignInCheckIn.Services
{
    public class ChildSigninService : IChildSigninService
    {
        private readonly IChildSigninRepository _childSigninRepository;

        public ChildSigninService(IChildSigninRepository childSigninRepository)
        {
            _childSigninRepository = childSigninRepository;
        }

        public List<ParticipantDto> GetChildrenByPhoneNumber(string phoneNumber)
        {
            var mpChildren = _childSigninRepository.GetChildrenByPhoneNumber(phoneNumber);
            return Mapper.Map<List<MpParticipantDto>, List<ParticipantDto>>(mpChildren);
        }
    }
}