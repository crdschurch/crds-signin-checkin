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
    public class ChildCheckinService : IChildCheckinService
    {
        private readonly IChildCheckinRepository _childCheckinRepository;

        public ChildCheckinService(IChildCheckinRepository childCheckinRepository)
        {
            _childCheckinRepository = childCheckinRepository;
        }

        public List<ParticipantDto> GetChildrenByPhoneNumber(string phoneNumber)
        {
            var mpChildren = _childCheckinRepository.GetChildrenByPhoneNumber(phoneNumber);
            return Mapper.Map<List<MpParticipantDto>, List<ParticipantDto>>(mpChildren);
        }
    }
}