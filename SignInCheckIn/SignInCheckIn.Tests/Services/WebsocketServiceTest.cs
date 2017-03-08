using Crossroads.Utilities.Services.Interfaces;
using Moq;
using NUnit.Framework;
using SignInCheckIn.Models.DTO;
using SignInCheckIn.Services;
using SignInCheckIn.Services.Interfaces;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SignInCheckIn.Hubs;

namespace SignInCheckIn.Tests.Services
{
    public class WebsocketServiceTest
    {
        private Mock<IEventService> _eventService;
        private Mock<IApplicationConfiguration> _applicationConfiguration;
        private Mock<IHubContext> _hubContext;
        
        private WebsocketService _fixture;


        [SetUp]
        public void SetUp()
        {
            AutoMapperConfig.RegisterMappings();

            _eventService = new Mock<IEventService>(MockBehavior.Strict);
            _applicationConfiguration = new Mock<IApplicationConfiguration>();
            
            _applicationConfiguration.Setup(ac => ac.CheckinParticipantsChannel).Returns("CheckinParticipantsChannel");
            _applicationConfiguration.Setup(ac => ac.CheckinCapacityChannel).Returns("CheckinCapacityChannel");
            _applicationConfiguration.Setup(ac => ac.AdventureClubEventTypeId).Returns(20);

            var eventDto = new EventDto()
            {
                ParentEventId = 678,
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
            var eventId = 123;
            var roomId = 456;
            const string expectedChannelName = "CheckinCapacityChannel123456";

            var eventRoom = new EventRoomDto()
            {
                Capacity = 4,
                EventRoomId = 333
            };

            var mock = new Mock<IDependance>();
            mock.Setup(m => m.OnEvent(expectedChannelName, It.IsAny<ChannelEvent>()));
            var mockIHubConnectionContext = new Mock<IHubConnectionContext<dynamic>>();
            mockIHubConnectionContext.Setup(hc => hc.Group(expectedChannelName)).Returns(mock.Object);
            _hubContext.SetupGet(hc => hc.Clients).Returns(mockIHubConnectionContext.Object);

            _fixture.PublishCheckinCapacity(eventId, roomId, eventRoom);

        }

        // PublishCheckinParticipantsCheckedIn
        // PublishCheckinParticipantsAdd
        // PublishCheckinParticipantsRemove
        // PublishCheckinParticipantsOverrideCheckin
        // PublishesParentEventId

    }

    public interface IDependance
    {
        string OnEvent(string channelName, ChannelEvent channelEvent);
    }
}
