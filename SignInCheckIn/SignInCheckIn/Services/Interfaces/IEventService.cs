using System;
using System.Collections.Generic;
using SignInCheckIn.Models.DTO;
using MinistryPlatform.Translation.Models.DTO;

namespace SignInCheckIn.Services.Interfaces
{
    public interface IEventService
    {
        List<EventDto> GetCheckinEvents(DateTime startDate, DateTime endDate, int site);

        EventDto GetEvent(int eventId);
        EventDto GetCurrentEventForSite(int siteId, string kioskId);
        //List<EventRoomDto> GetCurrentEventsForSite(int siteId); 
        bool CheckEventTimeValidity(EventDto mpEventDto);
        List<EventRoomDto> ImportEventSetup(string authenticationToken, int destinationEventId, int sourceEventId);
        List<EventRoomDto> ResetEventSetup(string authenticationToken, int eventId);
        List<EventDto> GetEventMaps(string token, int eventId);
        List<ParticipantDto> GetListOfChildrenForEvent(string token, int eventId, string search);
        void UpdateAdventureClubStatusIfNecessary(MpEventDto eventDto, string token);
        List<ContactDto> GetFamiliesForSearch(string token, string search);
        HouseholdDto GetHouseholdByHouseholdId(string token, int householdId);
        void UpdateHouseholdInformation(string token, HouseholdDto householdDto);
    }
}
