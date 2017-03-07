using System;
using System.Collections.Generic;
using SignInCheckIn.Models.DTO;
using Microsoft.AspNet.SignalR;

namespace SignInCheckIn.Services.Interfaces
{
    public interface IWebsocketService
    {
        void PublishCheckinCapacity(IHubContext context, int eventId, int roomId, object data);
        void PublishCheckinParticipantsCheckedIn(IHubContext context, int eventId, int roomId, object data);
        void PublishCheckinParticipantsAdd(IHubContext context, int eventId, int roomId, object data);
        void PublishCheckinParticipantsRemove(IHubContext context, int eventId, int roomId, object data);
        void PublishCheckinParticipantsOverrideCheckin(IHubContext context, int eventId, int roomId, object data);
    }
}
