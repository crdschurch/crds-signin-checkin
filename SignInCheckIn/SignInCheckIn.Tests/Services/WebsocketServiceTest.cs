using System.Collections.Generic;
using Crossroads.Utilities.Services.Interfaces;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Moq;
using NUnit.Framework;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Services;
using SignInCheckIn.Services.Interfaces;
using SignInCheckIn.Hubs;

namespace SignInCheckIn.Tests.Services
{
    public class WebsocketServiceTest
    {
        private Mock<IEventService> _eventService;
        private Mock<IApplicationConfiguration> _applicationConfiguration;
        private Mock<IHubContext> _hubContext;
        
        private WebsocketService _fixture;

        private const int eventId = 123;
        private const int roomId = 456;


        [SetUp]
        public void SetUp()
        {
            AutoMapperConfig.RegisterMappings();

            _eventService = new Mock<IEventService>(MockBehavior.Strict);
            _applicationConfiguration = new Mock<IApplicationConfiguration>();
            
            _applicationConfiguration.Setup(ac => ac.CheckinParticipantsChannel).Returns("CheckinParticipantsChannel");
            _applicationConfiguration.Setup(ac => ac.CheckinRoomChannel).Returns("CheckinRoomChannel");
            _applicationConfiguration.Setup(ac => ac.AdventureClubEventTypeId).Returns(20);

            var eventDto = new EventDto()
            {
                ParentEventId = null,
                EventId = 123,
                EventTypeId = 7 // not Adventure Club
            };
            _eventService.Setup(m => m.GetEvent(It.IsAny<int>())).Returns(eventDto);
            _hubContext = new Mock<IHubContext>();

            _fixture = new WebsocketService(_eventService.Object, _applicationConfiguration.Object, _hubContext.Object);
        }

        [Test]
        public void ShouldPublishCheckinCapacity()
        {
            const string expectedChannelName = "CheckinParticipantsChannel123456";
            var data = new List<ParticipantDto>();
            var mock = new Mock<IDependance>();
            mock.Setup(m => m.OnEvent(expectedChannelName, It.IsAny<ChannelEvent>()));
            var mockIHubConnectionContext = new Mock<IHubConnectionContext<dynamic>>();
            mockIHubConnectionContext.Setup(hc => hc.Group(expectedChannelName)).Returns(mock.Object);
            _hubContext.SetupGet(hc => hc.Clients).Returns(mockIHubConnectionContext.Object);

            _fixture.PublishCheckinParticipantsAdd(eventId, roomId, data);

            mock.VerifyAll();
            mockIHubConnectionContext.VerifyAll();
        }

        [Test]
        public void ShouldPublishCheckinParticipantsCheckedIn()
        {
            const string expectedChannelName = "CheckinParticipantsChannel123456";
            var data = new ParticipantDto();
            var mock = new Mock<IDependance>();
            mock.Setup(m => m.OnEvent(expectedChannelName, It.IsAny<ChannelEvent>()));
            var mockIHubConnectionContext = new Mock<IHubConnectionContext<dynamic>>();
            mockIHubConnectionContext.Setup(hc => hc.Group(expectedChannelName)).Returns(mock.Object);
            _hubContext.SetupGet(hc => hc.Clients).Returns(mockIHubConnectionContext.Object);

            _fixture.PublishCheckinParticipantsCheckedIn(eventId, roomId, data);

            mock.VerifyAll();
            mockIHubConnectionContext.VerifyAll();
        }

        [Test]
        public void ShouldPublishCheckinParticipantsAdd()
        {
            const string expectedChannelName = "CheckinParticipantsChannel123456";
            var data = new List<ParticipantDto>();
            var mock = new Mock<IDependance>();
            mock.Setup(m => m.OnEvent(expectedChannelName, It.IsAny<ChannelEvent>()));
            var mockIHubConnectionContext = new Mock<IHubConnectionContext<dynamic>>();
            mockIHubConnectionContext.Setup(hc => hc.Group(expectedChannelName)).Returns(mock.Object);
            _hubContext.SetupGet(hc => hc.Clients).Returns(mockIHubConnectionContext.Object);

            _fixture.PublishCheckinParticipantsAdd(eventId, roomId, data);

            mock.VerifyAll();
            mockIHubConnectionContext.VerifyAll();
        }

        [Test]
        public void ShouldPublishCheckinParticipantsRemove()
        {
            const string expectedChannelName = "CheckinParticipantsChannel123456";
            var data = new ParticipantDto();
            var mock = new Mock<IDependance>();
            mock.Setup(m => m.OnEvent(expectedChannelName, It.IsAny<ChannelEvent>()));
            var mockIHubConnectionContext = new Mock<IHubConnectionContext<dynamic>>();
            mockIHubConnectionContext.Setup(hc => hc.Group(expectedChannelName)).Returns(mock.Object);
            _hubContext.SetupGet(hc => hc.Clients).Returns(mockIHubConnectionContext.Object);

            _fixture.PublishCheckinParticipantsRemove(eventId, roomId, data);

            mock.VerifyAll();
            mockIHubConnectionContext.VerifyAll();
        }

        [Test]
        public void ShouldPublishCheckinParticipantsOverrideCheckin()
        {
            const string expectedChannelName = "CheckinParticipantsChannel123456";
            var data = new ParticipantDto();
            var mock = new Mock<IDependance>();
            mock.Setup(m => m.OnEvent(expectedChannelName, It.IsAny<ChannelEvent>()));
            var mockIHubConnectionContext = new Mock<IHubConnectionContext<dynamic>>();
            mockIHubConnectionContext.Setup(hc => hc.Group(expectedChannelName)).Returns(mock.Object);
            _hubContext.SetupGet(hc => hc.Clients).Returns(mockIHubConnectionContext.Object);

            _fixture.PublishCheckinParticipantsOverrideCheckin(eventId, roomId, data);

            mock.VerifyAll();
            mockIHubConnectionContext.VerifyAll();
        }

        [Test]
        public void ShouldPublishByParentEventId()
        {
            _eventService = new Mock<IEventService>(MockBehavior.Strict);
            _applicationConfiguration = new Mock<IApplicationConfiguration>();

            _applicationConfiguration.Setup(ac => ac.CheckinParticipantsChannel).Returns("CheckinParticipantsChannel");
            _applicationConfiguration.Setup(ac => ac.CheckinRoomChannel).Returns("CheckinRoomChannel");
            _applicationConfiguration.Setup(ac => ac.AdventureClubEventTypeId).Returns(20);

            var eventDto = new EventDto()
            {
                ParentEventId = 678,
                EventId = 123,
                EventTypeId = 20 // Adventure Club
            };
            _eventService.Setup(m => m.GetEvent(It.IsAny<int>())).Returns(eventDto);
            _hubContext = new Mock<IHubContext>();

            _fixture = new WebsocketService(_eventService.Object, _applicationConfiguration.Object, _hubContext.Object);

            // should publish via the parent channel since it is AC event
            const string expectedChannelName = "CheckinParticipantsChannel678456";
            var data = new ParticipantDto();
            var mock = new Mock<IDependance>();
            mock.Setup(m => m.OnEvent(expectedChannelName, It.IsAny<ChannelEvent>()));
            var mockIHubConnectionContext = new Mock<IHubConnectionContext<dynamic>>();
            mockIHubConnectionContext.Setup(hc => hc.Group(expectedChannelName)).Returns(mock.Object);
            _hubContext.SetupGet(hc => hc.Clients).Returns(mockIHubConnectionContext.Object);

            _fixture.PublishCheckinParticipantsOverrideCheckin(eventId, roomId, data);

            mock.VerifyAll();
            mockIHubConnectionContext.VerifyAll();
        }

    }

    public interface IDependance
    {
        string OnEvent(string channelName, ChannelEvent channelEvent);
    }
}
