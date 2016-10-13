using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Web;
using MinistryPlatform.Translation.Repositories.Interfaces;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Services.Interfaces;

namespace SignInCheckIn.Services
{
    public class RoomService : IRoomService
    {
        private readonly IEventRepository _eventRespository;
        private readonly IRoomRepository _roomRepository;

        public RoomService(IEventRepository eventRepository, IRoomRepository roomRepository)
        {
            _eventRespository = eventRepository;
            _roomRepository = roomRepository;
        }

        public List<EventRoomDto> GetLocationRoomsByEventId(int eventId)
        {
            // first, need to get the event in order to get the location
            var mpEvent = _eventRespository.GetEventById(eventId);

            // then, call the get rooms with the location id
            //var rooms = _roomRepository.GetRoomsForEvent(eventId);

            return null;
        }
    }
}