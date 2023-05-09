using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RIAT.DAL.Entity.Data;
using RIAT.DAL.Entity.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RIAT.UI.Web.Hubs
{
    public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
    {
        static List<UserDetail> ConnectedUsers = new List<UserDetail>();
        static List<MessageDetail> CurrentMessage = new List<MessageDetail>();

        public static ConcurrentDictionary<string, string> usuarios = new ConcurrentDictionary<string, string>();
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<ChatHub> _hubContext;
        private ApplicationUser _appUser;
        private readonly RIATContext _context;

        public ChatHub(UserManager<ApplicationUser> userManager, IHubContext<ChatHub> hubContext, RIATContext context)
        {
            _userManager = userManager;
            _hubContext = hubContext;
            _context = context;
        }

        public override Task OnConnectedAsync()
        {
            _appUser = _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)Context.User).Result;
            string userName = _appUser.FirstName + " " + _appUser.LastName;

            // To get the clients connected connection ID's
            var id = Context.ConnectionId;
            if (ConnectedUsers.Count(x => x.ConnectionId == id) == 0)
            {
                ConnectedUsers.Add(new UserDetail { ConnectionId = id, UserName = userName });

                // send to caller
                Clients.Caller.SendAsync("Connected", id, userName, ConnectedUsers, CurrentMessage);

                // send to all except caller client
                Clients.AllExcept(id).SendAsync("NewUserConnected", id, userName);
            }

            var group = Context.User.Identity.Name;
            Groups.AddToGroupAsync(Context.ConnectionId, group);

            usuarios.TryAdd(Context.ConnectionId, group);

            _hubContext.Clients.All.SendAsync("online", usuarios.Count);

            return Clients.Group(group).SendAsync("Send", $"{userName} entrou no Chat");
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            ApplicationUser appUser = _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)Context.User).Result;
            string userName = appUser.FirstName + " " + appUser.LastName;
            usuarios.TryRemove(Context.ConnectionId, out userName);

            _hubContext.Clients.All.SendAsync("online", usuarios.Count);

            return Clients.All.SendAsync("Send", $"{userName} saiu do Chat");
        }

        public void SendMessageToAll(string userName, string message)
        {
            // store last 100 messages in cache
            AddMessageinCache(userName, message);

            // Broad cast message
            Clients.All.SendAsync("messageReceived", userName, message);
        }

        #region private Messages send to individual
        private void AddMessageinCache(string userName, string message)
        {
            CurrentMessage.Add(new MessageDetail { UserName = userName, Message = message });

            if (CurrentMessage.Count > 100)
                CurrentMessage.RemoveAt(0);
        }
        #endregion

        public async Task SendMessage(string message, bool join)
        {
            var group = Context.User.Identity.Name;

            ApplicationUser appUser = _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)Context.User).Result;
            string userName = appUser.FirstName + " " + appUser.LastName;

            if (join)
            {
                await JoinRoom(group).ConfigureAwait(false);
                await Clients.Group(group).SendAsync("ReceiveMessage", userName, " entrou na sala " + group).ConfigureAwait(true);

            }
            else
            {
                await Clients.Group(group).SendAsync("ReceiveMessage", userName, message).ConfigureAwait(true);
            }
        }

        //Ok, now we can finally send to one user by username
        public void SendToUser(string to, string message)
        {
            ApplicationUser appUser = _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)Context.User).Result;
            string userName = appUser.FirstName + " " + appUser.LastName;

            //Send to every match in the dictionary, so users with multiple connections and the same name receive the message in all browsers
            foreach (KeyValuePair<string, string> user in usuarios)
            {
                if (user.Value.Equals(to))
                {
                    Clients.Group(user.Value).SendAsync("ReceiveMessage", userName, message);
                }
            }
        }

        public Task JoinRoom(string roomName)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, roomName);

        }
        public Task LeaveRoomProfessional(string roomName)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }

        public Task JoinRoomProfessional()
        {
            string roomName = "Room" + _appUser.Id;

            var profissional = _context.Profissionals
                                      .Include(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation)
                                      .Include(p => p.Pacientes).ThenInclude(p => p.IdPessoaNavigation).ThenInclude(p => p.AspuIdAspnetuserNavigation)
                                      .Where(p => p.IdPessoaNavigation.AspuIdAspnetuserNavigation.Id == _appUser.Id)
                                      .FirstOrDefault();

            return Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        }

        public Task LeaveRoomProfessional()
        {
            string roomName = "Room" + _appUser.Id;
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }

        public Task JoinRoomAdministrativo()
        {
            string roomName = "Room" + _appUser.Id;
            return Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        }

        public Task LeaveRoomAdministrativo()
        {
            string roomName = "Room" + _appUser.Id;
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }

        public Task SendMessageToGroup(string groupName, string message)
        {
            return Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId}: {message}");
        }

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupName}.");
        }
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
