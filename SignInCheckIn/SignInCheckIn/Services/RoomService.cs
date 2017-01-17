using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Crossroads.Utilities.Services.Interfaces;
using MinistryPlatform.Translation.Extensions;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Services.Interfaces;

namespace SignInCheckIn.Services
{
    public class RoomService : IRoomService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IAttributeRepository _attributeRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IApplicationConfiguration _applicationConfiguration;

        public RoomService(IEventRepository eventRepository,
                           IRoomRepository roomRepository,
                           IAttributeRepository attributeRepository,
                           IGroupRepository groupRepository,
                           IApplicationConfiguration applicationConfiguration)
        {
            _eventRepository = eventRepository;
            _roomRepository = roomRepository;
            _attributeRepository = attributeRepository;
            _groupRepository = groupRepository;
            _applicationConfiguration = applicationConfiguration;
        }

        public List<EventRoomDto> GetLocationRoomsByEventId(string authenticationToken, int eventId)
        {
            var mpEvent = _eventRepository.GetEventById(eventId);

            // Get All the Event Groups for this Event
            var eventGroups = _eventRepository.GetEventGroupsForEvent(mpEvent.EventId) ?? new List<MpEventGroupDto>();

            // Load up lookups for age ranges, grades, birth months, and nursery months
            var ages = _attributeRepository.GetAttributesByAttributeTypeId(_applicationConfiguration.AgesAttributeTypeId, authenticationToken);
            var grades = _attributeRepository.GetAttributesByAttributeTypeId(_applicationConfiguration.GradesAttributeTypeId, authenticationToken);
            var birthMonths = _attributeRepository.GetAttributesByAttributeTypeId(_applicationConfiguration.BirthMonthsAttributeTypeId, authenticationToken);
            var nurseryMonths = _attributeRepository.GetAttributesByAttributeTypeId(_applicationConfiguration.NurseryAgesAttributeTypeId, authenticationToken);

            // Get All Rooms for this Event
            var mpEventRooms = _roomRepository.GetRoomsForEvent(mpEvent.EventId, mpEvent.LocationId);
            var eventRooms = Mapper.Map<List<MpEventRoomDto>, List<EventRoomDto>>(mpEventRooms);

            // Get All the Event Groups Assigned to each room for this event
            foreach (var eventRoom in eventRooms)
            {
                var tmpEventRoom = GetEventRoomAgeAndGradeGroups(authenticationToken, eventRoom, eventGroups, ages, grades, birthMonths, nurseryMonths);
                eventRoom.AssignedGroups = tmpEventRoom.AssignedGroups;
            }

            return eventRooms;
        }

        public EventRoomDto CreateOrUpdateEventRoom(string authenticationToken, EventRoomDto eventRoom)
        {
            UpdateSubEventRoom(authenticationToken, eventRoom);
            return UpdateMainEventRoom(authenticationToken, eventRoom);
        }

        private EventRoomDto UpdateMainEventRoom(string authenticationToken, EventRoomDto eventRoom)
        {
            var response = _roomRepository.CreateOrUpdateEventRoom(authenticationToken, Mapper.Map<MpEventRoomDto>(eventRoom));
            return Mapper.Map<EventRoomDto>(response);
        }

        private void UpdateSubEventRoom(string authenticationToken, EventRoomDto eventRoom)
        {
            // look to see if there is an AC event for this event and room
            var acEvents = _eventRepository.GetSubeventsForEvents(new List<int> { eventRoom.EventId }, _applicationConfiguration.AdventureClubEventTypeId);
            var acEvent = acEvents != null && acEvents.Any() ? acEvents.FirstOrDefault() : null;

            // if there is an ac event see if it has the room
            if (acEvent == null) return;
            // if it has the room update the room
            var acEventRoom = _roomRepository.GetEventRoom(acEvent.EventId, eventRoom.RoomId);
            if (acEventRoom == null) return;
            acEventRoom.Capacity = eventRoom.Capacity;
            acEventRoom.Volunteers = eventRoom.Volunteers;
            acEventRoom.AllowSignIn = eventRoom.AllowSignIn;
            _roomRepository.CreateOrUpdateEventRoom(authenticationToken, Mapper.Map<MpEventRoomDto>(acEventRoom));
        }

        public EventRoomDto GetEventRoomAgesAndGrades(string authenticationToken, int eventId, int roomId)
        {
            // Get the EventRoom, or the Room if no EventRoom
            var selectedEventRoom = GetEventRoom(authenticationToken, eventId, roomId);

            // Get All the Event Groups for this Event
            var eventGroups = _eventRepository.GetEventGroupsForEvent(eventId) ?? new List<MpEventGroupDto>();

            // Load up lookups for age ranges, grades, birth months, and nursery months
            var ages = _attributeRepository.GetAttributesByAttributeTypeId(_applicationConfiguration.AgesAttributeTypeId, authenticationToken);
            var grades = _attributeRepository.GetAttributesByAttributeTypeId(_applicationConfiguration.GradesAttributeTypeId, authenticationToken);
            var birthMonths = _attributeRepository.GetAttributesByAttributeTypeId(_applicationConfiguration.BirthMonthsAttributeTypeId, authenticationToken);
            var nurseryMonths = _attributeRepository.GetAttributesByAttributeTypeId(_applicationConfiguration.NurseryAgesAttributeTypeId, authenticationToken);

            // Get All the Event Groups Assigned to this room for this event
            return GetEventRoomAgeAndGradeGroups(authenticationToken, selectedEventRoom, eventGroups, ages, grades, birthMonths, nurseryMonths);
        }

        private EventRoomDto GetEventRoomAgeAndGradeGroups(string authenticationToken, EventRoomDto eventRoom, List<MpEventGroupDto> eventGroups, 
            IEnumerable<MpAttributeDto> ages, IEnumerable<MpAttributeDto> grades, List<MpAttributeDto> birthMonths, List<MpAttributeDto> nurseryMonths)
        {
            // Frontend wants months like "Jan" and "Feb", not "January" and "February" - trim them down here, but we may want to move this to frontend in the future
            birthMonths.ForEach(m => m.Name = m.Name.Substring(0, 3));

            // Get current event groups with a room reservation for this room
            var eventRoomGroups = GetEventGroupsWithRoomReservationForEvent(authenticationToken, eventGroups, eventRoom.RoomId);

            var agesAndGrades = new List<AgeGradeDto>();

            // Add age ranges (including selected groups) to the response
            agesAndGrades.AddRange(GetAgeRangesAndCurrentSelections(ages, nurseryMonths, birthMonths, eventRoomGroups));

            var maxSort = agesAndGrades.Select(r => r.SortOrder).Last();

            // Add grade ranges (including selected groups) to the response
            agesAndGrades.AddRange(GetGradesAndCurrentSelection(grades, eventRoomGroups, maxSort));

            eventRoom.AssignedGroups = agesAndGrades;

            return eventRoom;
        }

        public List<AgeGradeDto> GetGradeAttributes(string authenticationToken)
        {
            var grades = _attributeRepository.GetAttributesByAttributeTypeId(_applicationConfiguration.GradesAttributeTypeId, authenticationToken);
            return GetGradesAndCurrentSelection(grades, new List<MpEventGroupDto>(), 0).ToList();
        }

        private EventRoomDto GetEventRoom(string token, int eventId, int roomId)
        {
            var events = _eventRepository.GetEventAndCheckinSubevents(token, eventId);

            if (events.Count == 0)
            {
                throw new Exception("Event not found for event id: " + eventId + " in GetEventRoom in RoomService");
            }

            // set to the parent id by default
            var selectedEvent = events.First(r => r.ParentEventId == null);

            // this will need to be updated once we're looking at subevents other than AC events,
            // as part of the refactor
            var eventGroups = _eventRepository.GetEventGroupsForEvent(eventId) ?? new List<MpEventGroupDto>();
            foreach (var eventItem in events)
            {
                eventGroups = GetEventGroupsWithRoomReservationForEvent(token, eventGroups, roomId);

                // if there are any event groups on the event, that is the "active"
                // event or subevent for that room
                if (eventGroups.Count > 0)
                {
                    selectedEvent = eventItem;
                    break;
                }
            }

            //var eventRoom = _roomRepository.GetEventRoomForEventMaps(eventIds, roomId); <-- remove this


            var eventRoom = _roomRepository.GetEventRoom(selectedEvent.EventId, roomId);

            if (eventRoom == null)
            {
                var room = _roomRepository.GetRoom(roomId);
                if (room == null)
                {
                    throw new ApplicationException($"Could not locate room with id {roomId}");
                }

                eventRoom = new MpEventRoomDto
                {
                    EventId = selectedEvent.EventId,
                    RoomId = room.RoomId,
                    RoomName = room.RoomName,
                    RoomNumber = room.RoomNumber
                };
            }

            var returnRoomDto = Mapper.Map<EventRoomDto>(eventRoom);

            // during the refactor, update this to actually set the event or subevent type on the room
            returnRoomDto.AdventureClub = (selectedEvent.EventTypeId == _applicationConfiguration.AdventureClubEventTypeId);

            return returnRoomDto;
        }

        private List<MpEventGroupDto> GetEventGroupsWithRoomReservationForEvent(string authenticationToken, List<MpEventGroupDto> eventGroups, int roomId)
        {
            if (eventGroups.Any())
            {
                eventGroups = eventGroups.FindAll(e => e.HasRoomReservation() && e.RoomReservation.RoomId == roomId);
                if (eventGroups.Any())
                {
                    var groups = _groupRepository.GetGroups(authenticationToken, eventGroups.Select(e => e.Group.Id), true);

                    eventGroups.ForEach(e =>
                    {
                        e.Group = groups.Find(g => g.Id == e.Group.Id);
                    });
                }
            }
            return eventGroups;
        }

        private IEnumerable<AgeGradeDto> GetAgeRangesAndCurrentSelections(IEnumerable<MpAttributeDto> ages, List<MpAttributeDto> nurseryMonths, List<MpAttributeDto> birthMonths, List<MpEventGroupDto> eventGroups)
        {
            var response = new List<AgeGradeDto>();
            ages.OrderBy(a => a.SortOrder).ToList().ForEach(a =>
            {
                response.Add(new AgeGradeDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    SortOrder = a.SortOrder,
                    Ranges = (a.Id == _applicationConfiguration.NurseryAgeAttributeId ? nurseryMonths : birthMonths).Select(r => new AgeGradeDto.AgeRangeDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Selected = a.Id == _applicationConfiguration.NurseryAgeAttributeId ?
                            eventGroups.HasMatchingNurseryMonth(r.Id) :
                            eventGroups.HasMatchingBirthMonth(a.Id, r.Id),
                        SortOrder = r.SortOrder,
                        TypeId = r.Type.Id,
                        EventGroupIds = a.Id == _applicationConfiguration.NurseryAgeAttributeId ?
                            eventGroups.GetMatchingNurseryMonths(r.Id).Select(g => g.Group.Id).ToList() :
                            eventGroups.GetMatchingBirthMonths(a.Id, r.Id).Select(g => g.Group.Id).ToList()
                    }).OrderBy(r => r.SortOrder).ToList(),
                    TypeId = a.Type.Id
                });
            });

            return response;
        }

        private static IEnumerable<AgeGradeDto> GetGradesAndCurrentSelection(IEnumerable<MpAttributeDto> grades, List<MpEventGroupDto> eventGroups, int maxSort)
        {
            var response = new List<AgeGradeDto>();
            grades.OrderBy(g => g.SortOrder).ToList().ForEach(g =>
            {
                response.Add(new AgeGradeDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Selected = eventGroups.HasMatchingGradeGroup(g.Id),
                    SortOrder = g.SortOrder + maxSort,
                    TypeId = g.Type.Id,
                    EventGroupId = eventGroups.GetMatchingGradeGroups(g.Id).Select(e => e.Group.Id).FirstOrDefault()
                });
            });
            return response;
        }

        public EventRoomDto UpdateEventRoomAgesAndGrades(string authenticationToken, int eventId, int roomId, EventRoomDto eventRoom)
        {
            var eventDto = _eventRepository.GetEventById(eventId);
            eventRoom.AdventureClub = eventDto.ParentEventId.HasValue && eventDto.EventTypeId == _applicationConfiguration.AdventureClubEventTypeId;

            // Start by deleting all current event groups for this room reservation (if any)
            DeleteCurrentEventGroupsForRoomReservation(authenticationToken, eventId, roomId);

            // Check to see if anything is selected on input if not and this was a AC event then cancel
            if (!eventRoom.AssignedGroups.Exists(g => g.Selected || g.HasSelectedRanges))
            {
                // Cancel the AC event as there is no longer a room associated with it
                if (!eventRoom.AdventureClub) return eventRoom;

                eventDto.Cancelled = true;
                _eventRepository.UpdateEvent(authenticationToken, eventDto);
                eventRoom.AdventureClub = false;
                return eventRoom;
            }

            // Get the existing eventRoom, if any
            var existingEventRoom = _roomRepository.GetEventRoom(eventId, roomId) ?? new MpEventRoomDto
            {
                EventId = eventId,
                RoomId = roomId,
                AllowSignIn = true
            };

            // Create the room reservation, if needed
            if (!existingEventRoom.EventRoomId.HasValue)
            {
                var created = _roomRepository.CreateOrUpdateEventRoom(authenticationToken, Mapper.Map<MpEventRoomDto>(existingEventRoom));
                eventRoom.EventRoomId = created.EventRoomId;
                eventRoom.EventId = eventId;
                eventRoom.RoomId = roomId;
            }
            else
            {
                // This is needed in case the frontend does not send the EventRoomId (for instance, when multiple
                // updates are made on the page, but the frontend does not update its model with the new event room id)
                eventRoom.EventRoomId = existingEventRoom.EventRoomId;
            }

            // Create nursery event groups
            CreateEventGroups(authenticationToken,
                              eventRoom,
                              eventRoom.AssignedGroups.FindAll(
                                  g =>
                                      (g.Selected || g.HasSelectedRanges) && g.TypeId == _applicationConfiguration.AgesAttributeTypeId &&
                                      g.Id == _applicationConfiguration.NurseryAgeAttributeId), true);

            // Create age event groups
            CreateEventGroups(authenticationToken,
                              eventRoom,
                              eventRoom.AssignedGroups.FindAll(
                                  g =>
                                      (g.Selected || g.HasSelectedRanges) && g.TypeId == _applicationConfiguration.AgesAttributeTypeId &&
                                      g.Id != _applicationConfiguration.NurseryAgeAttributeId), true);

            // Create grade event groups
            CreateEventGroups(authenticationToken,
                              eventRoom,
                              eventRoom.AssignedGroups.FindAll(g => (g.Selected || g.HasSelectedRanges) && g.TypeId == _applicationConfiguration.GradesAttributeTypeId), false);

            // If an AC event room make sure the AC event is not cancelled
            if (!eventRoom.AdventureClub) return eventRoom;
            eventDto.Cancelled = false;
            _eventRepository.UpdateEvent(authenticationToken, eventDto);

            return eventRoom;
        }

        private void CreateEventGroups(string authenticationToken, EventRoomDto eventRoom, List<AgeGradeDto> selectedGroups, bool isAgeGroup)
        {
            // Create a list of attributes corresponding to the selected groups
            var attributes = GetMpAttributesForSelectedAges(selectedGroups).ToList();
            if (!attributes.Any())
            {
                return;
            }

            // Now get all the groups matching these attributes - but then need to filter it further, as there could be "extras"
            // in this result.  For instance, looking for groups with the January birth month attribute is going to return 
            // Nursery January groups, as well as Ages 1-5 January groups.  So since GetGroupsByAttribute gets us a superset of
            // what we need, do a FindAll to get only those that have the same age range.  Could have potentially done this
            // in the group repository, but would have made that method more complex.
            var groups =
                _groupRepository.GetGroupsByAttribute(authenticationToken, attributes, true)
                    .FindAll(
                        g =>
                            isAgeGroup
                                ? g.HasAgeRange() && (g.HasNurseryMonth() || g.HasBirthMonth()) &&
                                  selectedGroups.Exists(
                                      sg =>
                                          (sg.Selected || sg.HasSelectedRanges) && sg.Id == g.AgeRange.Id &&
                                          sg.Ranges.Exists(r => r.Selected && r.Id == (g.HasNurseryMonth() ? g.NurseryMonth.Id : g.BirthMonth.Id)))
                                : g.HasGrade())
                    .Select(g => new MpEventGroupDto {EventId = eventRoom.EventId, GroupId = g.Id, RoomReservationId = eventRoom.EventRoomId})
                    .ToList();

            _eventRepository.CreateEventGroups(authenticationToken, groups);
        }

        private static IEnumerable<MpAttributeDto> GetMpAttributesForSelectedAges(List<AgeGradeDto> ages)
        {
            var selectedAges = ages.FindAll(a => a.Selected && !a.HasRanges).Select(a => new MpAttributeDto
            {
                Id = a.Id,
                Name = a.Name,
                SortOrder = a.SortOrder,
                Type = new MpAttributeTypeDto
                {
                    Id = a.TypeId
                }
            });

            var selectedRanges = ages.FindAll(a => a.HasSelectedRanges).Select(a => a.Ranges.FindAll(r => r.Selected).Select(r => new MpAttributeDto
            {
                Id = r.Id,
                Name = r.Name,
                SortOrder = r.SortOrder,
                Type = new MpAttributeTypeDto
                {
                    Id = r.TypeId,
                    Name = a.Name
                }
            })).SelectMany(x =>
            {
                var mpAttributeDtos = x as MpAttributeDto[] ?? x.ToArray();
                return mpAttributeDtos;
            });

            return selectedAges.Concat(selectedRanges).ToList();
        }

        private void DeleteCurrentEventGroupsForRoomReservation(string authenticationToken, int eventId, int roomId)
        {
            var currentEventGroups = _eventRepository.GetEventGroupsForEventRoom(eventId, roomId);
            if (currentEventGroups.Any())
            {
                _eventRepository.DeleteEventGroups(authenticationToken, currentEventGroups.Select(g => g.Id));
            }
        }

        public List<EventRoomDto> GetAvailableRooms(int roomId, int eventId)
        {
            var mpEvent = _eventRepository.GetEventById(eventId);

            // exclude the origin room from the available rooms
            var mpEventAllRooms = _roomRepository.GetRoomsForEvent(mpEvent.EventId, mpEvent.LocationId);
            var mpEventAvailableRooms = mpEventAllRooms.Where(r => r.RoomId != roomId).ToList();
            var mpCurrentEventRoom = mpEventAllRooms.First(r => r.RoomId == roomId);

            var eventRooms = Mapper.Map<List<MpEventRoomDto>, List<EventRoomDto>>(mpEventAvailableRooms);

            // make sure to filter null values
            var eventRoomIds = eventRooms.Select(r => r.EventRoomId).Distinct().Where(r => r != null).ToList();

            if (eventRoomIds.Any(r => r != null))
            {
                var bumpingRules = _roomRepository.GetBumpingRulesForEventRooms(eventRoomIds, mpCurrentEventRoom.EventRoomId);

                foreach (var rule in bumpingRules)
                {
                    // set the rule id and priority on the matching event room - which is the "to" field, if it's a "bumping" event room
                    foreach (var room in eventRooms.Where(room => room.EventRoomId == rule.ToEventRoomId))
                    {
                        room.BumpingRuleId = rule.BumpingRuleId;
                        room.BumpingRulePriority = rule.PriorityOrder;
                    }
                }
            }

            return eventRooms;
        }

        public List<EventRoomDto> UpdateAvailableRooms(string authenticationToken, int eventId, int roomId, List<EventRoomDto> eventRoomDtos)
        {
            var sourceEventRoom = _roomRepository.GetEventRoom(eventId, roomId);

            if (sourceEventRoom == null)
            {
                throw new Exception("Event Room not found for event " + eventId + " and room " + roomId);
            }

            var bumpingRules = _roomRepository.GetBumpingRulesByRoomId(sourceEventRoom.EventRoomId.GetValueOrDefault());
            var bumpingRuleIds = bumpingRules.Select(r => r.BumpingRuleId).Distinct();
            _roomRepository.DeleteBumpingRules(authenticationToken, bumpingRuleIds);

            var selectedRooms = eventRoomDtos.Where(r => r.BumpingRulePriority != null).ToList();

            List<MpBumpingRuleDto> mpBumpingRuleDtos = new List<MpBumpingRuleDto>();

            foreach (var selectedRoom in selectedRooms)
            {
                // create the event room here
                if (selectedRoom.EventRoomId == null)
                {
                    var mpEventRoomDto = Mapper.Map<MpEventRoomDto>(selectedRoom);

                    // we get a new object from this call, but only need the end off it
                    selectedRoom.EventRoomId = _roomRepository.CreateOrUpdateEventRoom(null, mpEventRoomDto).EventRoomId;
                }

                MpBumpingRuleDto mpBumpingRuleDto = new MpBumpingRuleDto
                {
                    FromEventRoomId = sourceEventRoom.EventRoomId.GetValueOrDefault(),
                    ToEventRoomId = selectedRoom.EventRoomId.GetValueOrDefault(),
                    PriorityOrder = selectedRoom.BumpingRulePriority.GetValueOrDefault(),
                    BumpingRuleTypeId = 1
                };

                mpBumpingRuleDtos.Add(mpBumpingRuleDto);
            }

            _roomRepository.CreateBumpingRules(authenticationToken, mpBumpingRuleDtos);

            // pull back the newly created rooms
            return GetAvailableRooms(roomId, eventId);
        }

        public EventRoomDto CreateOrUpdateAdventureClubRoom(string authenticationToken, EventRoomDto eventRoom)
        {
            var parentEvent = _eventRepository.GetEventById(eventRoom.EventId);

            // probably needs to have the parent event id passed down?
            var subEvents = _eventRepository.GetEventAndCheckinSubevents(authenticationToken, eventRoom.EventId);

            // 20 = "Adventure Club"
            // if there are no AC events for that event, create one
            if (subEvents.All(r => r.EventTypeId != 20)) // switch to config value
            {
                MpEventDto mpEventDto = new MpEventDto();
                mpEventDto.EventTitle = $"Adventure Club for Event {eventRoom.EventId}";
                mpEventDto.ParentEventId = eventRoom.EventId;
                mpEventDto.CongregationId = parentEvent.CongregationId;
                mpEventDto.ProgramId = parentEvent.ProgramId;
                mpEventDto.PrimaryContact = parentEvent.PrimaryContact;
                mpEventDto.MinutesForSetup = parentEvent.MinutesForSetup;
                mpEventDto.MinutesForCleanup = parentEvent.MinutesForCleanup;
                mpEventDto.EventStartDate = parentEvent.EventStartDate;
                mpEventDto.EventEndDate = parentEvent.EventEndDate;
                mpEventDto.Cancelled = parentEvent.Cancelled;
  
                _eventRepository.CreateSubEvent(authenticationToken, new MpEventDto());

                // assign applicable fields to the event room
            }

            var response = _roomRepository.CreateOrUpdateEventRoom(authenticationToken, Mapper.Map<MpEventRoomDto>(eventRoom));

            return Mapper.Map<EventRoomDto>(response);
        }
    }
}