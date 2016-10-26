﻿using System;
using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;

namespace MinistryPlatform.Translation.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly IApiUserRepository _apiUserRepository;
        private readonly IMinistryPlatformRestRepository _ministryPlatformRestRepository;
        private readonly List<string> _eventGroupsColumns;

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
                "Event_Room_ID_Table_Room_ID_Table.[Room_ID]"
            };
        }

        public List<MpEventDto> GetEvents(DateTime startDate, DateTime endDate, int site)
        {
            var apiUserToken = _apiUserRepository.GetToken();

            var columnList = new List<string>
            {
                "Event_ID",
                "Event_Title",
                "Event_Start_Date",
                "Event_Type_ID_Table.Event_Type",
                "Congregation_ID_Table.Congregation_Name"
            };

            var startTimeString = startDate.ToShortDateString();
            var endTimeString = endDate.ToShortDateString();

            return _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken)
                .Search<MpEventDto>($"[Allow_Check-in]=1 AND [Cancelled]=0 AND [Event_Start_Date] >= '{startTimeString}' AND [Event_Start_Date] <= '{endTimeString}' AND Events.[Congregation_ID] = {site}", columnList);
        }


        public MpEventDto GetEventById(int eventId)
        {
            var apiUserToken = _apiUserRepository.GetToken();

            var columnList = "Event_ID, Event_Title,Event_Start_Date,Event_Type_ID_Table.Event_Type," +
                             "Congregation_ID_Table.Congregation_Name, Events.Congregation_ID,Congregation_ID_Table.Location_ID";

            return _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken)
                .Get<MpEventDto>(eventId, columnList);
        }

        public List<MpEventGroupDto> GetEventGroupsForEvent(int eventId)
        {
            return _ministryPlatformRestRepository.UsingAuthenticationToken(_apiUserRepository.GetToken())
                .Search<MpEventGroupDto>($"Event_Groups.Event_ID = {eventId}", _eventGroupsColumns);
        }

        public void DeleteEventGroups(string authenticationToken, IEnumerable<int> eventGroupIds)
        {
            var token = authenticationToken ?? _apiUserRepository.GetToken();
            _ministryPlatformRestRepository.UsingAuthenticationToken(token).Delete<MpEventGroupDto>(eventGroupIds);
        }
    }
}
