using System;
using System.Collections.Generic;
using System.Linq;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;

namespace MinistryPlatform.Translation.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly IApiUserRepository _apiUserRepository;
        private readonly IMinistryPlatformRestRepository _ministryPlatformRestRepository;
        private readonly List<string> _eventRoomColumns;

        public RoomRepository(IApiUserRepository apiUserRepository,
            IMinistryPlatformRestRepository ministryPlatformRestRepository)
        {
            _apiUserRepository = apiUserRepository;
            _ministryPlatformRestRepository = ministryPlatformRestRepository;

            _eventRoomColumns = new List<string>
            {
                "Event_Rooms.Event_Room_ID",
                "Event_Rooms.Event_ID",
                "Event_Rooms.Room_ID",
                "Room_ID_Table.Room_Name",
                "Room_ID_Table.Room_Number",
                "Event_Rooms.Allow_Checkin",
                "Event_Rooms.Volunteers",
                "Event_Rooms.Capacity"
            };
        }

        public List<MpRoomDto> GetRoomsForEvent(int eventId, int congregationId)
        {
            var apiUserToken = _apiUserRepository.GetToken();

            var columnList = new List<string>
            {
                "Room_ID",
                "Room_Name",
                "Room_Number",
                
                //"Event_ID",
                //"Event_Title",
                //"Event_Start_Date",
                //"Event_Type_ID_Table.Event_Type",
                //"Congregation_ID_Table.Congregation_Name"
            };



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

            throw new NotImplementedException();
        }

        public MpEventRoomDto CreateOrUpdateEventRoom(string authenticationToken, MpEventRoomDto eventRoom)
        {
            MpEventRoomDto response;
            if (eventRoom.EventRoomId.HasValue)
            {
                response = _ministryPlatformRestRepository.UsingAuthenticationToken(authenticationToken).Update(eventRoom, _eventRoomColumns);
            }
            else
            {
                response = _ministryPlatformRestRepository.UsingAuthenticationToken(authenticationToken).Create(eventRoom, _eventRoomColumns);
            }

            return response;
            //return response?.EventRoomId == null ? null : GetEventRoom(response.EventRoomId.Value);
        }

        public MpEventRoomDto GetEventRoom(int eventId, int roomId)
        {
            var result =
                _ministryPlatformRestRepository.UsingAuthenticationToken(_apiUserRepository.GetToken())
                    .Search<MpEventRoomDto>($"Event_Rooms.Event_ID = {eventId} AND Event_Rooms.Room_ID = {roomId}", _eventRoomColumns);
            return result == null || !result.Any() ? null : result.First();
        }

        public MpEventRoomDto GetEventRoom(int eventRoomId)
        {
            return _ministryPlatformRestRepository.UsingAuthenticationToken(_apiUserRepository.GetToken()).Get<MpEventRoomDto>(eventRoomId, _eventRoomColumns);
        }
    }
}
