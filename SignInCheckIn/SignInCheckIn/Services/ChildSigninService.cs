using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IGroupRepository _groupRepository;

        private readonly int _defaultEarlyCheckinPeriod;
        private readonly int _defaultLateCheckinPeriod;

        public ChildSigninService(IChildSigninRepository childSigninRepository, IConfigRepository configRepository, IEventRepository eventRepository, IGroupRepository groupRepository)
        {
            _childSigninRepository = childSigninRepository;
            _eventRepository = eventRepository;
            _groupRepository = groupRepository;

            _defaultEarlyCheckinPeriod = int.Parse(configRepository.GetMpConfigByKey("DefaultEarlyCheckIn").Value);
            _defaultLateCheckinPeriod = int.Parse(configRepository.GetMpConfigByKey("DefaultLateCheckIn").Value);
        }

        public ParticipantEventMapDto GetChildrenAndEventByPhoneNumber(string phoneNumber, int siteId)
        {
            // look between midnights on the current day
            var eventOffsetStartString = DateTime.Now.ToShortDateString();
            var eventOffsetStartTime = DateTime.Parse(eventOffsetStartString);
            var eventOffsetEndTime = DateTime.Parse(eventOffsetStartString).AddDays(1);

            var currentEvents = _eventRepository.GetEvents(eventOffsetStartTime, eventOffsetEndTime, siteId).Where(r => CheckEventTimeValidity(r)).ToList();

            if (!currentEvents.Any())
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

            // Get groups that are configured for the event
            var eventGroupsForEvent = _eventRepository.GetEventGroupsForEvent(participantEventMapDto.CurrentEvent.EventId);

            var mpEventParticipantDtoList = (from participant in participantEventMapDto.Participants.Where(r => r.Selected)
                // Get groups for the participant
                let groupIds = _groupRepository.GetGroupsForParticipantId(participant.ParticipantId)

                // TODO: Gracefully handle exception for mix of valid and invalid signins
                let eventGroup = eventGroupsForEvent.Find(r => groupIds.Exists(g => r.GroupId == g.Id))

                select
                    new MpEventParticipantDto
                    {
                        EventId = participantEventMapDto.CurrentEvent.EventId,
                        ParticipantId = participant.ParticipantId,
                        ParticipantStatusId = 3, // Status ID of 3 = "Attended"
                        TimeIn = DateTime.Now,
                        OpportunityId = null,
                        RoomId = eventGroup.RoomReservation.RoomId
                    }).ToList();


            var response = new ParticipantEventMapDto
            {
                CurrentEvent = participantEventMapDto.CurrentEvent,
                Participants = _childSigninRepository.CreateEventParticipants(mpEventParticipantDtoList).Select(Mapper.Map<ParticipantDto>).ToList()
            };

            response.Participants.ForEach(p => p.Selected = true);

            return response;
        }

        private bool CheckEventTimeValidity(MpEventDto mpEventDto)
        {
            // use the event's checkin period if available, otherwise default to the mp config values
            var beginSigninWindow = mpEventDto.EventStartDate.AddMinutes(-(mpEventDto.EarlyCheckinPeriod ?? _defaultEarlyCheckinPeriod));
            var endSigninWindow = mpEventDto.EventStartDate.AddMinutes(mpEventDto.LateCheckinPeriod ?? _defaultLateCheckinPeriod);

            return DateTime.Now >= beginSigninWindow && DateTime.Now <= endSigninWindow;
        }
    }
}