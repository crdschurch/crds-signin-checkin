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
        private readonly IConfigRepository _configRepository;
        private readonly IEventRepository _eventRepository;

        public ChildSigninService(IChildSigninRepository childSigninRepository, IConfigRepository configRepository, IEventRepository eventRepository)
        {
            _childSigninRepository = childSigninRepository;
            _configRepository = configRepository;
            _eventRepository = eventRepository;
        }

        public ParticipantEventMapDto GetChildrenAndEventByPhoneNumber(string phoneNumber, int siteId)
        {
            var currentEvents = _eventRepository.GetEvents(DateTime.Now, DateTime.Now, siteId);

            if (!(currentEvents).Any())
            {
                throw new Exception("No current events for site");
            }

            var mpChildren = _childSigninRepository.GetChildrenByPhoneNumber(phoneNumber);
            var childrenDtos = Mapper.Map<List<MpParticipantDto>, List<ParticipantDto>>(mpChildren);
            var eventDto = Mapper.Map<MpEventDto, EventDto>(currentEvents.First());

            ParticipantEventMapDto participantEventMapDto = new ParticipantEventMapDto
            {
                Participants = childrenDtos,
                CurrentEvent = eventDto
            };

            return participantEventMapDto;
        }

        public ParticipantEventMapDto SigninParticipants(ParticipantEventMapDto participantEventMapDto)
        {
            var earlyCheckinConfig = _configRepository.GetMpConfigByKey("DefaultEarlyCheckIn");
            var lateCheckinConfig = _configRepository.GetMpConfigByKey("DefaultLateCheckIn");

            var mpEvent = _eventRepository.GetEventById(participantEventMapDto.CurrentEvent.EventId);

            // use the event's checkin period if available, otherwise default to the mp config values
            var beginSigninWindow = mpEvent.EventStartDate.AddMinutes(-(mpEvent.EarlyCheckinPeriod ?? int.Parse(earlyCheckinConfig.Value)));
            var endSigninWindow = mpEvent.EventStartDate.AddMinutes((mpEvent.LateCheckinPeriod ?? int.Parse(lateCheckinConfig.Value)));

            if (!(DateTime.Now >= beginSigninWindow && DateTime.Now <= endSigninWindow))
            {
                throw new Exception("Sign-In Not Available For Event " + mpEvent.EventId);
            }

            List<MpEventParticipantDto> mpEventParticipantDtoList = new List<MpEventParticipantDto>();

            // Status ID of 3 = "Attended"
            foreach (var participant in participantEventMapDto.Participants.Where(r => r.Selected == true))
            {
                MpEventParticipantDto mpEventParticipantDto = new MpEventParticipantDto
                {
                    EventId = participantEventMapDto.CurrentEvent.EventId,
                    ParticipantId = participant.ParticipantId,
                    ParticipantStatusId = 3,
                    TimeIn = System.DateTime.Now,
                    OpportunityId = null
                };

                mpEventParticipantDtoList.Add(mpEventParticipantDto);
            }

            var response = new ParticipantEventMapDto
            {
                CurrentEvent = participantEventMapDto.CurrentEvent
            };

            response.Participants = _childSigninRepository.CreateEventParticipants(mpEventParticipantDtoList).Select(Mapper.Map<ParticipantDto>).ToList();

            return response;
        }
    }
}