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
        public List<MpEventParticipantDto> SignInParticipants(ParticipantEventMapDto participantEventMapDto, List<MpEventDto> currentEvents)
        {
            var mpEventParticipantList = new List<MpEventParticipantDto>();

            // we need to get the events on a per person basis, when doing this - the previous check for duplicate signins needs to 
            // be pulling all events on the day
            foreach (var participant in participantEventMapDto.Participants.Where(r => r.DuplicateSignIn == false && r.Selected == true))
            {
                mpEventParticipantList.AddRange(SignInParticipant(participant, participantEventMapDto, currentEvents));
            }

            return mpEventParticipantList;
        }

        public List<MpEventParticipantDto> SignInParticipant(ParticipantDto participant, ParticipantEventMapDto participantEventMapDto, List<MpEventDto> currentEvents)
        {
            var mpEventParticipantList = new List<MpEventParticipantDto>();

            // if a participant is under 3, by axiom, they do not get signed into adventure club
            var participantAge = System.DateTime.Now - participant.DateOfBirth;
            var underThreeSignIn = (participantAge.Days / 365 < 3) ? true : false;

            var adventureClubSignIn = (participantEventMapDto.ServicesAttended == 2);

            // get the events they can sign into
            var eligibleEvents = EvaluateSignInEvents(participantEventMapDto.CurrentEvent.EventSiteId, adventureClubSignIn, underThreeSignIn, currentEvents);

            // // TODO: Add check here to make sure there's a group assigned for the event room, otherwise set an error --
            // this actually needs to be run after the attempt to sign in, so we can post-mortem why they couldn't sign in
            var eventRooms = GetSignInEventRooms(participant.GroupId.GetValueOrDefault(), eligibleEvents.Select(r => r.EventId).ToList());

            //foreach (var eligibleEvent in eligibleEvents)
            //{
            //    if (!(eventRooms.Exists(r => r.EventId == eligibleEvent.EventId)))
            //    {
            //        // set participant status as age/grade group not assigned for the event - this may need to be reworked, since
            //        // it should only be doing this for events they're being signed into - do we want to account for bumping between
            //        // events based on this? or do a check once they're assigned to an event? Or is this actually necessary?
            //    }
            //}

            // TODO: Add invalid event time check in here - might live a little father up?

            // the first condition is for non-ac signins, second is for ac signins 
            // we will need to set error messages on them as part of a sub-function
            if (eligibleEvents.Any(r => r.ParentEventId != null))
            {
                mpEventParticipantList.AddRange(AssignParticipantToRoomsWithAc(eventRooms, eligibleEvents, participant));
            }
            else
            {
                mpEventParticipantList.AddRange(AssignParticipantToRoomsNonAc(eventRooms, eligibleEvents, participant));
            }

            AuditSigninIssues(participantEventMapDto, mpEventParticipantList, eligibleEvents);

            return mpEventParticipantList;
        } 

        // parameters determine the behavior of what events we get back
        public List<MpEventDto> EvaluateSignInEvents(int siteId, bool adventureClubSignIn, bool underThree, List<MpEventDto> currentEvents)
        {
            List<MpEventDto> eligibleEvents = new List<MpEventDto>();

            // we start by getting a list of all events for the day
            var dateToday = DateTime.Parse(DateTime.Now.ToShortDateString());

            // return only a single event
            if (adventureClubSignIn == false)
            {
                eligibleEvents.Add(currentEvents.First(r => r.ParentEventId == null));
                return eligibleEvents;
            }

            if (underThree == true)
            {
                var serviceEventSet = currentEvents.Where(r => r.ParentEventId == null).ToList();

                // we need to get first two event services, and no matching ac events
                for (int i = 0; i < 2; i++)
                {
                    eligibleEvents.Add(serviceEventSet[i]);
                }

                return eligibleEvents;
            }

            if (adventureClubSignIn == true)
            {
                var serviceEventSet = currentEvents.Where(r => r.ParentEventId == null).ToList();

                // we need to get first two event services, and then the matching ac events - if there's
                // a combination of 1, 1AC, 2, 3, 3AC, 3 and 3 AC are not considered for checkin
                for(int i = 0; i < 2; i++)
                {
                    eligibleEvents.Add(serviceEventSet[i]);

                    if (currentEvents.Any(r => r.ParentEventId == serviceEventSet[i].EventId))
                    {
                        eligibleEvents.Add(currentEvents.First(r => r.ParentEventId == serviceEventSet[i].EventId));
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

        // this will assign a person to one or two event rooms, depending on how many events are passed in - there is no bumping between events here, so just
        // look to see how many 
        public List<MpEventParticipantDto> AssignParticipantToRoomsNonAc(List<MpEventRoomDto> eventRoomDtos, List<MpEventDto> eventDtos, ParticipantDto participant)
        {
            var mpEventParticipantRecords = new List<MpEventParticipantDto>();

            // sort the events in ascending time - want to start with the first service event for this
            eventDtos = eventDtos.OrderBy(r => r.EventStartDate).ToList();

            foreach (var serviceEvent in eventDtos)
            {
                // need to make sure that there is only a single event room returned on these...
                var eventRoom = eventRoomDtos.First(r => r.EventId == serviceEvent.EventId);

                if (eventRoom.Capacity > (eventRoom.SignedIn + eventRoom.CheckedIn))
                {
                    // consider just passing down the event room?
                    var mpEventParticipantDto = TranslateParticipantDtoToMpEventParticipantDto(participant, eventRoom.EventId, eventRoom.RoomId);
                    mpEventParticipantRecords.Add(mpEventParticipantDto);
                }
                else
                {
                    var bumpedRoom = ProcessBumpingRules(serviceEvent.EventId, eventRoom.EventId);

                    if (bumpedRoom != null)
                    {
                        var mpEventParticipantDto = TranslateParticipantDtoToMpEventParticipantDto(participant, serviceEvent.EventId, bumpedRoom.RoomId);
                        mpEventParticipantRecords.Add(mpEventParticipantDto);
                    }
                }
            }

            // set the room ids and other data on the participant here? - what order do we want the room ids in?
            for (int i = 0; i < mpEventParticipantRecords.Count; i++)
            {
                if (i == 0)
                {
                    participant.AssignedRoomId = mpEventParticipantRecords[i].RoomId;
                    participant.AssignedRoomName = mpEventParticipantRecords[i].RoomName;
                }

                if (i == 1)
                {
                    participant.AssignedSecondaryRoomId = mpEventParticipantRecords[i].RoomId;
                    participant.AssignedSecondaryRoomName = mpEventParticipantRecords[i].RoomName;
                }
            }

            // TODO: We need to coalesce the participant records, so if they can't sign into one event or the other, we get a 
            // correct message
            return mpEventParticipantRecords;
        }

        // this is going to have a list of the rooms on an event that a participant can be signed into - 
        // this essentially filters down and finds the actual room, as opposed to potential rooms
        public List<MpEventParticipantDto> AssignParticipantToRoomsWithAc(List<MpEventRoomDto> eventRoomDtos, List<MpEventDto> eventDtos, ParticipantDto participant)
        {
            // search for the ac event and rooms first - if we find a room on the first event, switch to the service event and look for a room
            // on it. If we don't find them for both, we punt and they get a rock
            var mpEventParticipantRecords = new List<MpEventParticipantDto>();

            // look at the ac events first?
            var acEventDtos = eventDtos.Where(r => r.ParentEventId != null).OrderBy(r => r.EventStartDate).ToList();
            var serviceEventDtos = eventDtos.Where(r => r.ParentEventId == null).OrderBy(r => r.EventStartDate).ToList();

            var acEventSignInDataItems = GetEligibleRoomsForEvents(acEventDtos, eventRoomDtos);
            var serviceEventSignInDataItems = GetEligibleRoomsForEvents(serviceEventDtos, eventRoomDtos);

            var signInRooms = FinalizeAcRoomAssignments(acEventSignInDataItems, serviceEventSignInDataItems);

            foreach (var room in signInRooms)
            {
                mpEventParticipantRecords.Add(TranslateParticipantDtoToMpEventParticipantDto(participant, room.EventId, room.RoomId));
            }

            //foreach (var acEvent in acEventDtos)
            //{
            //    // need to make sure that there is only a single event room returned on these...
            //    var eventRoom = eventRoomDtos.First(r => r.EventId == acEvent.EventId);

            //    if (eventRoom.Capacity > eventRoom.SignedIn)
            //    {
            //        acEventRooms.Add(new EventRoomSignInData
            //        {
            //            EventId = acEvent.EventId,
            //            EventRoomId = eventRoom.EventRoomId.GetValueOrDefault(),
            //            RoomId = eventRoom.RoomId,
            //            RoomName = eventRoom.RoomName
            //        });
            //    }
            //    else
            //    {
            //        var bumpedRoom = ProcessBumpingRules(acEvent.EventId, eventRoom.EventId);

            //        if (bumpedRoom != null)
            //        {
            //            acEventRooms.Add(new EventRoomSignInData
            //            {
            //                EventId = acEvent.EventId,
            //                EventRoomId = bumpedRoom.EventRoomId,
            //                RoomId = bumpedRoom.RoomId,
            //                RoomName = bumpedRoom.RoomName
            //            });
            //        }
            //    }
            //}

            // if we are already signed into an ac event, go ahead and try to sign into the other service event -- this feels kinda soorta wrong. 
            // it feels like we need to look at both events and determine which one we can sign into before actually trying to sign in...
            //var serviceEvents = eventDtos.Where(r => r.EventId != mpEventParticipantRecords[0].EventId);

            // some obsolete code here
            //if (bumpedRoom != null)
            //{
            //    acEventRooms.Add(bumpedRoom);
            //}

            //if (bumpedRoom != null)
            //{
            //    var mpEventParticipantDto = TranslateParticipantDtoToMpEventParticipantDto(participant, acEvent.EventId, bumpedRoom.RoomId);
            //    mpEventParticipantRecords.Add(mpEventParticipantDto);
            //}
            //else
            //{
            //    // if we are not able to sign into this AC event, look at the next AC event
            //    //continue;
            //}

            // set the room ids and other data on the participant here? - what order do we want the room ids in?
            for (int i = 0; i < mpEventParticipantRecords.Count; i++)
            {
                if (i == 0)
                {
                    participant.AssignedRoomId = mpEventParticipantRecords[i].RoomId;
                    participant.AssignedRoomName = mpEventParticipantRecords[i].RoomName;
                }

                if (i == 1)
                {
                    participant.AssignedSecondaryRoomId = mpEventParticipantRecords[i].RoomId;
                    participant.AssignedSecondaryRoomName = mpEventParticipantRecords[i].RoomName;
                }
            }

            return mpEventParticipantRecords;
        }

        public List<EventRoomSignInData> GetEligibleRoomsForEvents(List<MpEventDto> eventDtos, List<MpEventRoomDto> mpEventRoomDtos)
        {
            List<EventRoomSignInData> eligibleRooms = new List<EventRoomSignInData>();

            foreach (var acEvent in eventDtos)
            {
                var eventRoom = mpEventRoomDtos.First(r => r.EventId == acEvent.EventId);

                if (eventRoom.Capacity > (eventRoom.SignedIn + eventRoom.CheckedIn))
                {
                    eligibleRooms.Add(new EventRoomSignInData
                    {
                        EventId = acEvent.EventId,
                        EventRoomId = eventRoom.EventRoomId.GetValueOrDefault(),
                        ParentEventId = acEvent.ParentEventId,
                        RoomId = eventRoom.RoomId,
                        RoomName = eventRoom.RoomName
                    });
                }
                else
                {
                    var bumpedRoom = ProcessBumpingRules(acEvent.EventId, eventRoom.EventRoomId.GetValueOrDefault());

                    if (bumpedRoom != null)
                    {
                        eligibleRooms.Add(new EventRoomSignInData
                        {
                            EventId = acEvent.EventId,
                            EventRoomId = bumpedRoom.EventRoomId,
                            ParentEventId = acEvent.ParentEventId,
                            RoomId = bumpedRoom.RoomId,
                            RoomName = bumpedRoom.RoomName
                        });
                    }
                }
            }

            return eligibleRooms;
        } 

        public List<EventRoomSignInData> FinalizeAcRoomAssignments(List<EventRoomSignInData> acEventRoomDtos, List<EventRoomSignInData> serviceEventRoomDtos)
        {
            // do some logic here to figure out which one to sign into
            var returnEventRooms = new List<EventRoomSignInData>();

            // if there is only one eligible room in the service event set, automatically assign it and add the
            // ac room tied to the other event
            if (serviceEventRoomDtos.Count == 1 && acEventRoomDtos.Count == 2)
            {
                returnEventRooms.Add(serviceEventRoomDtos.First());

                // add the ac event room that is not tied to the service event
                if (acEventRoomDtos.Any(r => r.ParentEventId != serviceEventRoomDtos.First().EventId))
                {
                    returnEventRooms.Add(acEventRoomDtos.First(r => r.ParentEventId != serviceEventRoomDtos.First().EventId));
                }

                return returnEventRooms;
            }

            // just pick one of the service events and the other ac event
            if (serviceEventRoomDtos.Count == 2 && acEventRoomDtos.Count == 2)
            {
                returnEventRooms.Add(serviceEventRoomDtos.First());
                returnEventRooms.Add(acEventRoomDtos.First(r => r.ParentEventId != returnEventRooms.First().EventId));

                return returnEventRooms;
            }

            // add the first service event room that is not equal to the only 
            if (serviceEventRoomDtos.Count == 2 && acEventRoomDtos.Count == 1)
            {
                returnEventRooms.Add(serviceEventRoomDtos.First(r => r.EventId != acEventRoomDtos[0].ParentEventId));
                returnEventRooms.Add(acEventRoomDtos.First(r => r.ParentEventId != returnEventRooms.First().EventId));

                return returnEventRooms;
            }

            // if none of the above cases are met, just return an empty set
            return returnEventRooms;
        } 

        /*** Helper Functions ***/
        private bool CheckEventTimeValidity(MpEventDto mpEventDto)
        {
            // check to see if the event's start is equal to or later than the time minus the offset period
            var offsetPeriod = DateTime.Now.AddMinutes(-(mpEventDto.EarlyCheckinPeriod ?? _defaultEarlyCheckinPeriod));
            return mpEventDto.EventStartDate >= offsetPeriod;
        }

        // return event participant data if there's a room for them to bump to
        // TODO: Possibly switch this to returning a room id?
        //public MpEventParticipantDto ProcessBumpingRules(int eventId, int eventRoomId)
        public MpBumpingRoomsDto ProcessBumpingRules(int eventId, int eventRoomId)
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
                    return bumpingRoom;
                }
            }

            return null;
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

        // TODO: Possibly use automapper here
        private MpEventParticipantDto TranslateParticipantDtoToMpEventParticipantDto(ParticipantDto participant, int eventId, int roomId)
        {
            MpEventParticipantDto mpEventParticipantDto = new MpEventParticipantDto
            {
                EventId = eventId,
                ParticipantId = participant.ParticipantId,
                ParticipantStatusId = 3, // TODO: Switch to live config value; Status ID of 3 = "Attended"
                FirstName = participant.FirstName,
                LastName = participant.LastName,
                Nickname = participant.Nickname,
                TimeIn = DateTime.Now,
                OpportunityId = null,
                RoomId = roomId,
                GroupId = participant.GroupId
            };

            return mpEventParticipantDto;
        }
    }

    public class EventRoomSignInData
    {
        public int EventId { get; set; }
        public int EventRoomId { get; set; }
        public int? ParentEventId { get; set; }
        public int RoomId { get; set; }
        public string RoomName { get; set; }
    }
}