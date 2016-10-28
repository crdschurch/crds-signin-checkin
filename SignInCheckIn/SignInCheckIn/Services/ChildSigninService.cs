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
        private readonly IEventRepository _eventRepository;

        public ChildSigninService(IChildSigninRepository childSigninRepository, IEventRepository eventRepository)
        {
            _childSigninRepository = childSigninRepository;
            _eventRepository = eventRepository;
        }

        public List<ParticipantDto> GetChildrenByPhoneNumber(string phoneNumber)
        {
            var mpChildren = _childSigninRepository.GetChildrenByPhoneNumber(phoneNumber);
            return Mapper.Map<List<MpParticipantDto>, List<ParticipantDto>>(mpChildren);
        }

        public void SigninParticipants(ParticipantEventMapDto participantEventMapDto)
        {
            var mpEvent = _eventRepository.GetEventById(participantEventMapDto.CurrentEvent.EventId);

            var beginSigninWindow = mpEvent.EventStartDate.AddMinutes(-mpEvent.EarlyCheckinPeriod);
            var endSigninWindow = mpEvent.EventStartDate.AddMinutes(mpEvent.LateCheckinPeriod);

            if (!(DateTime.Now >= beginSigninWindow && DateTime.Now <= endSigninWindow))
            {
                throw new Exception("Sign-In Not Available For Event " + mpEvent.EventId);
            }

            foreach (var participant in participantEventMapDto.Participants.Where(r => r.Selected == true))
            {

                // status should be set to arrived 
            }
        }
    }
}