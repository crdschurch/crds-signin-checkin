using System;
using System.Collections.Generic;
using MinistryPlatform.Translation.Models.DTO;
using MinistryPlatform.Translation.Repositories.Interfaces;

namespace MinistryPlatform.Translation.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly IApiUserRepository _apiUserRepository;
        private readonly IMinistryPlatformRestRepository _ministryPlatformRestRepository;
        
        public EventRepository(IApiUserRepository apiUserRepository,
            IMinistryPlatformRestRepository ministryPlatformRestRepository)
        {
            _apiUserRepository = apiUserRepository;
            _ministryPlatformRestRepository = ministryPlatformRestRepository;
        }

        public List<MpEventDto> GetEvents()
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

            var currentTimeString = DateTime.Now.ToShortDateString();
            var offsetTimeString = DateTime.Now.AddDays(6).ToShortDateString();

            // 99 is for development - "Oakley Service"
            return _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken)
                .Search<MpEventDto>($"[Allow_Check-in]=1 AND [Cancelled]=0 AND [Event_Start_Date] >= '{currentTimeString}' AND [Event_Start_Date] <= '{offsetTimeString}'", columnList);
        }


        public MpEventDto GetEventById(int eventId)
        {
            var apiUserToken = _apiUserRepository.GetToken();

            var columnList = "Event_ID, Event_Title,Event_Start_Date,Event_Type_ID_Table.Event_Type," +
                             "Congregation_ID_Table.Congregation_Name, Events.Congregation_ID,Congregation_ID_Table.Location_ID";

            return _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken)
                .Get<MpEventDto>(eventId, columnList);
        }
    }
}
