using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;

namespace API.SignalR
{
    public class PresenceTracker
    {
        private static readonly Dictionary<string, List<string>> OnlineUsers =
            new Dictionary<string, List<string>>();

        public Task<bool> UserConnected(string username, string connectedId)
        {
            bool isOnline = false;
            // Use a lock to ensure thread safety when modifying the OnlineUsers dictionary.
            lock (OnlineUsers)

            {
                // Check if the username is already present in the OnlineUsers dictionary.
                if (OnlineUsers.ContainsKey(username))
                {
                    // If the username exists, add the connectedId to the list of connections for that username.
                    OnlineUsers[username].Add(connectedId);
                }
                else
                {
                    // If the username doesn't exist, create a new entry with the username and a list containing the connectedId.
                    OnlineUsers.Add(username, new List<string> { connectedId });
                    isOnline = true;
                }
            }
            return Task.FromResult(isOnline);
        }


        public Task<bool> UserDisconnected(string username, string connectedId)
        {
            bool isOffline = false;
            lock (OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(username)) return Task.FromResult(isOffline);
                OnlineUsers[username].Remove(connectedId);

                if (OnlineUsers[username].Count == 0)
                {
                    OnlineUsers.Remove(username);
                    isOffline = true;
                }
            }
            return Task.FromResult(isOffline);
        }

        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;
            lock (OnlineUsers)
            {
                // Order the usernames alphabetically and select them into an array.
                onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
            }
            return Task.FromResult(onlineUsers);
        }

        public static Task<List<string>> GetConnectionsForUser(string username)
        {
            List<string> connectionIds;
            lock (OnlineUsers)
            {
                // Retrieve the list of connection IDs for the specified username.
                connectionIds = OnlineUsers.GetValueOrDefault(username);
            }
            // Wrap the connectionIds list in a completed Task<List<string>>.
            return Task.FromResult(connectionIds);
        }





    }
}