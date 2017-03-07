using System;
using Crossroads.Utilities.Services.Interfaces;
using Microsoft.AspNet.SignalR;
using MinistryPlatform.Translation.Models.DTO;
using SignInCheckIn.Services.Interfaces;
using SignInCheckIn.Hubs;

namespace SignInCheckIn.Services
{
    public class WebsocketService : IWebsocketService
    {
        private readonly IEventService _eventService;
        private readonly IApplicationConfiguration _applicationConfiguration;

        public WebsocketService(IEventService eventService, IApplicationConfiguration applicationConfiguration)
        {
            _eventService = eventService;
            _applicationConfiguration = applicationConfiguration;
        }

        public void PublishCheckinCapacity(IHubContext context, int eventId, int roomId, object data)
        {
            var publishEventId = GetPublishEventId(eventId);
            var channelName = $"{_applicationConfiguration.CheckinCapacityChannel}{publishEventId}{roomId}";
            Publish(context, channelName, null, data);
        }

        public void PublishCheckinParticipantsCheckedIn(IHubContext context, int eventId, int roomId, object data)
        {
            var publishEventId = GetPublishEventId(eventId);
            var channelName = GetChannelNameCheckinParticipants(publishEventId, roomId);
            Publish(context,channelName, "CheckedIn", data);
        }

        public void PublishCheckinParticipantsAdd(IHubContext context, int eventId, int roomId, object data)
        {
            var publishEventId = GetPublishEventId(eventId);
            var channelName = GetChannelNameCheckinParticipants(publishEventId, roomId);
            Publish(context, channelName, "Add", data);
        }

        public void PublishCheckinParticipantsRemove(IHubContext context, int eventId, int roomId, object data)
        {
            var publishEventId = GetPublishEventId(eventId);
            var channelName = GetChannelNameCheckinParticipants(publishEventId, roomId);
            Publish(context, channelName, "Remove", data);
        }

        public void PublishCheckinParticipantsOverrideCheckin(IHubContext context, int eventId, int roomId, object data)
        {
            var publishEventId = GetPublishEventId(eventId);
            var channelName = GetChannelNameCheckinParticipants(publishEventId, roomId);
            Publish(context, channelName, "OverrideCheckin", data);
        }


        private void Publish(IHubContext context, string channelName, string action, object data)
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
            context.Clients.Group(channelEvent.ChannelName).OnEvent(channelEvent.ChannelName, channelEvent);
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