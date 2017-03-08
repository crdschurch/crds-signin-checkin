using System;
using System.Collections.Generic;
using SignInCheckIn.Models.DTO;
using Microsoft.AspNet.SignalR;

namespace SignInCheckIn.Services.Interfaces
{
    public interface IWebsocketService
    {
        void PublishCheckinCapacity(int eventId, int roomId, EventRoomDto data);
        void PublishCheckinParticipantsCheckedIn(int eventId, int roomId, ParticipantDto data);
        void PublishCheckinParticipantsAdd(int eventId, int roomId, IEnumerable<ParticipantDto> data);
        void PublishCheckinParticipantsRemove(int eventId, int roomId, ParticipantDto data);
        void PublishCheckinParticipantsOverrideCheckin(int eventId, int roomId, ParticipantDto data);
    }
}
