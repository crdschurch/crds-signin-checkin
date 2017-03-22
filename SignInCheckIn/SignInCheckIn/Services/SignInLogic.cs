using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using SignInCheckIn.Services.Interfaces;
using Crossroads.Utilities.Services.Interfaces;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
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
        private readonly IChildSigninRepository _childSigninRepository;

        private readonly int _defaultEarlyCheckinPeriod;
        private readonly int _defaultLateCheckinPeriod;

        public SignInLogic(IEventRepository eventRepository, IApplicationConfiguration applicationConfiguration, IConfigRepository configRepository,
            IGroupRepository groupRepository, IRoomRepository roomRepository, IParticipantRepository participantRepository, IChildSigninRepository childSigninRepository)
        {
            _eventRepository = eventRepository;
            _applicationConfiguration = applicationConfiguration;
            _groupRepository = groupRepository;
            _roomRepository = roomRepository;
            _participantRepository = participantRepository;
            _childSigninRepository = childSigninRepository;

            _defaultEarlyCheckinPeriod = int.Parse(configRepository.GetMpConfigByKey("DefaultEarlyCheckIn").Value);
            _defaultLateCheckinPeriod = int.Parse(configRepository.GetMpConfigByKey("DefaultLateCheckIn").Value);
        }

        public List<ParticipantDto> SignInParticipants(ParticipantEventMapDto participantEventMapDto, List<MpEventDto> currentEvents)
        {
            var mpEventParticipantList = new List<MpEventParticipantDto>();

            // we need to get the events on a per person basis, when doing this - the previous check for duplicate signins needs to 
            // be pulling all events on the day
            foreach (var participant in participantEventMapDto.Participants.Where(r => r.DuplicateSignIn == false && r.Selected == true))
            {
                mpEventParticipantList.AddRange(SignInParticipant(participant, participantEventMapDto, currentEvents));
            }

            // insert event participants here and get their event participant ids
            mpEventParticipantList = _childSigninRepository.CreateEventParticipants(mpEventParticipantList);

            var mappedParticipants = mpEventParticipantList.Select(Mapper.Map<ParticipantDto>).ToList();

            return mappedParticipants;
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

            SyncInvalidSignins(mpEventParticipantList, participant);

            AuditSigninIssues(participantEventMapDto, mpEventParticipantList, eligibleEvents, participant);

            // save the participant if they are selected and have a valid room assignment - moved down here so that we
            // don't sign in multiple kids to a single room over capacity -- also, we want to make sure that 
            // we are using this logic correctly - getting a rock vs. no sign in, so we may still need the
            // mp event participants to be created

            // this conditional will need to be updated so that we save mp event participants regardless of the status of the
            // participant...
            //if (participant.Selected == true && participant.AssignedRoomId != null && mpEventParticipantList.Any())
            //{
            //    mpEventParticipantList = _childSigninRepository.CreateEventParticipants(mpEventParticipantList);
            //}

            //mpEventParticipantList = _childSigninRepository.CreateEventParticipants(mpEventParticipantList);

            return mpEventParticipantList;
        } 

        // parameters determine the behavior of what events we get back
        public List<MpEventDto> EvaluateSignInEvents(int siteId, bool adventureClubSignIn, bool underThree, List<MpEventDto> currentEvents)
        {
            List<MpEventDto> eligibleEvents = new List<MpEventDto>();

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
            var eventRooms = _roomRepository.GetEventRoomsByEventRoomIds(eventRoomIds).Where(r => r.AllowSignIn).ToList();

            return eventRooms;
        }

        // set assignments for non-ac rooms here -- this is not picking up when a partici
        public List<MpEventParticipantDto> AssignParticipantToRoomsNonAc(List<MpEventRoomDto> eventRoomDtos, List<MpEventDto> eventDtos, ParticipantDto participant)
        {
            var mpEventParticipantRecords = new List<MpEventParticipantDto>();

            // sort the events in ascending time - want to start with the first service event for this
            eventDtos = eventDtos.OrderBy(r => r.EventStartDate).ToList();

            foreach (var serviceEvent in eventDtos)
            {
                // need to make sure that there is only a single event room returned on these...
                var eventRoom = eventRoomDtos.FirstOrDefault(r => r.EventId == serviceEvent.EventId);
                if (eventRoom == null)
                {
                    // if there is a not an eligible event room for the child for either event, add a default 
                    // event participant record so they get a rock
                    var mpEventParticipantDto = TranslateParticipantDtoToMpEventParticipantDto(participant, serviceEvent.EventId, null, _applicationConfiguration.ErrorParticipationStatusId);
                    mpEventParticipantRecords.Add(mpEventParticipantDto);
                    continue;
                }

                if (eventRoom.Capacity > (eventRoom.SignedIn + eventRoom.CheckedIn))
                {
                    // consider just passing down the event room?
                    var mpEventParticipantDto = TranslateParticipantDtoToMpEventParticipantDto(participant, eventRoom.EventId, eventRoom.RoomId, _applicationConfiguration.SignedInParticipationStatusId);
                    mpEventParticipantRecords.Add(mpEventParticipantDto);
                }
                else
                {
                    var bumpedRoom = ProcessBumpingRules(serviceEvent.EventId, eventRoom.EventRoomId.GetValueOrDefault());

                    if (bumpedRoom != null)
                    {
                        var mpEventParticipantDto = TranslateParticipantDtoToMpEventParticipantDto(participant, serviceEvent.EventId, bumpedRoom.RoomId, _applicationConfiguration.SignedInParticipationStatusId);
                        mpEventParticipantRecords.Add(mpEventParticipantDto);
                    }
                    // if they have no room assignment, add an unsignedin participant so that their label gets printed appropriately
                    else
                    {
                        var mpEventParticipantDto = TranslateParticipantDtoToMpEventParticipantDto(participant, serviceEvent.EventId, null, _applicationConfiguration.CapacityParticipationStatusId);
                        mpEventParticipantRecords.Add(mpEventParticipantDto);
                    }
                }
            }

            // set the room ids and other data on the participant here
            SetParticipantRoomAssignments(participant, mpEventParticipantRecords);

            // set event participant id here for call number and printing purposes
            if (mpEventParticipantRecords.Any())
            {
                participant.EventParticipantId = mpEventParticipantRecords[0].EventParticipantId;
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

            // look at the ac events first?
            var acEventDtos = eventDtos.Where(r => r.ParentEventId != null).OrderBy(r => r.EventStartDate).ToList();
            var serviceEventDtos = eventDtos.Where(r => r.ParentEventId == null).OrderBy(r => r.EventStartDate).ToList();

            var acEventSignInDataItems = GetEligibleRoomsForEvents(acEventDtos, eventRoomDtos);
            var serviceEventSignInDataItems = GetEligibleRoomsForEvents(serviceEventDtos, eventRoomDtos);

            var signInRooms = FinalizeAcRoomAssignments(acEventSignInDataItems, serviceEventSignInDataItems);

            if (signInRooms.Count == 2)
            {
                foreach (var room in signInRooms)
                {
                    mpEventParticipantRecords.Add(TranslateParticipantDtoToMpEventParticipantDto(participant, room.EventId, room.RoomId, _applicationConfiguration.SignedInParticipationStatusId));
                }

                // set the room ids on the participant here
                SetParticipantRoomAssignments(participant, mpEventParticipantRecords);

                // set event participant id here for call number and printing purposes
                if (mpEventParticipantRecords.Any())
                {
                    participant.EventParticipantId = mpEventParticipantRecords[0].EventParticipantId;
                }
            }
            else
            {
                // for AC signins, we will assume that we're not able to sign in because of capacity - this will be corrected in the audit section if there's actually
                // an error on signing in
                var mpEventParticipantDto = TranslateParticipantDtoToMpEventParticipantDto(participant, 0, null, _applicationConfiguration.CapacityParticipationStatusId);
                mpEventParticipantRecords.Add(mpEventParticipantDto);
            }

            return mpEventParticipantRecords;
        }

        public List<EventRoomSignInData> GetEligibleRoomsForEvents(List<MpEventDto> eventDtos, List<MpEventRoomDto> mpEventRoomDtos)
        {
            List<EventRoomSignInData> eligibleRooms = new List<EventRoomSignInData>();

            foreach (var eventDto in eventDtos)
            {
                // this is to guard against no event rooms being assigned for the group on the event
                if (!mpEventRoomDtos.Any(r => r.EventId == eventDto.EventId))
                {
                    continue;
                }

                var eventRoom = mpEventRoomDtos.First(r => r.EventId == eventDto.EventId);

                if (eventRoom.Capacity > (eventRoom.SignedIn + eventRoom.CheckedIn))
                {
                    eligibleRooms.Add(new EventRoomSignInData
                    {
                        EventId = eventDto.EventId,
                        EventRoomId = eventRoom.EventRoomId.GetValueOrDefault(),
                        ParentEventId = eventDto.ParentEventId,
                        RoomId = eventRoom.RoomId,
                        RoomName = eventRoom.RoomName
                    });
                }
                else
                {
                    var bumpedRoom = ProcessBumpingRules(eventDto.EventId, eventRoom.EventRoomId.GetValueOrDefault());

                    if (bumpedRoom != null)
                    {
                        eligibleRooms.Add(new EventRoomSignInData
                        {
                            EventId = eventDto.EventId,
                            EventRoomId = bumpedRoom.EventRoomId,
                            ParentEventId = eventDto.ParentEventId,
                            RoomId = bumpedRoom.RoomId,
                            RoomName = bumpedRoom.RoomName
                        });
                    }
                }
            }

            return eligibleRooms;
        } 

        // this function actually determines what ac and non-ac rooms a participant is assigned to
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

        // return the event room if there's an event room for them to bump to
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

                if (bumpingRoom.AllowSignIn && bumpingRoom.Capacity > signedAndCheckedIn)
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
        public void AuditSigninIssues(ParticipantEventMapDto participantEventMapDto, List<MpEventParticipantDto> mpEventParticipantDtos, List<MpEventDto> eligibleEvents, ParticipantDto participant)
        {
            if (participant.GroupId == null)
            {
                participant.SignInErrorMessage = $"Age/Grade Group Not Assigned. {participant.Nickname} is not in a Kids Club Group (DOB: {participant.DateOfBirth.ToShortDateString() })";

                foreach (var mpEventParticipantDto in mpEventParticipantDtos)
                {
                    mpEventParticipantDto.ParticipantStatusId = _applicationConfiguration.ErrorParticipationStatusId;
                }
            }

            if (participant.AssignedRoomId == null && participant.GroupId != null)
            {
                // select rooms for the events...see if there were any rooms on the event for the participant
                var eventRooms = _roomRepository.GetRoomsForEvent(eligibleEvents.Select(r => r.EventId).ToList(), participantEventMapDto.CurrentEvent.EventSiteId);

                if (!(eventRooms.Any(r => r.AllowSignIn == true)))
                {
                    // since we have multiple events we can possibly sign into, it does not make sense to record the event name
                    // they could not sign into
                    var group = participant.GroupId.HasValue ? _groupRepository.GetGroup(null, participant.GroupId.Value) : null;
                    participant.SignInErrorMessage = $"There are no {@group?.Name} rooms open for {participant.Nickname}";

                    foreach (var mpEventParticipantDto in mpEventParticipantDtos)
                    {
                        mpEventParticipantDto.ParticipantStatusId = _applicationConfiguration.ErrorParticipationStatusId;
                    }
                }
            }
        }

        /*** Helper Functions ***/
        private bool CheckEventTimeValidity(MpEventDto mpEventDto)
        {
            // check to see if the event's start is equal to or later than the time minus the offset period
            var offsetPeriod = DateTime.Now.AddMinutes(-(mpEventDto.EarlyCheckinPeriod ?? _defaultEarlyCheckinPeriod));
            return mpEventDto.EventStartDate >= offsetPeriod;
        }

        // TODO: this will need to be updated so that we're setting the status based on the participant's ability to sign in, etc
        private MpEventParticipantDto TranslateParticipantDtoToMpEventParticipantDto(ParticipantDto participant, int eventId, int? roomId, int participationStatusId)
        {
            MpEventParticipantDto mpEventParticipantDto = new MpEventParticipantDto
            {
                EventId = eventId,
                ParticipantId = participant.ParticipantId,
                ParticipantStatusId = participationStatusId,
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

        private void SetParticipantRoomAssignments(ParticipantDto participant, List<MpEventParticipantDto> mpEventParticipantRecords)
        {
            // set the room ids and other data on the participant here
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
        }

        // this needs to be updated to handle capacity or error sign in statuses, too...
        public void SyncInvalidSignins(List<MpEventParticipantDto> mpEventParticipantDtoList, ParticipantDto participantDto)
        {
            if (mpEventParticipantDtoList.Any(r => r.HasRoomAssignment == false))
            {
                foreach (var subItem in mpEventParticipantDtoList.Where(r => r.ParticipantId == participantDto.ParticipantId))
                {
                    subItem.RoomId = null;
                }

                participantDto.AssignedRoomId = null;
                participantDto.AssignedRoomName = String.Empty;
                participantDto.AssignedSecondaryRoomId = null;
                participantDto.AssignedSecondaryRoomName = String.Empty;
            }
        }
    }

    // this is used to handle data for both event rooms and bumping rooms
    public class EventRoomSignInData
    {
        public int EventId { get; set; }
        public int EventRoomId { get; set; }
        public int? ParentEventId { get; set; }
        public int RoomId { get; set; }
        public string RoomName { get; set; }
    }
}