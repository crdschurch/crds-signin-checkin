using MinistryPlatform.Translation.Models.DTO;
using SignInCheckIn.Models.DTO;
using System;
using System.Collections.Generic;

namespace SignInCheckIn.Services.Interfaces
{
    public interface IEventService
    {
        List<EventDto> GetCheckinEvents(DateTime startDate, DateTime endDate, int site, string kioskId);
        List<EventDto> GetCheckinEventTemplates(int site);
        EventDto GetEvent(int eventId);
        EventDto GetCurrentEventForSite(int siteId, string kioskId);
        //List<EventRoomDto> GetCurrentEventsForSite(int siteId); 
        bool CheckEventTimeValidity(EventDto mpEventDto);
        List<EventRoomDto> ImportEventSetup(int destinationEventId, int sourceEventId);
        List<EventRoomDto> ResetEventSetup(int eventId);
        List<EventDto> GetEventMaps(int eventId);
        List<ParticipantDto> GetListOfChildrenForEvent(int eventId, string search);
        void UpdateAdventureClubStatusIfNecessary(MpEventDto eventDto);
        List<ContactDto> GetFamiliesForSearch(string search);
        HouseholdDto GetHouseholdByHouseholdId(int householdId);
        HouseholdDto UpdateHouseholdInformation(HouseholdDto householdDto);
        List<CapacityDto> GetCapacityBySite(int siteId);
        EventDto GetCurrentEventForSiteKcOnly(int siteId);
    }
}
