using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AutoMapper;
using Crossroads.Utilities.Services.Interfaces;
using Crossroads.Web.Common.MinistryPlatform;
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
        private readonly IApiUserRepository _apiUserRepository;

        public RoomService(IEventRepository eventRepository,
                           IRoomRepository roomRepository,
                           IAttributeRepository attributeRepository,
                           IGroupRepository groupRepository,
                           IApplicationConfiguration applicationConfiguration,
                           IApiUserRepository apiUserRepository)
        {
            _eventRepository = eventRepository;
            _roomRepository = roomRepository;
            _attributeRepository = attributeRepository;
            _groupRepository = groupRepository;
            _applicationConfiguration = applicationConfiguration;
            _apiUserRepository = apiUserRepository;
        }

        public List<EventRoomDto> GetLocationRoomsByEventId(string authenticationToken, int eventId)
        {
            var result = _roomRepository.GetManageRoomsListData(eventId);

            var mpEventRooms = result[0].Select(r => r.ToObject<MpEventRoomDto>()).ToList();
            var eventGroups = result[1].Select(r => r.ToObject<MpEventGroupDto>()).ToList();
            var mpGroupAttributes = result[2].Select(r => r.ToObject<MpGroupAttributeDto>()).ToList();
            var allAttributes = result[3].Select(r => r.ToObject<MpAttributeDto>()).ToList();

            // set the attributes on the group here - can't do it in the db, as they're calculated properties I think
            foreach (var mpGroupAttribute in mpGroupAttributes)
            {
                // try to match the attribute on the groups
                if (mpGroupAttribute.AttributeTypeId == _applicationConfiguration.AgesAttributeTypeId)
                {
                    foreach (var eventGroup in eventGroups.Where(r => r.GroupId == mpGroupAttribute.GroupId))
                    {
                        eventGroup.Group.AgeRange = mpGroupAttribute.GetAttributeDto();
                    }
                }

                // try to match the attribute on the groups
                if (mpGroupAttribute.AttributeTypeId == _applicationConfiguration.GradesAttributeTypeId)
                {
                    foreach (var eventGroup in eventGroups.Where(r => r.GroupId == mpGroupAttribute.GroupId))
                    {
                        eventGroup.Group.Grade = mpGroupAttribute.GetAttributeDto();
                    }
                }

                // try to match the attribute on the groups
                if (mpGroupAttribute.AttributeTypeId == _applicationConfiguration.BirthMonthsAttributeTypeId)
                {
                    foreach (var eventGroup in eventGroups.Where(r => r.GroupId == mpGroupAttribute.GroupId))
                    {
                        eventGroup.Group.BirthMonth = mpGroupAttribute.GetAttributeDto();
                    }
                }

                // try to match the attribute on the groups
                if (mpGroupAttribute.AttributeTypeId == _applicationConfiguration.NurseryAgesAttributeTypeId)
                {
                    foreach (var eventGroup in eventGroups.Where(r => r.GroupId == mpGroupAttribute.GroupId))
                    {
                        eventGroup.Group.NurseryMonth = mpGroupAttribute.GetAttributeDto();
                    }
                }

            }

            var ages = allAttributes.Where(r => r.Type.Id == _applicationConfiguration.AgesAttributeTypeId).ToList();
            var grades = allAttributes.Where(r => r.Type.Id == _applicationConfiguration.GradesAttributeTypeId).ToList();
            var birthMonths = allAttributes.Where(r => r.Type.Id == _applicationConfiguration.BirthMonthsAttributeTypeId).ToList();
            var nurseryMonths = allAttributes.Where(r => r.Type.Id == _applicationConfiguration.NurseryAgesAttributeTypeId).ToList();
            birthMonths.ForEach(m => m.Name = m.Name.Substring(0, 3));

            // Get All Rooms for this Event
            var eventRooms = Mapper.Map<List<MpEventRoomDto>, List<EventRoomDto>>(mpEventRooms);

            // Get All the Event Groups Assigned to each room for this event
            foreach (var eventRoom in eventRooms)
            {
                // Get current event groups with a room reservation for this room
                var eventRoomGroups = eventGroups.Where(r => r.RoomId == eventRoom.RoomId).ToList();

                var agesAndGrades = new List<AgeGradeDto>();

                // Add age ranges
                agesAndGrades.AddRange(GetAgeRangesAndCurrentSelections(ages, nurseryMonths, birthMonths, eventRoomGroups));

                var maxSort = 0;

                if (agesAndGrades.Any())
                {
                    maxSort = agesAndGrades.Select(r => r.SortOrder).Last();
                }

                // Add grade ranges
                agesAndGrades.AddRange(GetGradesAndCurrentSelection(grades, eventRoomGroups, maxSort));

                eventRoom.AssignedGroups = agesAndGrades;
            }

            eventRooms = eventRooms.OrderBy(r => r.RoomName).ToList();

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
            
            // Get All the Event Groups for this Event
            var eventGroups = _eventRepository.GetEventGroupsForEvent(eventRoom.EventId) ?? new List<MpEventGroupDto>();

            // Load up lookups for age ranges, grades, birth months, and nursery months
            var ages = _attributeRepository.GetAttributesByAttributeTypeId(_applicationConfiguration.AgesAttributeTypeId, authenticationToken);
            var grades = _attributeRepository.GetAttributesByAttributeTypeId(_applicationConfiguration.GradesAttributeTypeId, authenticationToken);
            var birthMonths = _attributeRepository.GetAttributesByAttributeTypeId(_applicationConfiguration.BirthMonthsAttributeTypeId, authenticationToken);
            var nurseryMonths = _attributeRepository.GetAttributesByAttributeTypeId(_applicationConfiguration.NurseryAgesAttributeTypeId, authenticationToken);

            var tmpEventRoom = GetEventRoomAgeAndGradeGroups(authenticationToken, eventRoom, eventGroups, ages, grades, birthMonths, nurseryMonths);

            var updatedEventRoom = Mapper.Map<EventRoomDto>(response);
            updatedEventRoom.AssignedGroups = tmpEventRoom.AssignedGroups;
            return updatedEventRoom;
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
            var events = _eventRepository.GetEventAndCheckinSubevents(authenticationToken, eventId);
            var result = _roomRepository.GetSingleRoomGroupsData(eventId, roomId);

            var mpEventRooms = result[0].Select(r => r.ToObject<MpEventRoomDto>()).ToList();
            var eventGroups = result[1].Select(r => r.ToObject<MpEventGroupDto>()).ToList();
            var mpGroupAttributes = result[2].Select(r => r.ToObject<MpGroupAttributeDto>()).ToList();
            var allAttributes = result[3].Select(r => r.ToObject<MpAttributeDto>()).ToList();

            // set the attributes on the group here - can't do it in the db, as they're calculated properties I think
            foreach (var mpGroupAttribute in mpGroupAttributes)
            {
                // try to match the attribute on the groups
                if (mpGroupAttribute.AttributeTypeId == _applicationConfiguration.AgesAttributeTypeId)
                {
                    foreach (var eventGroup in eventGroups.Where(r => r.GroupId == mpGroupAttribute.GroupId))
                    {
                        eventGroup.Group.AgeRange = mpGroupAttribute.GetAttributeDto();
                    }
                }

                // try to match the attribute on the groups
                if (mpGroupAttribute.AttributeTypeId == _applicationConfiguration.GradesAttributeTypeId)
                {
                    foreach (var eventGroup in eventGroups.Where(r => r.GroupId == mpGroupAttribute.GroupId))
                    {
                        eventGroup.Group.Grade = mpGroupAttribute.GetAttributeDto();
                    }
                }

                // try to match the attribute on the groups
                if (mpGroupAttribute.AttributeTypeId == _applicationConfiguration.BirthMonthsAttributeTypeId)
                {
                    foreach (var eventGroup in eventGroups.Where(r => r.GroupId == mpGroupAttribute.GroupId))
                    {
                        eventGroup.Group.BirthMonth = mpGroupAttribute.GetAttributeDto();
                    }
                }

                // try to match the attribute on the groups
                if (mpGroupAttribute.AttributeTypeId == _applicationConfiguration.NurseryAgesAttributeTypeId)
                {
                    foreach (var eventGroup in eventGroups.Where(r => r.GroupId == mpGroupAttribute.GroupId))
                    {
                        eventGroup.Group.NurseryMonth = mpGroupAttribute.GetAttributeDto();
                    }
                }

            }

            var ages = allAttributes.Where(r => r.Type.Id == _applicationConfiguration.AgesAttributeTypeId).ToList();
            var grades = allAttributes.Where(r => r.Type.Id == _applicationConfiguration.GradesAttributeTypeId).ToList();
            var birthMonths = allAttributes.Where(r => r.Type.Id == _applicationConfiguration.BirthMonthsAttributeTypeId).ToList();
            var nurseryMonths = allAttributes.Where(r => r.Type.Id == _applicationConfiguration.NurseryAgesAttributeTypeId).ToList();
            birthMonths.ForEach(m => m.Name = m.Name.Substring(0, 3));

            // Get All Rooms for this Event
            var eventRooms = Mapper.Map<List<MpEventRoomDto>, List<EventRoomDto>>(mpEventRooms);

            // JPC - if this is a new room, we just return a new dto - the real event room record will be created when we save
            if (eventRooms.Count == 0)
            {
                eventRooms.Add(new EventRoomDto
                {
                    EventId = eventId,
                    RoomId = roomId
                });
            }

            // Get All the Event Groups Assigned to each room for this event
            foreach (var eventRoom in eventRooms)
            {
                // Get current event groups with a room reservation for this room
                var eventRoomGroups = eventGroups.Where(r => r.RoomId == eventRoom.RoomId).ToList();

                var agesAndGrades = new List<AgeGradeDto>();

                // Add age ranges (including selected groups) to the response - makes no service calls
                agesAndGrades.AddRange(GetAgeRangesAndCurrentSelections(ages, nurseryMonths, birthMonths, eventRoomGroups));

                var maxSort = 0;

                if (agesAndGrades.Any())
                {
                    maxSort = agesAndGrades.Select(r => r.SortOrder).Last();
                }

                // Add grade ranges (including selected groups) to the response - makes no service calls
                agesAndGrades.AddRange(GetGradesAndCurrentSelection(grades, eventRoomGroups, maxSort));

                eventRoom.AssignedGroups = agesAndGrades;
            }

            // set adventure club status
            var returnEventRoom = eventRooms.OrderByDescending(r => r.AllowSignIn).ThenBy(r => r.RoomName).First();
            var selectedEvent = events.FirstOrDefault(e => e.EventId == (returnEventRoom?.EventId ?? eventId));
            returnEventRoom.AdventureClub = (selectedEvent.EventTypeId == _applicationConfiguration.AdventureClubEventTypeId);

            return returnEventRoom;
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

        public EventRoomDto GetEventRoom(int eventId, int roomId)
        {
            var token = _apiUserRepository.GetToken();
            return GetEventRoom(token, eventId, roomId, false);
        }

        private EventRoomDto GetEventRoom(string token, int eventId, int roomId, bool canCreateEventRoom = true)
        {
            var events = _eventRepository.GetEventAndCheckinSubevents(token, eventId);

            if (events.Count == 0)
            {
                throw new Exception("Event not found for event id: " + eventId + " in GetEventRoom in RoomService");
            }

            var eventRoom = _roomRepository.GetEventRoomForEventMaps(events.Select(e => e.EventId).ToList(), roomId);

            // set to the parent id by default
            var selectedEvent = events.FirstOrDefault(e => e.EventId == (eventRoom?.EventId ?? eventId));
            
            if (canCreateEventRoom && eventRoom == null)
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
            eventRoom.EventId = eventId; // Kind of a hack, but we'll see if it works - looks like the event room event id is not in sync?

            XElement nurseryGroupXml = new XElement("NurseryGroupXml", null);

            var nurseryGroups = eventRoom.AssignedGroups.FindAll(g =>
                                      (g.Selected || g.HasSelectedRanges) && g.TypeId == _applicationConfiguration.AgesAttributeTypeId &&
                                      g.Id == _applicationConfiguration.NurseryAgeAttributeId).ToList();

            foreach (var nurseryGroup in nurseryGroups)
            {
                foreach (var range in nurseryGroup.Ranges)
                {
                    XElement idElement = new XElement("Id", range.Id);
                    XElement typeIdElement = new XElement("TypeId", range.TypeId);
                    XElement selectedElement = new XElement("Selected", range.Selected);

                    nurseryGroupXml.Add(new XElement("Attribute", idElement, typeIdElement, selectedElement));
                }
            }

            XElement yearGroupXml = new XElement("YearGroupXml", null);
            
            var yearGroups = eventRoom.AssignedGroups.FindAll(g =>
                                                                  (g.Selected || g.HasSelectedRanges) && g.TypeId == _applicationConfiguration.AgesAttributeTypeId &&
                                                                  g.Id != _applicationConfiguration.NurseryAgeAttributeId).ToList();

            foreach (var yearGroup in yearGroups)
            {
                foreach (var range in yearGroup.Ranges)
                {
                    XElement yearIdElement = new XElement("YearId", range.Id);
                    XElement yearTypeIdElement = new XElement("YearTypeId", range.TypeId);
                    XElement monthIdElement = new XElement("MonthId", yearGroup.Id);
                    XElement monthIdTypeElement = new XElement("MonthTypeId", yearGroup.TypeId);
                    XElement selectedElement = new XElement("Selected", range.Selected);

                    yearGroupXml.Add(new XElement("Attribute", yearIdElement, yearTypeIdElement, monthIdElement, monthIdTypeElement, selectedElement));
                }
            }

            XElement gradeGroupXml = new XElement("GradeGroupXml", null);
            var gradeGroups = eventRoom.AssignedGroups.Where(r => r.TypeId == _applicationConfiguration.GradesAttributeTypeId).ToList();

            foreach (var gradeGroup in gradeGroups)
            {
                    XElement idElement = new XElement("Id", gradeGroup.Id);
                    XElement typeIdElement = new XElement("TypeId", gradeGroup.TypeId);
                    XElement selectedElement = new XElement("Selected", gradeGroup.Selected);

                    gradeGroupXml.Add(new XElement("Attribute", idElement, typeIdElement, selectedElement));               
            }

            XElement groupXml = new XElement("Groups", nurseryGroupXml, yearGroupXml, gradeGroupXml);

            var result = _roomRepository.SaveSingleRoomGroupsData(authenticationToken, eventRoom.EventId, roomId, groupXml.ToString());
            var mpEventRooms = result[0].Select(r => r.ToObject<MpEventRoomDto>()).ToList();
            var eventRooms = Mapper.Map<List<MpEventRoomDto>, List<EventRoomDto>>(mpEventRooms);

            return eventRooms.First(); 
        }

        [Obsolete("Replaced by the new stored proc - left here for reference")]
        private void UpdateAdventureClubStatusIfNecessary(MpEventDto eventDto, int roomId, string token)
        {
            // we need to figure out if this event is the adventure club event or the service event
            // if it is not the AC event, we need to get the AC event
            if (eventDto.EventTypeId != _applicationConfiguration.AdventureClubEventTypeId)
            {
                eventDto = _eventRepository.GetSubeventByParentEventId(token, eventDto.EventId, _applicationConfiguration.AdventureClubEventTypeId);
            }

            // search to see if there are existing Event Rooms for the AC subevent
            var eventRoom = _roomRepository.GetEventRoom(eventDto.EventId);
            
            if (eventRoom != null)
            {
                // if there are, set cancelled to false
                eventDto.Cancelled = false;
                _eventRepository.UpdateEvent(token, eventDto);
            }
            else
            {
                // if that are not, set cancelled to true
                // if there are, set cancelled to false
                eventDto.Cancelled = true;
                _eventRepository.UpdateEvent(token, eventDto);
            }
        }

        [Obsolete("Replaced by the new stored proc - left here for reference")]
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

        [Obsolete("Replaced by the new stored proc - left here for reference")]
        private void DeleteRoomReservationForOtherEvent(string authenticationToken, EventRoomDto eventRoom, MpEventDto eventDto, int roomId)
        { 
            // Make sure there is not an existing room reservation on the 'other' event
            // (i.e. the AC subevent if creating room reseverations for service event, and vice versa)
            if (eventRoom.AdventureClub)
            {
                // is adventure club, so delete any event rooms on the parent service event
                var oldEventRoom = _roomRepository.GetEventRoom(eventDto.ParentEventId.Value, roomId);
                DeleteCurrentEventGroupsForRoomReservation(authenticationToken, eventDto.ParentEventId.Value, roomId);
                if (oldEventRoom != null)
                {
                    _roomRepository.DeleteEventRoom(authenticationToken, oldEventRoom.EventRoomId.Value);
                }
            }
            else
            {
                // not adventure club, so delete any event rooms on the adventure club event
                var adventureClubSubevent = _eventRepository.GetSubeventByParentEventId(authenticationToken, eventDto.EventId, _applicationConfiguration.AdventureClubEventTypeId);
                var oldEventRoom = _roomRepository.GetEventRoom(adventureClubSubevent.EventId, roomId);
                DeleteCurrentEventGroupsForRoomReservation(authenticationToken, adventureClubSubevent.EventId, roomId);
                if (oldEventRoom != null)
                {
                    _roomRepository.DeleteEventRoom(authenticationToken, oldEventRoom.EventRoomId.Value);
                }
            }
        }

        public List<EventRoomDto> GetAvailableRooms(string token, int roomId, int eventId)
        {
            var events = _eventRepository.GetEventAndCheckinSubevents(token, eventId);

            if (events.Count == 0)
            {
                throw new Exception("Event not found for event id: " + eventId + " in GetEventRoom in RoomService");
            }

            var mpCurrentEventRoom = _roomRepository.GetEventRoomForEventMaps(events.Select(e => e.EventId).ToList(), roomId);
            
            // set to the parent id by default
            var mpEvent = events.FirstOrDefault(e => e.EventId == (mpCurrentEventRoom?.EventId ?? eventId));

            // exclude the origin room from the available rooms
            var mpEventAllRooms = _roomRepository.GetRoomsForEvent(mpEvent.EventId, mpEvent.LocationId);
            var mpEventAvailableRooms = mpEventAllRooms.Where(r => r.RoomId != roomId).ToList();


            // if there is no existing event room for the selected room, create one to have something to
            // attach the bumping rooms to
            if (mpCurrentEventRoom == null)
            {
                mpCurrentEventRoom = mpEventAllRooms.First(r => r.RoomId == roomId);

                var eventRoom = new MpEventRoomDto
                {
                    EventId = eventId,
                    RoomId = mpCurrentEventRoom.RoomId,
                    RoomName = mpCurrentEventRoom.RoomName,
                    RoomNumber = mpCurrentEventRoom.RoomNumber
                };

                var currentEventRoomDto = _roomRepository.CreateOrUpdateEventRoom(null, eventRoom);
                mpCurrentEventRoom.EventRoomId = currentEventRoomDto.EventRoomId;
            }

            var eventRooms = Mapper.Map<List<MpEventRoomDto>, List<EventRoomDto>>(mpEventAvailableRooms);

            // make sure to filter null values
            var eventRoomIds = eventRooms.Select(r => r.EventRoomId).Distinct().Where(r => r != null).ToList();

            if (eventRoomIds.Any(r => r != null) && mpCurrentEventRoom.EventRoomId != null)
            {
                var bumpingRules = _roomRepository.GetBumpingRulesForEventRooms(eventRoomIds, mpCurrentEventRoom.EventRoomId);

                foreach (var rule in bumpingRules)
                {
                    // set the rule id and priority on the matching event room - which is the "to" field, if it's a "bumping" event room
                    foreach (var room in eventRooms.Where(room => room.EventRoomId == rule.ToEventRoomId))
                    {
                        room.BumpingRuleId = rule.BumpingRuleId;
                        room.BumpingRulePriority = rule.PriorityOrder;
                        room.BumpingRuleTypeId = rule.BumpingRuleTypeId;
                    }
                }
            }

            return eventRooms;
        }

        public List<EventRoomDto> UpdateAvailableRooms(string authenticationToken, int eventId, int roomId, List<EventRoomDto> eventRoomDtos)
        {
            var events = _eventRepository.GetEventAndCheckinSubevents(authenticationToken, eventId);
            var sourceEventRoom = _roomRepository.GetEventRoomForEventMaps(events.Select(e => e.EventId).ToList(), roomId);

            if (sourceEventRoom == null)
            {
                throw new Exception("Event Room not found for event " + eventId + " and room " + roomId);
            }

            var bumpingRules = _roomRepository.GetBumpingRulesByRoomId(sourceEventRoom.EventRoomId.GetValueOrDefault());
            var bumpingRuleIds = bumpingRules.Select(r => r.BumpingRuleId).Distinct();
            _roomRepository.DeleteBumpingRules(authenticationToken, bumpingRuleIds);

            var selectedRooms = eventRoomDtos.Where(r => r.BumpingRulePriority != null).ToList();

            var mpBumpingRuleDtos = new List<MpBumpingRuleDto>();

            foreach (var selectedRoom in selectedRooms)
            {
                // create the event room here
                if (selectedRoom.EventRoomId == null)
                {
                    var mpEventRoomDto = Mapper.Map<MpEventRoomDto>(selectedRoom);

                    // we get a new object from this call, but only need the end off it
                    selectedRoom.EventRoomId = _roomRepository.CreateOrUpdateEventRoom(null, mpEventRoomDto).EventRoomId;
                }

                var mpBumpingRuleDto = new MpBumpingRuleDto
                {
                    FromEventRoomId = sourceEventRoom.EventRoomId.GetValueOrDefault(),
                    ToEventRoomId = selectedRoom.EventRoomId.GetValueOrDefault(),
                    PriorityOrder = selectedRoom.BumpingRulePriority.GetValueOrDefault(),
                    BumpingRuleTypeId = selectedRoom.BumpingRuleTypeId.Value
                };

                mpBumpingRuleDtos.Add(mpBumpingRuleDto);
            }

            _roomRepository.CreateBumpingRules(authenticationToken, mpBumpingRuleDtos);

            // pull back the newly created rooms
            return GetAvailableRooms(authenticationToken, roomId, eventId);
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

        public List<MpGroupDto> GetEventUnassignedGroups(string authenticationToken, int eventId)
        {
            var ages = _attributeRepository.GetAttributesByAttributeTypeId(_applicationConfiguration.AgesAttributeTypeId, authenticationToken);
            var grades = _attributeRepository.GetAttributesByAttributeTypeId(_applicationConfiguration.GradesAttributeTypeId, authenticationToken);
            var birthMonths = _attributeRepository.GetAttributesByAttributeTypeId(_applicationConfiguration.BirthMonthsAttributeTypeId, authenticationToken);
            var nurseryMonths = _attributeRepository.GetAttributesByAttributeTypeId(_applicationConfiguration.NurseryAgesAttributeTypeId, authenticationToken);

            // existing groups assigned to a room event
            var eventGroups = _eventRepository.GetEventGroupsForEvent(eventId) ?? new List<MpEventGroupDto>();
            var allGroupsAttributes = ages.Concat(grades)
                                    .Concat(birthMonths)
                                    .Concat(nurseryMonths)
                                    .ToList();

            var unassignedGroups = _groupRepository.GetGroupsByAttribute(authenticationToken, allGroupsAttributes, false);
            unassignedGroups = unassignedGroups.GroupBy(n => n.Id).Select(grp => grp.Last()).OrderByDescending(g => g.Id).ToList();
            eventGroups.ForEach(eg =>
            {
                unassignedGroups.RemoveAll(g => eg.GroupId == g.Id);
            });

            return unassignedGroups;
        }
    }
}