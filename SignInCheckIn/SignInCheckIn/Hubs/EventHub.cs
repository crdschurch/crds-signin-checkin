using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace SignInCheckIn.Hubs
{
    public class EventHub : Hub
    {
        public async Task Subscribe(string channel)
        {
            await Groups.Add(Context.ConnectionId, channel);
        }

        public async Task Unsubscribe(string channel)
        {
            await Groups.Remove(Context.ConnectionId, channel);
        }
    }
}