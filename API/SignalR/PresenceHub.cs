

using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class PresenceHub : Hub

    {
        private readonly PresenceTracker _tracker;

        public PresenceHub(PresenceTracker tracker)

        {
            _tracker = tracker;
        }
        public override async Task OnConnectedAsync()

        {
            // using PresenceTracker to make sure a user has connected 
            var isOnline = await _tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);
            //if connected, Notify other connected clients that the user is now online.
            if (isOnline)
                await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());
            // Get the current list of online users from PresenceTracker.
            var currentUsers = await _tracker.GetOnlineUsers();
            // Broadcast the updated list of online users to clients that invoked the method.
            await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var isOffline = await _tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);
            if (isOffline)
                await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());

            await base.OnDisconnectedAsync(exception);
        }






    }
}