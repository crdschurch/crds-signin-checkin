using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;

namespace MinistryPlatform.Translation.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly IApiUserRepository _apiUserRepository;
        private readonly IMinistryPlatformRestRepository _ministryPlatformRestRepository;

        public RoomRepository(IApiUserRepository apiUserRepository,
            IMinistryPlatformRestRepository ministryPlatformRestRepository)
        {
            _apiUserRepository = apiUserRepository;
            _ministryPlatformRestRepository = ministryPlatformRestRepository;
        }

        public List<MpEventRoomDto> GetRoomsForEvent(int eventId, int locationId)
        {
            var apiUserToken = _apiUserRepository.GetToken();





            //// remove the trailing "OR " to avoid syntax error
            //int place = dateOffsetSearchString.LastIndexOf("OR ");
            //dateOffsetSearchString = dateOffsetSearchString.Remove(place, "OR ".Length).Insert(place, "");

            //// 99 is for development - "Oakley Service"
            //return _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken)
            //    .Search<MpRoomDto>("Events.Event_Type_ID=99 AND [Allow_Check-in]=1 AND (" + dateOffsetSearchString + ")", columnList);




            // get the location off of an event
            // Congregation_ID_Table_Location_ID_Table.[Location_ID] AS [Location ID]

            // get the location off of a room
            // Building_ID_Table_Location_ID_Table.[Location_ID] AS [Location ID]

            // get event rooms for the event

            // match event rooms

            var roomColumnList = new List<string>
            {
                "Room_ID",
                "Room_Name",
                "Room_Number",
            };

            var rooms = _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken)
                .Search<MpRoomDto>("Building_ID_Table.Location_ID=" + locationId, roomColumnList);

            var eventRoomColumnList = new List<string>
            {
                "Event_Room_ID"
            };

            var eventRooms = _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken)
                .Search<MpEventRoomDto>("Event_ID=" + eventId, eventRoomColumnList);

            foreach (var room in rooms)
            {
                // populate the room data on an existing room event, or add a new event room dto for that room in the return call
                MpEventRoomDto tempDto = eventRooms.Where(r => r.RoomId == room.RoomId).FirstOrDefault();

                if (tempDto == null)
                {
                    // create a new dto and it to the event rooms list, with default values
                    MpEventRoomDto newEventRoomDto = new MpEventRoomDto
                    {
                        AllowSignIn = false,
                        Capacity = 0,
                        CheckedIn = 0,
                        EventId = eventId,
                        EventRoomId = null,
                        RoomId = room.RoomId,
                        RoomName = room.RoomName,
                        SignedIn = 0,
                        Volunteers = 0
                    };

                    eventRooms.Add(newEventRoomDto);
                }
            }

            return eventRooms;
        }

    }
}
