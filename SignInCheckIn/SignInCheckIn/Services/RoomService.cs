using System.Collections.Generic;
using System.Linq;
using AutoMapper;
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

        private readonly int _kcAgesAttributeTypeId;
        private readonly int _kcGradesAttributeTypeId;
        private readonly int _kcBirthMonthsAttributeTypeId;
        private readonly int _kcNurseryAgesAttributeTypeId;
        private readonly int _kcNurseryAgeAttributeId;

        public RoomService(IEventRepository eventRepository, IRoomRepository roomRepository, IAttributeRepository attributeRepository, IGroupRepository groupRepository)
        {
            _eventRepository = eventRepository;
            _roomRepository = roomRepository;
            _attributeRepository = attributeRepository;
            _groupRepository = groupRepository;

            // TODO Get rid of hard-coded IDs, pull from config file
            _kcAgesAttributeTypeId = 102;
            _kcGradesAttributeTypeId = 104;
            _kcBirthMonthsAttributeTypeId = 103;
            _kcNurseryAgesAttributeTypeId = 105;
            _kcNurseryAgeAttributeId = 9014;
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

        public List<AgeGradeDto> GetEventRoomAgesAndGrades(string authenticationToken, int eventId, int roomId)
        {
            var ages = _attributeRepository.GetAttributesByAttributeTypeId(_kcAgesAttributeTypeId, authenticationToken);
            var grades = _attributeRepository.GetAttributesByAttributeTypeId(_kcGradesAttributeTypeId, authenticationToken);
            var birthMonths = _attributeRepository.GetAttributesByAttributeTypeId(_kcBirthMonthsAttributeTypeId, authenticationToken);
            var nurseryMonths = _attributeRepository.GetAttributesByAttributeTypeId(_kcNurseryAgesAttributeTypeId, authenticationToken);

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

            var response = new List<AgeGradeDto>();
            var maxSort = 0;
            ages.OrderBy(a => a.SortOrder).ToList().ForEach(a =>
            {
                response.Add(new AgeGradeDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    SortOrder = a.SortOrder,
                    Ranges = (a.Id == _kcNurseryAgeAttributeId ? nurseryMonths : birthMonths).Select(r => new AgeGradeDto.AgeRangeDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Selected =
                            eventGroups.Exists(
                                e =>
                                    a.Id == _kcNurseryAgeAttributeId
                                        ? e.Group.HasNurseryMonth() && e.Group.NurseryMonth.Id == r.Id
                                        : !e.Group.HasNurseryMonth() && e.Group.BirthMonth.Id == r.Id),
                        SortOrder = r.SortOrder,
                        TypeId = a.Type.Id
                    }).OrderBy(r => r.SortOrder).ToList(),
                    TypeId = a.Type.Id
                });
                maxSort = a.SortOrder;
            });

            grades.OrderBy(g => g.SortOrder).ToList().ForEach(g =>
            {
                response.Add(new AgeGradeDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Selected = eventGroups.Exists(e => g.Id == e.Group.AgeRange.Id),
                    SortOrder = g.SortOrder + maxSort,
                    TypeId = g.Type.Id
                });
            });

            return response;
        }
    }
}