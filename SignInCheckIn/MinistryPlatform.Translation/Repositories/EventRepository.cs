using System;
using System.Collections.Generic;
using System.Linq;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;

namespace MinistryPlatform.Translation.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly IApiUserRepository _apiUserRepository;
        private readonly IMinistryPlatformRestRepository _ministryPlatformRestRepository;
        private readonly List<string> _eventGroupsColumns;
        private readonly List<string> _eventColumns;

        private const string ResetEventStoredProcedureName = "api_crds_ResetEcheckEvent";
        private const string ImportEventStoredProcedureName = "api_crds_ImportEcheckEvent";

        public EventRepository(IApiUserRepository apiUserRepository,
            IMinistryPlatformRestRepository ministryPlatformRestRepository)
        {
            _apiUserRepository = apiUserRepository;
            _ministryPlatformRestRepository = ministryPlatformRestRepository;

            _eventGroupsColumns = new List<string>
            {
                "Event_Groups.[Event_Group_ID]",
                "Event_ID_Table.[Event_ID]",
                "Group_ID_Table.[Group_ID]",
                "Event_Room_ID_Table.[Event_Room_ID]",
                "Event_Room_ID_Table_Room_ID_Table.[Room_ID]",
                "Event_Room_ID_Table.[Capacity]",
                "Event_Room_ID_Table.[Label]",
                "Event_Room_ID_Table.[Allow_Checkin]",
                "Event_Room_ID_Table.[Volunteers]",
                "[dbo].crds_getEventParticipantStatusCount(Event_ID_Table.[Event_ID], Event_Room_ID_Table_Room_ID_Table.[Room_ID], 3) AS Signed_In",
                "[dbo].crds_getEventParticipantStatusCount(Event_ID_Table.[Event_ID], Event_Room_ID_Table_Room_ID_Table.[Room_ID], 4) AS Checked_In"
            };

            _eventColumns = new List<string>
            {
                "Event_ID",
                "Parent_Event_ID",
                "Event_Title",
                "Program_ID",
                "Primary_Contact",
                "Event_Start_Date",
                "Event_End_Date",
                "[Early_Check-in_Period]",
                "[Late_Check-in_Period]",
                "Event_Type_ID_Table.Event_Type",
                "Events.Event_Type_ID",
                "Congregation_ID_Table.Congregation_Name",
                "Events.Congregation_ID",
                "Congregation_ID_Table.Location_ID",
                "[Allow_Check-in]"
            };
        }

        /// <summary>
        /// The end date parameter is automatically cast to the end of the day for that date
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="site"></param>
        /// <param name="includeSubevents"></param>
        /// <returns></returns>
        public List<MpEventDto> GetEvents(DateTime startDate, DateTime endDate, int site, bool? includeSubevents = false)
        {
            var apiUserToken = _apiUserRepository.GetToken();

            var startTimeString = startDate.ToString();
            // make sure end time is end of day
            var endTimeString = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59).ToString();

            var queryString =
                $"[Allow_Check-in]=1 AND [Cancelled]=0 AND [Event_Start_Date] >= '{startTimeString}' AND [Event_Start_Date] <= '{endTimeString}' AND Events.[Congregation_ID] = {site}";
            if (includeSubevents != true)
            {
                // do not include subevents
                queryString = $"{queryString} AND [Parent_Event_ID] IS NULL";
            }
            return _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken)
                .Search<MpEventDto>(queryString, _eventColumns);
        }

        public MpEventDto GetEventById(int eventId)
        {
            var apiUserToken = _apiUserRepository.GetToken();

            return _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken)
                .Get<MpEventDto>(eventId, _eventColumns);
        }

        public MpEventDto CreateSubEvent(string token, MpEventDto mpEventDto)
        {
            return _ministryPlatformRestRepository.UsingAuthenticationToken(token).Create(mpEventDto, _eventColumns);
        }

        public MpEventDto UpdateEvent(string token, MpEventDto mpEventDto)
        {
            return _ministryPlatformRestRepository.UsingAuthenticationToken(token).Update(mpEventDto, _eventColumns);
        }

        public List<MpEventGroupDto> GetEventGroupsForEvent(int eventId)
        {
            var eventIds = new List<int> {eventId};
            return GetEventGroupsForEvent(eventIds);
        }

        public List<MpEventGroupDto> GetEventGroupsForEvent(List<int> eventIds)
        {
            return _ministryPlatformRestRepository.UsingAuthenticationToken(_apiUserRepository.GetToken())
                .Search<MpEventGroupDto>($"Event_Groups.Event_ID IN ({string.Join(",", eventIds)})", _eventGroupsColumns);
        }

        public List<MpEventGroupDto> GetEventGroupsForEventRoom(int eventId, int roomId)
        {
            return
                _ministryPlatformRestRepository.UsingAuthenticationToken(_apiUserRepository.GetToken())
                    .Search<MpEventGroupDto>($"Event_Groups.Event_ID = {eventId} AND Event_Room_ID_Table_Room_ID_Table.Room_ID = {roomId}", _eventGroupsColumns);
        }

        public void DeleteEventGroups(string authenticationToken, IEnumerable<int> eventGroupIds)
        {
            var token = authenticationToken ?? _apiUserRepository.GetToken();
            _ministryPlatformRestRepository.UsingAuthenticationToken(token).Delete<MpEventGroupDto>(eventGroupIds);
        }

        public List<MpEventGroupDto> CreateEventGroups(string authenticationToken, List<MpEventGroupDto> eventGroups)
        {
            var token = authenticationToken ?? _apiUserRepository.GetToken();
            return _ministryPlatformRestRepository.UsingAuthenticationToken(token).Create(eventGroups, _eventGroupsColumns);
        }

        public void ResetEventSetup(string authenticationToken, int eventId)
        {
            _ministryPlatformRestRepository.UsingAuthenticationToken(authenticationToken)
                .PostStoredProc(ResetEventStoredProcedureName, new Dictionary<string, object> {{"@EventId", eventId}});
        }

        public void ImportEventSetup(string authenticationToken, int destinationEventId, int sourceEventId)
        {
            _ministryPlatformRestRepository.UsingAuthenticationToken(authenticationToken)
                .PostStoredProc(ImportEventStoredProcedureName, new Dictionary<string, object> {{"@DestinationEventId", destinationEventId}, {"@SourceEventId", sourceEventId}});
        }

        public List<MpEventDto> GetEventAndCheckinSubevents(string authenticationToken, int eventId)
        {
            var token = authenticationToken ?? _apiUserRepository.GetToken();

            return _ministryPlatformRestRepository.UsingAuthenticationToken(token)
                .Search<MpEventDto>($"(Events.Event_ID = {eventId} OR Events.Parent_Event_ID = {eventId}) AND Events.[Allow_Check-in] = 1", _eventColumns);
        }

	    public List<MpEventDto> GetSubeventsForEvents(List<int> eventIds, int? eventTypeId)
        {
            var apiUserToken = _apiUserRepository.GetToken();

            var queryString = eventIds.Aggregate("(", (current, id) => current + (id + ","));

            queryString = queryString.TrimEnd(',');
            queryString += ")";

            // search on the event type if it's not a null param
            var typeQueryString = (eventTypeId != null) ? " AND Events.[Event_Type_ID] = " + eventTypeId : "";

            return _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken)
                .Search<MpEventDto>($"Events.[Parent_Event_ID] IN {queryString} AND Events.[Allow_Check-in] = 1 {typeQueryString}", _eventColumns);
        }
    }
}
