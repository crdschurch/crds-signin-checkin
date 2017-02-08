using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace SignInCheckIn.Hubs
{
    public class ManageRoomsHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }
    }
}