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
    public class KioskService : IKioskService
    {
        private readonly IKioskRepository _kioskRepository;

        public KioskService(IKioskRepository kioskRepository)
        {
            _kioskRepository = kioskRepository;
        }

        public KioskConfigDto GetKioskConfigByIdentifier(Guid kioskIdentifier)
        {
            var mpKiosksDtos = _kioskRepository.GetMpKioskConfigByIdentifier(kioskIdentifier);

            return Mapper.Map<MpKioskConfigDto, KioskConfigDto>(mpKiosksDtos);
        }
    }
}