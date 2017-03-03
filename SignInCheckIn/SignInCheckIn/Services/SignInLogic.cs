using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignInCheckIn.Services.Interfaces;
using AutoMapper;
using Crossroads.Utilities.Services.Interfaces;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Printing.Utilities.Models;
using Printing.Utilities.Services.Interfaces;
using SignInCheckIn.Models.DTO;

namespace SignInCheckIn.Services
{
    public class SignInLogic : ISignInLogic
    {
        private readonly IEventRepository _eventRepository;
        private readonly IApplicationConfiguration _applicationConfiguration;
        private readonly IGroupRepository _groupRepository;
        private readonly IRoomRepository _roomRepository;

        private readonly int _defaultEarlyCheckinPeriod;
        private readonly int _defaultLateCheckinPeriod;

        public SignInLogic(IEventRepository eventRepository, IApplicationConfiguration applicationConfiguration, IConfigRepository configRepository,
            IGroupRepository groupRepository, IRoomRepository roomRepository)
        {
            _eventRepository = eventRepository;
            _applicationConfiguration = applicationConfiguration;
            _groupRepository = groupRepository;
            _roomRepository = roomRepository;

            _defaultEarlyCheckinPeriod = int.Parse(configRepository.GetMpConfigByKey("DefaultEarlyCheckIn").Value);
            _defaultLateCheckinPeriod = int.Parse(configRepository.GetMpConfigByKey("DefaultLateCheckIn").Value);
        }

        // JPC - might also be able to add some overloads and whatnot to handle other kinds of sign in?

        // this is probably going to need - 
        // 1. site id
        // 2. participant id
        // 3. date?
        // 4. ac signin yes/no
        public List<MpEventParticipantDto> SignInParticipant(int siteId, bool adventureClubSignIn, bool underThreeSignIn, int groupId)
        {
            // first step, get all eligible events -- this is for the site, at least for the next two events
            var dailyEvents = GetSignInEvents(siteId, adventureClubSignIn, underThreeSignIn);

            // next, get all event rooms for the event groups that are on those events
            var eventRooms = GetSignInEventRooms(groupId, dailyEvents.Select(r => r.EventId).ToList());

            if (dailyEvents.Any(r => r.ParentEventId != null))
            {
                return AssignParticipantToRoomsNonAc(eventRooms, dailyEvents);
            }
            else
            {
                return AssignParticipantToRoomsWithAc(eventRooms, dailyEvents);
            }
        }

        // parameters determine the behavior of what events we get back
        public List<MpEventDto> GetSignInEvents(int siteId, bool adventureClubSignIn, bool underThree)
        {
            List<MpEventDto> eligibleEvents = new List<MpEventDto>();

            // we start by getting a list of all events for the day
            var dateToday = DateTime.Parse(DateTime.Now.ToShortDateString());

            var dailyEvents = _eventRepository.GetEvents(dateToday, dateToday, siteId, true)
                .Where(r => CheckEventTimeValidity(r)).OrderBy(r => r.EventStartDate);

            // return only a single event
            if (adventureClubSignIn == false)
            {
                eligibleEvents.Add(dailyEvents.First(r => r.ParentEventId == null));
                return eligibleEvents;
            }

            if (underThree == true)
            {
                var serviceEventSet = dailyEvents.Where(r => r.ParentEventId == null).ToList();

                // we need to get first two event services, and no matching ac events
                for (int i = 0; i < 2; i++)
                {
                    eligibleEvents.Add(serviceEventSet[i]);
                }

                return eligibleEvents;
            }

            if (adventureClubSignIn == true)
            {
                var serviceEventSet = dailyEvents.Where(r => r.ParentEventId == null).ToList();

                // we need to get first two event services, and then the matching ac events - if there's
                // a combination of 1, 1AC, 2, 3, 3AC, 3 and 3 AC are not considered for checkin
                for(int i = 0; i < 2; i++)
                {
                    eligibleEvents.Add(serviceEventSet[i]);

                    if (dailyEvents.Any(r => r.ParentEventId == serviceEventSet[i].EventId))
                    {
                        eligibleEvents.Add(dailyEvents.First(r => r.ParentEventId == serviceEventSet[i].EventId));
                    }
                }

                return eligibleEvents;
            }

            return null;
        }

        public List<MpEventRoomDto> GetSignInEventRooms(int groupId, List<int> eventIds)
        {
            var eventGroups = _eventRepository.GetEventGroupsByGroupIdAndEventIds(groupId, eventIds);
            var eventRoomIds = eventGroups.Select(r => r.RoomReservationId.GetValueOrDefault()).ToList();
            var eventRooms = _roomRepository.GetEventRoomsByEventRoomIds(eventRoomIds).ToList();

            return eventRooms;
        }

        // this is going to have a list of the rooms on an event that a participant can be signed into - 
        // this essentially filters down and finds the actual room, as opposed to potential rooms
        public List<MpEventParticipantDto> AssignParticipantToRoomsNonAc(List<MpEventRoomDto> eventRoomDtos, List<MpEventDto> eventDtos)
        {
            var MpEventParticipantRecords = new List<MpEventParticipantDto>();

            // sort the events in ascending time - want to start with the first service event for this
            eventDtos = eventDtos.OrderBy(r => r.EventStartDate).ToList();

            foreach (var serviceEvent in eventDtos)
            {
                // need to make sure that there is only a single event room returned on these...
                var eventRoom = eventRoomDtos.First(r => r.EventId == serviceEvent.EventId);

                if (eventRoom.Capacity > eventRoom.SignedIn)
                {
                    // assign the room to the participant here
                }
                else
                {
                    var mpEventParticipant = ProcessBumpingRules(serviceEvent.EventId, eventRoom.EventRoomId.GetValueOrDefault());
                }
            }

            return MpEventParticipantRecords;
        }

        // this is going to have a list of the rooms on an event that a participant can be signed into - 
        // this essentially filters down and finds the actual room, as opposed to potential rooms
        public List<MpEventParticipantDto> AssignParticipantToRoomsWithAc(List<MpEventRoomDto> eventRoomDtos, List<MpEventDto> eventDtos)
        {
            // search for the ac event and rooms first - if we find a room on the first event, switch to the service event and look for a room
            // on it. If we don't find them for both, we punt and they get a rock
            var MpEventParticipantRecords = new List<MpEventParticipantDto>();

            // sort the events in ascending time - want to start with the first service event for this
            var acEventDtos = eventDtos.OrderBy(r => r.EventStartDate).ToList();

            foreach (var acEvent in acEventDtos)
            {
                // need to make sure that there is only a single event room returned on these...
                var eventRoom = eventRoomDtos.First(r => r.EventId == acEvent.EventId);

                if (eventRoom.Capacity > eventRoom.SignedIn)
                {
                    // assign the room to the participant here
                }
                else
                {
                    var mpEventParticipant = ProcessBumpingRules(acEvent.EventId, eventRoom.EventRoomId.GetValueOrDefault());
                }
            }

            return MpEventParticipantRecords;
        }

        /*** Helper Functions ***/
        private bool CheckEventTimeValidity(MpEventDto mpEventDto)
        {
            // check to see if the event's start is equal to or later than the time minus the offset period
            var offsetPeriod = DateTime.Now.AddMinutes(-(mpEventDto.EarlyCheckinPeriod ?? _defaultEarlyCheckinPeriod));
            return mpEventDto.EventStartDate >= offsetPeriod;
        }

        // return event participant data if there's a room for them to bump to
        public MpEventParticipantDto ProcessBumpingRules(int eventId, int eventRoomId)
        {
            var bumpingRooms = _roomRepository.GetBumpingRoomsForEventRoom(eventId, eventRoomId);

            // go through the bumping rooms in priority order and get the first one that is open and has capacity
            if (bumpingRooms == null)
            {
                return null;
            }
            foreach (var bumpingRoom in bumpingRooms)
            {
                // check if open and has capacity
                var signedAndCheckedIn = bumpingRoom.CheckedIn + bumpingRoom.SignedIn;

                if (bumpingRoom.AllowSignIn && bumpingRoom.Capacity >= signedAndCheckedIn)
                {
                    return new MpEventParticipantDto
                    {
                        RoomId = bumpingRoom.RoomId,
                        RoomName = bumpingRoom.RoomName
                    };
                }

                //if (!bumpingRoom.AllowSignIn || bumpingRoom.Capacity <= signedAndCheckedIn)
                //{
                //    continue;
                //}
                //else
                //{
                //    return new MpEventParticipantDto
                //    {
                //        RoomId = bumpingRoom.RoomId,
                //        RoomName = bumpingRoom.RoomName
                //    };
                //}
            }

            return null;
        }
    }
}