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

        // Room capacity changes
        public void PublishRoomCapacity(int eventId, int roomId, EventRoomDto data)
        {
            var publishEventId = GetPublishEventId(eventId);
            var channelName = $"{_applicationConfiguration.CheckinRoomChannel}{publishEventId}{roomId}";
            Publish(channelName, null, data);
        }

        // Event participant moves from signed in to checked in
        public void PublishCheckinParticipantsCheckedIn(int eventId, int roomId, ParticipantDto data)
        {
            var publishEventId = GetPublishEventId(eventId);
            var channelName = GetChannelNameCheckinParticipants(publishEventId, roomId);
            Publish(channelName, "CheckedIn", data);
        }

        // Event participant signed in
        public void PublishCheckinParticipantsAdd(int eventId, int roomId, List<ParticipantDto> data)
        {
            var publishEventId = GetPublishEventId(eventId);
            var channelName = GetChannelNameCheckinParticipants(publishEventId, roomId);
            Publish(channelName, "Add", data);
        }

        // Event participant signed out
        public void PublishCheckinParticipantsSignedInRemove(int eventId, int roomId, ParticipantDto data)
        {
            var publishEventId = GetPublishEventId(eventId);
            var channelName = GetChannelNameCheckinParticipants(publishEventId, roomId);
            Publish(channelName, "RemoveSignIn", data);
        }

        // Event participant checked out
        public void PublishCheckinParticipantsCheckedInRemove(int eventId, int roomId, ParticipantDto data)
        {
            var publishEventId = GetPublishEventId(eventId);
            var channelName = GetChannelNameCheckinParticipants(publishEventId, roomId);
            Publish(channelName, "RemoveCheckIn", data);
        }

        // Event participant as overridden
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