using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            string dateOffsetSearchString = "";

            // today + 6 days into the future
            for (int i = 0; i < 7; i++)
            {
                var baseDateTime = DateTime.Now.AddDays(i);
                var dateYear = baseDateTime.Year;
                var dateMonth = baseDateTime.Month;
                var dateDay = baseDateTime.Day;

                var searchToken = dateYear + "-" + dateMonth + "-" + dateDay;

                dateOffsetSearchString += "Event_Start_Date='" + searchToken + "' OR ";
            }

            // remove the trailing "OR " to avoid syntax error
            int place = dateOffsetSearchString.LastIndexOf("OR ");
            dateOffsetSearchString = dateOffsetSearchString.Remove(place, "OR ".Length).Insert(place, "");

            // 99 is for development - "Oakley Service"
            return _ministryPlatformRestRepository.UsingAuthenticationToken(apiUserToken)
                .Search<MpEventDto>("Events.Event_Type_ID=99 AND [Allow_Check-in]=1 AND (" + dateOffsetSearchString + ")", columnList);
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
