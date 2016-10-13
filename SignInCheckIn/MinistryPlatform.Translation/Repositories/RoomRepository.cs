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

    }
}
