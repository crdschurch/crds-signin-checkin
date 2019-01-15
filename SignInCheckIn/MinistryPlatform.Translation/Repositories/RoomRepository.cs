using Crossroads.Utilities.Services.Interfaces;
using Crossroads.Web.Common.MinistryPlatform;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace MinistryPlatform.Translation.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly IApiUserRepository _apiUserRepository;
        private readonly IMinistryPlatformRestRepository _ministryPlatformRestRepository;
        private readonly IApplicationConfiguration _applicationConfiguration;
        private readonly List<string> _eventRoomColumns;
        private readonly List<string> _roomColumnList;
        private readonly List<string> _bumpingRuleColumns;

        public RoomRepository(IApiUserRepository apiUserRepository,
            IMinistryPlatformRestRepository ministryPlatformRestRepository,
            IApplicationConfiguration applicationConfiguration)
        {
            _apiUserRepository = apiUserRepository;
            _ministryPlatformRestRepository = ministryPlatformRestRepository;
            _applicationConfiguration = applicationConfiguration;

            _eventRoomColumns = new List<string>
            {
                "Event_Rooms.Event_Room_ID",
                "Event_Rooms.Event_ID",
                "Event_Rooms.Room_ID",
                "Room_ID_Table.Room_Name",
                "Room_ID_Table.Room_Number",
                "Room_ID_Table.KC_Sort_Order",
                "Event_Rooms.Allow_Checkin",
                "Event_Rooms.Volunteers",
                "Event_Rooms.Capacity",
                "Event_Rooms.Label",
                "[dbo].crds_getEventParticipantStatusCount(Event_Rooms.Event_ID, Event_Rooms.Room_ID, 3) AS Signed_In",
                "[dbo].crds_getEventParticipantStatusCount(Event_Rooms.Event_ID, Event_Rooms.Room_ID, 4) AS Checked_In"
            };

            _roomColumnList = new List<string>
            {
                "Room_ID",
                "Room_Name",
                "Room_Number",
                "KC_Sort_Order"
            };

            _bumpingRuleColumns = new List<string>
            {
                "Bumping_Rules_ID",
                "Bumping_Rule_Type_ID",
                "From_Event_Room_ID",
                "To_Event_Room_ID",
                "Priority_Order"
            };
        }

        public List<MpEventRoomDto> GetRoomsForEvent(int eventId, int locationId)
        {
            var eventIds = new List<int> { eventId };
            return GetRoomsForEvent(eventIds, locationId);
        }

        public List<MpEventRoomDto> GetRoomsForEvent(List<int> eventIds, int locationId)
        {
            var apiUserToken = _apiUserRepository.GetDefaultApiClientToken();
            var roomUsageTypeKidsClub = _applicationConfiguration.RoomUsageTypeKidsClub;

            var rooms = _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken)
                .Search<MpRoomDto>("Room_Usage_Type_ID_Table.[Room_Usage_Type_ID] = " + roomUsageTypeKidsClub + " AND Building_ID_Table.Location_ID=" + locationId, _roomColumnList);

            var eventRoomColumnList = new List<string>
            {
                "Event_Room_ID",
                "Event_ID",
                "Event_Rooms.Room_ID",
                "Capacity",
                "Volunteers",
                "Allow_CheckIn",
                "[dbo].crds_getEventParticipantStatusCount(Event_ID, Event_Rooms.Room_ID, 3) AS Signed_In",
                "[dbo].crds_getEventParticipantStatusCount(Event_ID, Event_Rooms.Room_ID, 4) AS Checked_In"
            };

            var filter = eventIds.Count > 1
                ? $"Room_ID_Table.[Room_Usage_Type_ID] = {roomUsageTypeKidsClub} AND Event_ID IN ({string.Join(",", eventIds)})"
                : $"Room_ID_Table.[Room_Usage_Type_ID] = {roomUsageTypeKidsClub} AND Event_ID = {eventIds[0]}";

            var eventRooms = _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken)
                .Search<MpEventRoomDto>(filter, eventRoomColumnList);

            foreach (var room in rooms)
            {
                // populate the room data on an existing room event, or add a new event room dto for that room in the return call
                var tempDto = eventRooms.FirstOrDefault(r => r.RoomId == room.RoomId);

                if (tempDto == null)
                {
                    // create a new dto and it to the event rooms list, with default values
                    var newEventRoomDto = new MpEventRoomDto
                    {
                        AllowSignIn = false,
                        Capacity = 0,
                        CheckedIn = 0,
                        EventId = eventIds[0],
                        EventRoomId = null,
                        RoomId = room.RoomId,
                        RoomName = room.RoomName,
                        SignedIn = 0,
                        Volunteers = 0
                    };

                    eventRooms.Add(newEventRoomDto);
                }
                else
                {
                    // populate room info on room event dto
                    eventRooms.Where(x => x.RoomId == room.RoomId).All(x =>
                    {
                        x.RoomName = room.RoomName;
                        x.RoomNumber = room.RoomNumber;
                        return true;
                    });
                }
            }

            return eventRooms;
        }

        public MpEventRoomDto CreateOrUpdateEventRoom(MpEventRoomDto eventRoom)
        {
            var token = _apiUserRepository.GetDefaultApiClientToken();

            MpEventRoomDto response;
            // TODO Modify all frontend calls that get to this service to ensure they send eventRoomId if there is already a reservation
            // There are several instances where the frontend calls the backend, and an Event_Room gets created, but the frontend model is not
            // being updated with the EventRoomId.  This is causing the backend to create additional Event_Room records for the same room, because
            // we are only checking for the existence of EventRoomId to determine if this is a create or an update.
            if (eventRoom.EventRoomId.HasValue)
            {
                response = _ministryPlatformRestRepository.UsingAuthenticationToken(token).Update(eventRoom, _eventRoomColumns);
            }
            else
            {
                // Make sure it doesn't exist, in case we got a request without an eventRoomId, but there actually is already a reservation for this room
                var existingEventRoom = _ministryPlatformRestRepository.UsingAuthenticationToken(token)
                    .Search<MpEventRoomDto>($"Event_Rooms.Event_ID = {eventRoom.EventId} AND Event_Rooms.Room_ID = {eventRoom.RoomId}", _eventRoomColumns).FirstOrDefault();

                // If we did not find a record for this Event and Room, create one, otherwise update the existing one
                if (existingEventRoom == null)
                {
                    response = _ministryPlatformRestRepository.UsingAuthenticationToken(token).Create(eventRoom, _eventRoomColumns);
                }
                else
                {
                    eventRoom.EventRoomId = existingEventRoom.EventRoomId;
                    response = _ministryPlatformRestRepository.UsingAuthenticationToken(token).Update(eventRoom, _eventRoomColumns);
                }

            }

            return response;
        }

        public MpEventRoomDto GetEventRoom(int eventId, int? roomId = null)
        {
            var apiUserToken = _apiUserRepository.GetDefaultApiClientToken();

            var query = $"Event_Rooms.Event_ID = {eventId}";

            if (roomId != null)
            {
                query = $"{query} AND Event_Rooms.Room_ID = {roomId}";
            }

            return _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken).Search<MpEventRoomDto>(query, _eventRoomColumns).FirstOrDefault();
        }

        // look for an event room when we do not know if the event room is on a parent or child event
        public MpEventRoomDto GetEventRoomForEventMaps(List<int> eventIds, int roomId)
        {
            var apiUserToken = _apiUserRepository.GetDefaultApiClientToken();

            var rooms = _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken).
                Search<MpEventRoomDto>($"Event_Rooms.Event_ID IN ({string.Join(",", eventIds)}) AND Event_Rooms.Room_ID = {roomId}", _eventRoomColumns);

            if (!rooms.Any())
            {
                return null;
            }

            return rooms.FirstOrDefault();
        }

        public MpRoomDto GetRoom(int roomId)
        {
            var apiUserToken = _apiUserRepository.GetDefaultApiClientToken();
            return _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken).Get<MpRoomDto>(roomId, _roomColumnList);
        }

        public List<MpBumpingRuleDto> GetBumpingRulesByRoomId(int fromRoomId)
        {
            var apiUserToken = _apiUserRepository.GetDefaultApiClientToken();
            return _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken).Search<MpBumpingRuleDto>($"From_Event_Room_ID={fromRoomId}", _bumpingRuleColumns);
        }

        public void DeleteBumpingRules(IEnumerable<int> ruleIds)
        {
            var authenticationToken = _apiUserRepository.GetDefaultApiClientToken();
            _ministryPlatformRestRepository.UsingAuthenticationToken(authenticationToken).Delete<MpBumpingRuleDto>(ruleIds);
        }

        public void DeleteEventRoom(string authenticationToken, int eventRoomId)
        {
            _ministryPlatformRestRepository.UsingAuthenticationToken(authenticationToken).Delete<MpEventRoomDto>(eventRoomId);
        }

        public List<MpRoomDto> GetAvailableRoomsBySite(int locationId)
        {
            var apiUserToken = _apiUserRepository.GetDefaultApiClientToken();

            return _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken)
                .Search<MpRoomDto>("Building_ID_Table.Location_ID=" + locationId, _roomColumnList);
        }

        public void CreateBumpingRules(List<MpBumpingRuleDto> bumpingRules)
        {
            var authenticationToken = _apiUserRepository.GetDefaultApiClientToken();
            _ministryPlatformRestRepository.UsingAuthenticationToken(authenticationToken).Create(bumpingRules, _bumpingRuleColumns);
        }

        public List<MpBumpingRuleDto> GetBumpingRulesForEventRooms(List<int?> eventRoomIds, int? fromEventRoomId)
        {
            var queryString = eventRoomIds.Aggregate("(", (current, id) => current + (id + ","));

            queryString = queryString.TrimEnd(',');
            queryString += ")";

            var apiUserToken = _apiUserRepository.GetDefaultApiClientToken();
            return _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken).Search<MpBumpingRuleDto>($"To_Event_Room_ID IN {queryString} AND From_Event_Room_ID = {fromEventRoomId}", _bumpingRuleColumns);
        }

        public List<MpBumpingRoomsDto> GetBumpingRoomsForEventRoom(int eventId, int fromEventRoomId)
        {
            var bumpingRoomsColumns = new List<string>
            {
                "To_Event_Room_ID",
                "To_Event_Room_ID_Table.Room_ID",
                "Priority_Order",
                "Bumping_Rule_Type_ID",
                "To_Event_Room_ID_Table.Capacity",
                "To_Event_Room_ID_Table.Allow_Checkin",
                "To_Event_Room_ID_Table_Room_ID_Table.Room_Name",
                $"[dbo].crds_getEventParticipantStatusCount({eventId}, To_Event_Room_ID_Table.Room_Id, 3) AS Signed_In",
                $"[dbo].crds_getEventParticipantStatusCount({eventId}, To_Event_Room_ID_Table.Room_Id, 4) AS Checked_In"
            };

            var apiUserToken = _apiUserRepository.GetDefaultApiClientToken();
            var priorityOrderRooms = _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken).
                    Search<MpBumpingRoomsDto>($"From_Event_Room_ID = {fromEventRoomId}", bumpingRoomsColumns).
                    OrderBy(bumpingRoom => bumpingRoom.PriorityOrder).ToList();
            if (!priorityOrderRooms.Any())
            {
                return null;
            }
            var isBumpingTypeVacancy = priorityOrderRooms.First().BumpingRuleTypeId == _applicationConfiguration.BumpingRoomTypeVacancyId;
            if (isBumpingTypeVacancy)
            {
                // order by number signed  in + number checked in ascending
                return priorityOrderRooms.OrderBy(r => r.SignedIn + r.CheckedIn).ToList();
            }
            else
            {
                return priorityOrderRooms;
            }
        }

        public List<List<JObject>> GetManageRoomsListData(int eventId)
        {
            var parms = new Dictionary<string, object>
            {
                {"EventID", eventId},
            };

            var result = _ministryPlatformRestRepository.UsingAuthenticationToken(_apiUserRepository.GetDefaultApiClientToken()).GetFromStoredProc<JObject>("api_crds_Get_Checkin_Room_Data", parms);
            return result;
        }

        public List<List<JObject>> GetSingleRoomGroupsData(int eventId, int roomId)
        {
            var parms = new Dictionary<string, object>
            {
                {"EventID", eventId},
                {"RoomID", roomId},
            };

            var result = _ministryPlatformRestRepository.UsingAuthenticationToken(_apiUserRepository.GetDefaultApiClientToken()).GetFromStoredProc<JObject>("api_crds_Get_Checkin_Single_Room_Data", parms);
            return result;
        }

        public List<List<JObject>> SaveSingleRoomGroupsData(int eventId, int roomId, string groupsXml)
        {
            // new line chars come from the string conversion and need to be stripped out here to avoid a parsing error when calling the proc
            groupsXml = groupsXml.Replace(System.Environment.NewLine, string.Empty);

            var parms = new Dictionary<string, object>
            {
                {"@EventId", eventId},
                {"@RoomId", roomId},
                {"@GroupsXml", groupsXml}
            };

            var result = _ministryPlatformRestRepository.UsingAuthenticationToken(_apiUserRepository.GetDefaultApiClientToken()).PostStoredProc<JObject>("api_crds_Update_Single_Room_Checkin_Data", parms);
            return result;
        }

        public List<MpEventRoomDto> GetEventRoomsByEventRoomIds(List<int> eventRoomsIds)
        {
            var apiUserToken = _apiUserRepository.GetDefaultApiClientToken();
            return _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken).Search<MpEventRoomDto>($"Event_Room_ID IN ({string.Join(",", eventRoomsIds)})", _eventRoomColumns);
        }
    }
}
