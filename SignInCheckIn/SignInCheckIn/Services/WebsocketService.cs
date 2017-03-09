using System;
using System.Collections.Generic;
using Crossroads.Utilities.Services.Interfaces;
using Microsoft.AspNet.SignalR;
using MinistryPlatform.Translation.Models.DTO;
using SignInCheckIn.Services.Interfaces;
using SignInCheckIn.Hubs;
using SignInCheckIn.Models.DTO;

namespace SignInCheckIn.Services
{
    public class WebsocketService : IWebsocketService
    {
        private readonly IEventService _eventService;
        private readonly IApplicationConfiguration _applicationConfiguration;
        private readonly IHubContext _context;

        public WebsocketService(IEventService eventService, IApplicationConfiguration applicationConfiguration, IHubContext hubContext)
        {
            _eventService = eventService;
            _applicationConfiguration = applicationConfiguration;
            _context = hubContext;
        }

        public void PublishCheckinCapacity(int eventId, int roomId, EventRoomDto data)
        {
            var publishEventId = GetPublishEventId(eventId);
            var channelName = $"{_applicationConfiguration.CheckinCapacityChannel}{publishEventId}{roomId}";
            Publish(channelName, null, data);
        }

        public void PublishCheckinParticipantsCheckedIn(int eventId, int roomId, ParticipantDto data)
        {
            var publishEventId = GetPublishEventId(eventId);
            var channelName = GetChannelNameCheckinParticipants(publishEventId, roomId);
            Publish(channelName, "CheckedIn", data);
        }

        public void PublishCheckinParticipantsAdd(int eventId, int roomId, IEnumerable<ParticipantDto> data)
        {
            var publishEventId = GetPublishEventId(eventId);
            var channelName = GetChannelNameCheckinParticipants(publishEventId, roomId);
            Publish(channelName, "Add", data);
        }

        public void PublishCheckinParticipantsRemove(int eventId, int roomId, ParticipantDto data)
        {
            var publishEventId = GetPublishEventId(eventId);
            var channelName = GetChannelNameCheckinParticipants(publishEventId, roomId);
            Publish(channelName, "Remove", data);
        }

        public void PublishCheckinParticipantsOverrideCheckin(int eventId, int roomId, ParticipantDto data)
        {
            var publishEventId = GetPublishEventId(eventId);
            var channelName = GetChannelNameCheckinParticipants(publishEventId, roomId);
            Publish(channelName, "OverrideCheckin", data);
        }


        private void Publish(string channelName, string action, object data)
        {
            var channelEvent = new ChannelEvent
            {
                ChannelName = channelName,
                Data = data
            };
            if (!string.IsNullOrEmpty(action))
            {
                channelEvent.Name = action;
            }

            var group = _context.Clients.Group(channelEvent.ChannelName);
            if (group != null)
            {
                group.OnEvent(channelEvent.ChannelName, channelEvent);
            }
        }

        /*
        ** Get the parent event id as websockets should always publish to the parent event id
        ** rather than the AC event. This basically checked to see if it is an AC event, if so
        ** then return it's parent event, if not return what was passed in
        */
        private int GetPublishEventId(int eventId)
        {
            var e = _eventService.GetEvent(eventId);
            if (e.EventTypeId == _applicationConfiguration.AdventureClubEventTypeId)
            {
                return e.ParentEventId.Value;
            }
            else
            {
                return eventId;
            }
        }

        private string GetChannelNameCheckinParticipants(int eventId, int roomId)
        {
            return $"{_applicationConfiguration.CheckinParticipantsChannel}{eventId}{roomId}";
        }
    }
}