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
        private readonly IParticipantRepository _participantRepository;

        private readonly int _defaultEarlyCheckinPeriod;
        private readonly int _defaultLateCheckinPeriod;

        public SignInLogic(IEventRepository eventRepository, IApplicationConfiguration applicationConfiguration, IConfigRepository configRepository,
            IGroupRepository groupRepository, IRoomRepository roomRepository, IParticipantRepository participantRepository)
        {
            _eventRepository = eventRepository;
            _applicationConfiguration = applicationConfiguration;
            _groupRepository = groupRepository;
            _roomRepository = roomRepository;
            _participantRepository = participantRepository;

            _defaultEarlyCheckinPeriod = int.Parse(configRepository.GetMpConfigByKey("DefaultEarlyCheckIn").Value);
            _defaultLateCheckinPeriod = int.Parse(configRepository.GetMpConfigByKey("DefaultLateCheckIn").Value);
        }

        // JPC - might also be able to add some overloads and whatnot to handle other kinds of sign in?

        // this is probably going to need - 
        // 1. site id
        // 2. participant id
        // 3. date?
        // 4. ac signin yes/no
        public List<MpEventParticipantDto> SignInParticipants(ParticipantEventMapDto participantEventMapDto)
        {
            var mpEventParticipantList = new List<MpEventParticipantDto>();

            // we need to get the events on a per person basis, when doing this - the previous check for duplicate signins needs to 
            // be pulling all events on the day
            foreach (var participant in participantEventMapDto.Participants.Where(r => r.DuplicateSignIn == false && r.Selected == true))
            {
                // if a participant is under 3, by axiom, they do not get signed into adventure club
                var participantAge = System.DateTime.Now - participant.DateOfBirth;
                var underThreeSignIn = (participantAge.Days/365 < 3) ? true : false;

                var adventureClubSignIn = (participantEventMapDto.ServicesAttended == 2);

                // get the events they can sign into
                var eligibleEvents = GetSignInEvents(participantEventMapDto.CurrentEvent.EventSiteId, adventureClubSignIn, underThreeSignIn);

                // // TODO: Add check here to make sure there's a group assigned for the event room, otherwise set an error --
                // this actually needs to be run after the attempt to sign in, so we can post-mortem why they couldn't sign in
                var eventRooms = GetSignInEventRooms(participant.GroupId.GetValueOrDefault(), eligibleEvents.Select(r => r.EventId).ToList());

                foreach (var eligibleEvent in eligibleEvents)
                {
                    if (!(eventRooms.Exists(r => r.EventId == eligibleEvent.EventId)))
                    {
                        // set participant status as age/grade group not assigned for the event - this may need to be reworked, since
                        // it should only be doing this for events they're being signed into - do we want to account for bumping between
                        // events based on this? or do a check once they're assigned to an event? Or is this actually necessary?
                    }
                }

                // TODO: Add invalid event time check in here - might live a little father up?

                // the first condition is for non-ac signins, second is for ac signins 
                // we will need to set error messages on them as part of a sub-function
                if (eligibleEvents.Any(r => r.ParentEventId != null))
                {
                    mpEventParticipantList.AddRange(AssignParticipantToRoomsNonAc(eventRooms, eligibleEvents, participant));
                }
                else
                {
                    mpEventParticipantList.AddRange(AssignParticipantToRoomsWithAc(eventRooms, eligibleEvents, participant));
                }

                AuditSigninIssues(participantEventMapDto, mpEventParticipantList, eligibleEvents);
            }

            return mpEventParticipantList;
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
            // return all event rooms which match up to groups on the events
            var eventGroups = _eventRepository.GetEventGroupsByGroupIdAndEventIds(groupId, eventIds);
            var eventRoomIds = eventGroups.Select(r => r.RoomReservationId.GetValueOrDefault()).ToList();
            var eventRooms = _roomRepository.GetEventRoomsByEventRoomIds(eventRoomIds).ToList();

            return eventRooms;
        }

        // this is going to have a list of the rooms on an event that a participant can be signed into - 
        // this essentially filters down and finds the actual room, as opposed to potential rooms
        // we also need to set some error status codes here, yo
        public List<MpEventParticipantDto> AssignParticipantToRoomsNonAc(List<MpEventRoomDto> eventRoomDtos, List<MpEventDto> eventDtos, ParticipantDto participant)
        {
            var mpEventParticipantRecords = new List<MpEventParticipantDto>();

            // sort the events in ascending time - want to start with the first service event for this
            eventDtos = eventDtos.OrderBy(r => r.EventStartDate).ToList();

            foreach (var serviceEvent in eventDtos)
            {
                // need to make sure that there is only a single event room returned on these...
                var eventRoom = eventRoomDtos.First(r => r.EventId == serviceEvent.EventId);

                if (eventRoom.Capacity > eventRoom.SignedIn)
                {
                    // assign the room to the participant here
                    mpEventParticipantRecords.Add(new MpEventParticipantDto
                    {   
                        RoomId = eventRoom.RoomId,
                        RoomName = eventRoom.RoomName
                    });
                }
                else
                {
                    var mpEventParticipant = ProcessBumpingRules(serviceEvent.EventId, eventRoom.EventRoomId.GetValueOrDefault());
                    mpEventParticipantRecords.Add(mpEventParticipant);
                }
            }

            return mpEventParticipantRecords;
        }

        // this is going to have a list of the rooms on an event that a participant can be signed into - 
        // this essentially filters down and finds the actual room, as opposed to potential rooms
        public List<MpEventParticipantDto> AssignParticipantToRoomsWithAc(List<MpEventRoomDto> eventRoomDtos, List<MpEventDto> eventDtos, ParticipantDto participant)
        {
            // search for the ac event and rooms first - if we find a room on the first event, switch to the service event and look for a room
            // on it. If we don't find them for both, we punt and they get a rock
            var mpEventParticipantRecords = new List<MpEventParticipantDto>();

            // sort the events in ascending time - want to start with the first service event for this
            var acEventDtos = eventDtos.OrderBy(r => r.EventStartDate).ToList();

            foreach (var acEvent in acEventDtos)
            {
                // need to make sure that there is only a single event room returned on these...
                var eventRoom = eventRoomDtos.First(r => r.EventId == acEvent.EventId);

                if (eventRoom.Capacity > eventRoom.SignedIn)
                {
                    // assign the room to the participant here
                    mpEventParticipantRecords.Add(new MpEventParticipantDto
                    {
                        RoomId = eventRoom.RoomId,
                        RoomName = eventRoom.RoomName
                    });
                }
                else
                {
                    var mpEventParticipant = ProcessBumpingRules(acEvent.EventId, eventRoom.EventRoomId.GetValueOrDefault());
                    mpEventParticipantRecords.Add(mpEventParticipant);
                }
            }

            return mpEventParticipantRecords;
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
                // this will have a null room assignment and should show up in the code as not being assigned to a room
                return new MpEventParticipantDto();
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

            return new MpEventParticipantDto();
        }

        public void CheckForDuplicateSignIns(ParticipantEventMapDto participantEventMapDto)
        {
            // get the list of events for the site
            var dateToday = DateTime.Parse(DateTime.Now.ToShortDateString());

            var dailyEvents = _eventRepository.GetEvents(dateToday, dateToday, participantEventMapDto.CurrentEvent.EventSiteId, true)
                .Where(r => CheckEventTimeValidity(r)).OrderBy(r => r.EventStartDate);

            // go through each event and its sub event or parent event and check if its sub event or parent event
            // has this participant signed in
            var eventIds = new List<int>();
            foreach (var signinEvent in dailyEvents)
            {
                eventIds.Add(signinEvent.EventId);

                // if parent event get subevent else get parent event
                if (signinEvent.ParentEventId == null)
                {
                    var subEvent = _eventRepository.GetSubeventByParentEventId(signinEvent.EventId, _applicationConfiguration.AdventureClubEventTypeId);
                    if (subEvent != null)
                    {
                        eventIds.Add(subEvent.EventId);
                    }
                }
                else
                {
                    eventIds.Add(signinEvent.ParentEventId.Value);
                }
            }

            foreach (var eventItemId in eventIds)
            {
                var signedInParticipants = _participantRepository.GetEventParticipantsByEventAndParticipant(
                    eventItemId,
                    participantEventMapDto.Participants.Select(r => r.ParticipantId).ToList());

                foreach (var participant in participantEventMapDto.Participants)
                {
                    if (signedInParticipants.Any(r => r.ParticipantId == participant.ParticipantId))
                    {
                        participant.DuplicateSignIn = true;
                        participant.SignInErrorMessage = $"{participant.Nickname} is already signed in for this event.";
                    }
                }
            }
        }

        // foreach participant, if either of their event participant records do not have an assigned room,
        // determine what the problem is and set the error message correctly
        public void AuditSigninIssues(ParticipantEventMapDto participantEventMapDto, List<MpEventParticipantDto> mpEventParticipantDtos, List<MpEventDto> eligibleEvents)
        {
            foreach (var participant in participantEventMapDto.Participants)
            {
                var unassignedParticipants = mpEventParticipantDtos.Where(r => r.HasRoomAssignment == false && r.ParticipantId == participant.ParticipantId).ToList();

                if (unassignedParticipants.Any(r => r.GroupId == null))
                {
                    participant.SignInErrorMessage = $"Age/Grade Group Not Assigned. {participant.Nickname} is not in a Kids Club Group (DOB: {participant.DateOfBirth.ToShortDateString() })";
                }

                if (unassignedParticipants.Any(r => r.HasRoomAssignment == false && r.GroupId.HasValue))
                {
                    var mpEventParticipant = unassignedParticipants.First(r => r.HasRoomAssignment == false);

                    // select rooms for the events...see if there were any rooms on the event for the participant
                    var eventRooms = _roomRepository.GetEventRoomsByEventRoomIds(eligibleEvents.Select(r => r.EventId).ToList());

                    // probably need to look at the service count, too
                    if (!(eventRooms.Any(r => r.AllowSignIn == true)))
                    {
                        // since we have multiple events we can possibly sign into, it does not make sense to record the event name
                        // they could not sign into
                        var group = mpEventParticipant.GroupId.HasValue ? _groupRepository.GetGroup(null, mpEventParticipant.GroupId.Value) : null;
                        participant.SignInErrorMessage = $"There are no {@group?.Name} rooms open for {participant.Nickname}";
                    }
                }
            }
        }
    }
}