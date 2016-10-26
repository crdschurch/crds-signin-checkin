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

        public List<EventRoomDto> GetLocationRoomsByEventId(int eventId)
        {
            var mpEvent = _eventRepository.GetEventById(eventId);
            var mpEventRooms = _roomRepository.GetRoomsForEvent(mpEvent.EventId, mpEvent.LocationId);

            return Mapper.Map<List<MpEventRoomDto>, List<EventRoomDto>>(mpEventRooms);
        }

        public EventRoomDto CreateOrUpdateEventRoom(string authenticationToken, EventRoomDto eventRoom)
        {
            var response = _roomRepository.CreateOrUpdateEventRoom(authenticationToken, Mapper.Map<MpEventRoomDto>(eventRoom));

            return Mapper.Map<EventRoomDto>(response);
        }

        public EventRoomDto GetEventRoomAgesAndGrades(string authenticationToken, int eventId, int roomId)
        {
            // Get the EventRoom, or the Room if no EventRoom
            var response = GetEventRoom(eventId, roomId);

            // Load up lookups for age ranges, grades, birth months, and nursery months
            var ages = _attributeRepository.GetAttributesByAttributeTypeId(_applicationConfiguration.AgesAttributeTypeId, authenticationToken);
            var grades = _attributeRepository.GetAttributesByAttributeTypeId(_applicationConfiguration.GradesAttributeTypeId, authenticationToken);
            var birthMonths = _attributeRepository.GetAttributesByAttributeTypeId(_applicationConfiguration.BirthMonthsAttributeTypeId, authenticationToken);
            var nurseryMonths = _attributeRepository.GetAttributesByAttributeTypeId(_applicationConfiguration.NurseryAgesAttributeTypeId, authenticationToken);

            // Frontend wants months like "Jan" and "Feb", not "January" and "February" - trim them down here, but we may want to move this to frontend in the future
            birthMonths.ForEach(m => m.Name = m.Name.Substring(0, 3));

            // Get current event groups with a room reservation for this room
            var eventGroups = GetEventGroupsWithRoomReservationForEvent(authenticationToken, eventId, roomId);

            var agesAndGrades = new List<AgeGradeDto>();

            // Add age ranges (including selected groups) to the response
            agesAndGrades.AddRange(GetAgeRangesAndCurrentSelections(ages, nurseryMonths, birthMonths, eventGroups));

            var maxSort = agesAndGrades.Select(r => r.SortOrder).Last();

            // Add grade ranges (including selected groups) to the response
            agesAndGrades.AddRange(GetGradesAndCurrentSelection(grades, eventGroups, maxSort));

            response.AssignedGroups = agesAndGrades;

            return response;
        }

        private EventRoomDto GetEventRoom(int eventId, int roomId)
        {
            var eventRoom = _roomRepository.GetEventRoom(eventId, roomId);
            if (eventRoom == null)
            {
                var room = _roomRepository.GetRoom(roomId);
                if (room == null)
                {
                    throw new ApplicationException($"Could not locate room with id {roomId}");
                }

                eventRoom = new MpEventRoomDto
                {
                    RoomId = room.RoomId,
                    RoomName = room.RoomName,
                    RoomNumber = room.RoomNumber
                };
            }
            return Mapper.Map<EventRoomDto>(eventRoom);
        }

        private List<MpEventGroupDto> GetEventGroupsWithRoomReservationForEvent(string authenticationToken, int eventId, int roomId)
        {
            var eventGroups = _eventRepository.GetEventGroupsForEvent(eventId) ?? new List<MpEventGroupDto>();
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
                        TypeId = a.Type.Id
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
                    TypeId = g.Type.Id
                });
            });
            return response;
        }
    }
}