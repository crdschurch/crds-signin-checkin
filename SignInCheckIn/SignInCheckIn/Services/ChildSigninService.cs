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

        private readonly int _defaultEarlyCheckinPeriod;
        private readonly int _defaultLateCheckinPeriod;

        public ChildSigninService(IChildSigninRepository childSigninRepository, IConfigRepository configRepository, IEventRepository eventRepository)
        {
            _childSigninRepository = childSigninRepository;
            _configRepository = configRepository;
            _eventRepository = eventRepository;

            _defaultEarlyCheckinPeriod = int.Parse(_configRepository.GetMpConfigByKey("DefaultEarlyCheckIn").Value);
            _defaultLateCheckinPeriod = int.Parse(_configRepository.GetMpConfigByKey("DefaultLateCheckIn").Value);
        }

        public ParticipantEventMapDto GetChildrenAndEventByPhoneNumber(string phoneNumber, int siteId)
        {
            // look between midnights on the current day
            var eventOffsetStartString = DateTime.Now.ToShortDateString();
            var eventOffsetStartTime = DateTime.Parse(eventOffsetStartString);
            var eventOffsetEndTime = DateTime.Parse(eventOffsetStartString).AddDays(1);

            var currentEvents = _eventRepository.GetEvents(eventOffsetStartTime, eventOffsetEndTime, siteId).Where(r => CheckEventTimeValidity(r) == true).ToList();

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
            var mpEventDto = _eventRepository.GetEventById(participantEventMapDto.CurrentEvent.EventId);

            if (CheckEventTimeValidity(mpEventDto) == false)
            {
                throw new Exception("Sign-In Not Available For Event " + mpEventDto.EventId);
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

            foreach (var item in response.Participants)
            {
                item.Selected = true;
            }

            return response;
        }

        private bool CheckEventTimeValidity(MpEventDto mpEventDto)
        {
            // use the event's checkin period if available, otherwise default to the mp config values
            var beginSigninWindow = mpEventDto.EventStartDate.AddMinutes(-(mpEventDto.EarlyCheckinPeriod ?? _defaultEarlyCheckinPeriod));
            var endSigninWindow = mpEventDto.EventStartDate.AddMinutes(mpEventDto.LateCheckinPeriod ?? _defaultLateCheckinPeriod);

            if (!(DateTime.Now >= beginSigninWindow && DateTime.Now <= endSigninWindow))
            {
                return false;
            }

            return true;
        }
    }
}