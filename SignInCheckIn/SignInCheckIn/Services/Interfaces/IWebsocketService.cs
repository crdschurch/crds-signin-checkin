using System;
using System.Collections.Generic;
using SignInCheckIn.Models.DTO;
using Microsoft.AspNet.SignalR;

namespace SignInCheckIn.Services.Interfaces
{
    public interface IWebsocketService
    {
        void PublishCheckinParticipantsCheckedIn(IHubContext context, int eventId, int roomId, object data);
    }
}
