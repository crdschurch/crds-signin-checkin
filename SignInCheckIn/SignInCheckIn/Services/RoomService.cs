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
    public class RoomService : IRoomService
    {
        private readonly IEventRepository _eventRespository;
        private readonly IRoomRepository _roomRepository;
        private readonly IAttributeRepository _attributeRepository;

        private readonly int _kcAgesAttributeTypeId;
        private readonly int _kcGradesAttributeTypeId;
        private readonly int _kcBirthMonthsAttributeTypeId;
        private readonly int _kcNurseryAgesAttributeTypeId;
        private readonly int _kcNurseryAgeAttributeId;

        public RoomService(IEventRepository eventRepository, IRoomRepository roomRepository, IAttributeRepository attributeRepository)
        {
            _eventRespository = eventRepository;
            _roomRepository = roomRepository;
            _attributeRepository = attributeRepository;

            // TODO Get rid of hard-coded IDs, pull from config file
            _kcAgesAttributeTypeId = 102;
            _kcGradesAttributeTypeId = 104;
            _kcBirthMonthsAttributeTypeId = 103;
            _kcNurseryAgesAttributeTypeId = 105;
            _kcNurseryAgeAttributeId = 9014;
        }

        public List<EventRoomDto> GetLocationRoomsByEventId(int eventId)
        {
            var mpEvent = _eventRespository.GetEventById(eventId);
            var mpEventRooms = _roomRepository.GetRoomsForEvent(mpEvent.EventId, mpEvent.LocationId);

            return Mapper.Map<List<MpEventRoomDto>, List<EventRoomDto>>(mpEventRooms);
        }

        public EventRoomDto CreateOrUpdateEventRoom(string authenticationToken, EventRoomDto eventRoom)
        {
            var response = _roomRepository.CreateOrUpdateEventRoom(authenticationToken, Mapper.Map<MpEventRoomDto>(eventRoom));

            return Mapper.Map<EventRoomDto>(response);
        }

        public List<AgeGradeDto> GetEventRoomAgesAndGrades(string authenticationToken, int eventId, int roomId, int? eventRoomId)
        {
            var ages = _attributeRepository.GetAttributesByAttributeTypeId(_kcAgesAttributeTypeId, authenticationToken);
            var grades = _attributeRepository.GetAttributesByAttributeTypeId(_kcGradesAttributeTypeId, authenticationToken);
            var birthMonths = _attributeRepository.GetAttributesByAttributeTypeId(_kcBirthMonthsAttributeTypeId, authenticationToken);
            var nurseryMonths = _attributeRepository.GetAttributesByAttributeTypeId(_kcNurseryAgesAttributeTypeId, authenticationToken);

            // TODO Need to get existing room reservations to set 'Selected' properly when we need to edit

            var response = new List<AgeGradeDto>();
            var maxSort = 0;
            ages.OrderBy(a => a.SortOrder).ToList().ForEach(a =>
            {
                response.Add(new AgeGradeDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    // TODO This will be true if ALL ranges are selected for the room reservation
                    Selected = false,
                    SortOrder = a.SortOrder,
                    Ranges = (a.Id == _kcNurseryAgeAttributeId ? nurseryMonths : birthMonths).Select(r => new AgeGradeDto.AgeRangeDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        // TODO This will be true if this particular range is selected for the room reservation
                        Selected = false,
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
                    // TODO This will be true if the Grade is selected
                    Selected = false,
                    SortOrder = g.SortOrder + maxSort,
                    TypeId = g.Type.Id
                });
            });

            return response;
        }
    }
}