using System;
using Crossroads.Utilities.Services.Interfaces;
using Microsoft.AspNet.SignalR;
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

        private void Publish(IHubContext context, int eventId, string channelName, string name, object data)
        {
            var channelEvent = new ChannelEvent
            {
                ChannelName = channelName,
                Name = "OverrideCheckin",
                Data = data
            };
            context.Clients.Group(channelEvent.ChannelName).OnEvent(channelEvent.ChannelName, channelEvent);
        }

        public void PublishCheckinParticipantsCheckedIn(IHubContext context, int eventId, int roomId, object data)
        {
            // if AC event, we need to publish to parent event channel
            var publishEventId = eventId;
            var e = _eventService.GetEvent(eventId);
            if (e.EventTypeId == _applicationConfiguration.AdventureClubEventTypeId)
            {
                publishEventId = e.ParentEventId.Value;
            }

            var channelName = GetChannelNameCheckinParticipants(publishEventId, roomId);
            Publish(context, eventId, channelName, "CheckedIn", data);
        }

        private string GetChannelNameCheckinParticipants(int eventId, int roomId)
        {
            return $"{_applicationConfiguration.CheckinParticipantsChannel}{eventId}{roomId}";
        }
    }
}