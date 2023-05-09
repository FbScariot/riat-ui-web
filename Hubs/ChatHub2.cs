using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using RIAT.DAL.Entity.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RIAT.UI.Web.Hubs
{
    public class ChatHub2 : Hub
    {
        #region List of Data Member 

        static List<UserDetail> ConnectedUsers = new List<UserDetail>();
        static List<MessageDetail> CurrentMessage = new List<MessageDetail>();

        #endregion


        #region Send, recieve and broadcast message methods

        public void Connect(string userName)
        {
            // To get the clients connected connection ID's
            var id = Context.ConnectionId;
            if (ConnectedUsers.Count(x => x.ConnectionId == id) == 0)
            {
                ConnectedUsers.Add(new UserDetail { ConnectionId = id, UserName = userName });

                // send to caller
                //Clients.Caller.onConnected(id, userName, ConnectedUsers, CurrentMessage);

                // send to all except caller client
                //Clients.AllExcept(id).onNewUserConnected(id, userName);
            }
        }
        // To send message to all the connected clients
        public void SendMessageToAll(string userName, string message)
        {
            // store last 100 messages in cache
            AddMessageinCache(userName, message);

            // Broad cast message
            Clients.All.SendAsync("messageReceived", userName, message);
        }
        public void SendPrivateMessage(string toUserId, string message)
        {

            string fromUserId = Context.ConnectionId;

            var toUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == toUserId);
            var fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == fromUserId);

            if (toUser != null && fromUser != null)
            {
                // send to 
                Clients.Client(toUserId).SendAsync("sendPrivateMessage", fromUserId, fromUser.UserName, message);

                // send to caller user
                Clients.Caller.SendAsync("sendPrivateMessage", toUserId, fromUser.UserName, message);
            }

        }

        public override System.Threading.Tasks.Task OnDisconnectedAsync(Exception exception)
        {
            var item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            if (item != null)
            {
                ConnectedUsers.Remove(item);

                var id = Context.ConnectionId;
                //Clients.All.onUserDisconnected(id, item.UserName);
            }

            return base.OnDisconnectedAsync(exception);
        }

        #endregion


        #region private Messages send to individual
        private void AddMessageinCache(string userName, string message)
        {
            CurrentMessage.Add(new MessageDetail { UserName = userName, Message = message });

            if (CurrentMessage.Count > 100)
                CurrentMessage.RemoveAt(0);
        }
        #endregion
    }

    public class MessageDetail
    {
        public string UserName { get; set; }
        public string Message { get; set; }
    }
    public class UserDetail
    {
        public string ConnectionId { get; set; }
        public string UserName { get; set; }
    }
}
