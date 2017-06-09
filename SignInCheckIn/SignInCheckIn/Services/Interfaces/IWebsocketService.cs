using System;
using System.Collections.Generic;
using SignInCheckIn.Models.DTO;
using Microsoft.AspNet.SignalR;

namespace SignInCheckIn.Services.Interfaces
{
    public interface IWebsocketService
    {
        void PublishRoomCapacity(int eventId, int roomId, EventRoomDto data);
        void PublishCheckinParticipantsCheckedIn(int eventId, int roomId, ParticipantDto data);
        void PublishCheckinParticipantsAdd(int eventId, int roomId, List<ParticipantDto> data);
        void PublishCheckinParticipantsCheckedInRemove(int eventId, int roomId, ParticipantDto data);
        void PublishCheckinParticipantsSignedInRemove(int eventId, int roomId, ParticipantDto data);
        void PublishCheckinParticipantsOverrideCheckin(int eventId, int roomId, ParticipantDto data);
    }
}
