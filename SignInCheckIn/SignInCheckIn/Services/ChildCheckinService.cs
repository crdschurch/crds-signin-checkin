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
    public class ChildCheckinService : IChildCheckinService
    {
        private readonly IChildSigninRepository _childSigninRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IEventService _eventService;
        private readonly IGroupRepository _groupRepository;

        public ChildCheckinService(IChildSigninRepository childSigninRepository, IEventRepository eventRepository, IGroupRepository groupRepository, IEventService eventService)
        {
            _childSigninRepository = childSigninRepository;
            _eventRepository = eventRepository;
            _groupRepository = groupRepository;
            _eventService = eventService;
        }

        public ParticipantEventMapDto GetChildrenForCurrentEventAndRoom(int roomId, int siteId, int? eventId)
        {
            var eventDto = eventId == null ? _eventService.GetCurrentEventForSite(siteId) : _eventService.GetEvent((int) eventId);
            var mpChildren = _childSigninRepository.GetChildrenByPhoneNumber(phoneNumber);
            var childrenDtos = Mapper.Map<List<MpParticipantDto>, List<ParticipantDto>>(mpChildren);

            ParticipantEventMapDto participantEventMapDto = new ParticipantEventMapDto
            {
                Participants = childrenDtos,
                CurrentEvent = eventDto
            };

            return participantEventMapDto;
        }

        //    public ParticipantEventMapDto CheckinParticipants(ParticipantEventMapDto participantEventMapDto)
        //    {
        //        var eventDto = _eventService.GetEvent(participantEventMapDto.CurrentEvent.EventId);

        //        if (_eventService.CheckEventTimeValidity(eventDto) == false)
        //        {
        //            throw new Exception("Sign-In Not Available For Event " + eventDto.EventId);
        //        }

        //        Get groups that are configured for the event
        //       var eventGroupsForEvent = _eventRepository.GetEventGroupsForEvent(participantEventMapDto.CurrentEvent.EventId);

        //    var mpEventParticipantDtoList = (from participant in participantEventMapDto.Participants.Where(r => r.Selected)
        //             Get groups for the participant
        //            let groupIds = _groupRepository.GetGroupsForParticipantId(participant.ParticipantId)

        //             TODO: Gracefully handle exception for mix of valid and invalid signins
        //            let eventGroup = eventGroupsForEvent.Find(r => groupIds.Exists(g => r.GroupId == g.Id))

        //            select
        //                new MpEventParticipantDto
        //                {
        //                    EventId = participantEventMapDto.CurrentEvent.EventId,
        //                    ParticipantId = participant.ParticipantId,
        //                    ParticipantStatusId = 3, // Status ID of 3 = "Attended"
        //                    TimeIn = DateTime.Now,
        //                    OpportunityId = null,
        //                    RoomId = eventGroup.RoomReservation.RoomId
        //}).ToList();


        //var response = new ParticipantEventMapDto
        //{
        //    CurrentEvent = participantEventMapDto.CurrentEvent,
        //    Participants = _childSigninRepository.CreateEventParticipants(mpEventParticipantDtoList).Select(Mapper.Map<ParticipantDto>).ToList()
        //};

        //response.Participants.ForEach(p => p.Selected = true);

        //        return response;
        //    }
    }
}