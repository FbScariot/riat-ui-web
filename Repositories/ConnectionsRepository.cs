
using System.Collections.Generic;
using System.Linq;
using Chat.Models;

namespace Chat.Repositories
{
    public class ConnectionsRepository
    {
        private readonly Dictionary<string, User> connections = new Dictionary<string, User>();

        public void Add(string uniqueID, User user)
        {
            if (this.connections.Count > 0)
            {
                User userAntigo = GetAllUser().Where(c => c.signalRConnectionId == user.signalRConnectionId).FirstOrDefault();

                if (userAntigo != null)
                {
                    string connectionsIdAntigo = this.GetConnectionId(userAntigo.signalRConnectionId);
                    connections.Remove(connectionsIdAntigo);
                }
            }

            if (!connections.ContainsKey(uniqueID))
            {
                connections.Add(uniqueID, user);
            }
        }

        public void Remove(string uniqueID)
        {
            connections.Remove(uniqueID);
        }

        public string GetConnectionId(string id)
        {
            return (from con in connections
                    where con.Value.signalRConnectionId == id
                    select con.Key).FirstOrDefault();
        }

        //private string GetBySignalRConnectionId(string signalRConnectionId)
        //{
        //    return (from con in connections
        //            where con.Value.SignalRConnectionId == signalRConnectionId
        //            select con.Key).FirstOrDefault();
        //}

        public List<User> GetAllUser()
        {
            return (from con in connections
                    select con.Value
            ).ToList();
        }
    }
}