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

            _defaultEarlyCheckinPeriod = int.Parse(configRepository.GetMpConfigByKey("DefaultEarlyCheckIn").Value);
            _defaultLateCheckinPeriod = int.Parse(configRepository.GetMpConfigByKey("DefaultLateCheckIn").Value);
        }

        // JPC - might also be able to add some overloads and whatnot to handle other kinds of sign in?

        // this is probably going to need - 
        // 1. site id
        // 2. participant id
        // 3. date?
        // 4. ac signin yes/no
        public void SignInParticipant(int siteId, bool adventureClubSignIn, bool underThreeSignIn, int groupId)
        {
            // first step, get all eligible events -- this is for the site, at least for the next two events
            var dailyEvents = GetSignInEvents(siteId, adventureClubSignIn, underThreeSignIn);

            // next, get all event rooms for the event groups that are on those events


            //var eligibleRooms = _roomRepository.GetEventRoomsByEventGroup(groupId, dailyEvents.Select(r => r.EventId).ToList());



            // should be get groups for eligible events here
            var eventIds = dailyEvents.Select(r => r.EventId).ToList();
            var eventGroups = _eventRepository.GetEventGroupsForEvent(eventIds);

            //var groupId = 1;// JPC - crap value

            // get rooms for the event groups - this has to map the room id and the event id
            var eventRooms = _roomRepository.GetEventRoomsByEventGroup(groupId, eventIds);

            // run rules on groups -- if we get a hit on the first room, sign them in and 
            // move on, otherwise then load and run bumping rules
            foreach (var eventRoom in eventRooms)
            {
                if (eventRoom.SignedIn < eventRoom.Capacity)
                {

                }
                else
                {
                    var bumpingRooms = _roomRepository.GetBumpingRulesByRoomId(eventRoom.EventRoomId.GetValueOrDefault());

                    foreach (var item in bumpingRooms)
                    {
                        // run the rules - probaby needs to be broken out into different functions
                    }
                }
            }

            // return the dataset - probably at least the event rooms and event ids

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
            var eventGroups = _eventRepository.GetEventGroupsForEvent(groupId, 




            return null;
        } 

        /*** Helper Functions ***/
        private bool CheckEventTimeValidity(MpEventDto mpEventDto)
        {
            // check to see if the event's start is equal to or later than the time minus the offset period
            var offsetPeriod = DateTime.Now.AddMinutes(-(mpEventDto.EarlyCheckinPeriod ?? _defaultEarlyCheckinPeriod));
            return mpEventDto.EventStartDate >= offsetPeriod;
        }


    }
}