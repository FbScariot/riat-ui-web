
using Chat.Models;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Generic;
using Chat.Repositories;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using RIAT.DAL.Entity.Models;
using RIAT.DAL.Entity.Data;
using System;
using System.Collections.Concurrent;
using System.Net;
using Nancy.Json;
using System.Text;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OneSignal.RestAPIv3.Client;
using OneSignal.RestAPIv3.Client.Resources.Notifications;
using OneSignal.RestAPIv3.Client.Resources;

namespace Chat.Hubs
{
    public class ChatHubNovo : Hub
    {
        private readonly static ConnectionsRepository _connections = new ConnectionsRepository();
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<ChatHubNovo> _hubContext;
        private readonly RIATContext _context;
        private readonly ILogger<ChatHubNovo> _logger;

        private static readonly ConcurrentDictionary<string, User> usersConnecteds = new ConcurrentDictionary<string, User>();

        public ChatHubNovo(UserManager<ApplicationUser> userManager, IHubContext<ChatHubNovo> hubContext, RIATContext context, ILogger<ChatHubNovo> logger)
        {
            _userManager = userManager;
            _hubContext = hubContext;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Override para inserir cada usuário no nosso repositório, lembrando que esse repositório está em memória
        /// </summary>
        /// <returns> Retorna lista de usuário no chat e usuário que acabou de logar </returns>
        public override Task OnConnectedAsync()
        {
            try
            {
                WriteErrorLog("Entrou no método OnConnectedAsync.");
                WriteErrorLog("Dados recebidos em Context.GetHttpContext().Request.Query['user'] : " + Context.GetHttpContext().Request.Query["user"]);
                //WriteErrorLog("Dados recebidos em Context.GetHttpContext().Request.Query['access_token'] : " + Context.GetHttpContext().Request.Query["access_token"]);
                WriteErrorLog("Context.ConnectionId : " + Context.ConnectionId);

                //WriteErrorLog("Context.User.Identity.IsAuthenticated : " + Context.User.Identity.IsAuthenticated);

                string userString = Context.GetHttpContext().Request.Query["user"];

                User userSerialized = JsonConvert.DeserializeObject<User>(userString);

                if (Context.User.Identity.IsAuthenticated)
                {
                    WriteErrorLog("Context encontrado em Context.User.Identity.Name : " + Context.User.Identity.Name);

                    //ApplicationUser appUser = _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)Context.User).Result;

                    //if (_connections.GetUserId(_appUser.UserName) == null)
                    //{
                    //var user = new User();
                    //user.dtConnection = DateTime.Now;
                    //DateTime startDt = new DateTime(1970, 1, 1);
                    //TimeSpan timeSpan = DateTime.UtcNow - startDt;
                    //long millis = (long)timeSpan.TotalMilliseconds;
                    //user.signalRConnectionId = appUser.Id;
                    //user.name = appUser.FirstName + " " + appUser.LastName;

                    //_connections.Add(Context.ConnectionId, userSerialized);

                    //Ao usar o método All eu estou enviando a mensagem para todos os usuários conectados no meu Hub
                    //Clients.All.SendAsync("chat", _connections.GetAllUser().Where(u => u.signalRConnectionId != userSerialized.signalRConnectionId).ToList(), userSerialized);

                    //WriteErrorLog("Usuários conectados: " + _connections.GetAllUser().Count());
                    //    }
                    //return Task.FromException(new Exception("Encontrou o contexto: Nome: " + Context.User.Identity.Name));
                }
                else
                {
                    //AspNetUser aspnetUser = _context.AspNetUsers.Where(a => a.Id == userSerialized.signalRConnectionId).FirstOrDefault();

                    //WriteErrorLog("aspnetUser.FirstName :" + aspnetUser.FirstName);

                    WriteErrorLog("Context NÃO encontrado em Context.User.Identity.Name : ");
                    //return Task.FromException(new Exception("Não encontrou o contexto"));
                }

                Clients.All.SendAsync("chat", _connections.GetAllUser().Where(u => u.signalRConnectionId != userSerialized.signalRConnectionId).ToList(), userSerialized);

                _connections.Add(Context.ConnectionId, userSerialized);

                return base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex.StackTrace);
                return Task.FromException(ex);
            }
            finally
            {
                WriteErrorLog("Saiu no método OnConnectedAsync.");
            }
        }

        private void WriteErrorLog(string dados)
        {
            try
            {
                string directory = @"C:\inetpub\wwwroot\Rism\Produção\Logs";
                DirectoryInfo di = null;

                try
                {
                    if (!Directory.Exists(directory))
                    {
                        directory = AppDomain.CurrentDomain.BaseDirectory + @"..\Logs";
                        di = Directory.CreateDirectory(directory);
                    }
                }
                catch (Exception)
                {
                    directory = AppDomain.CurrentDomain.BaseDirectory + @"..\Logs";
                    //directory = @"C:\inetpub\wwwroot\Rism\Produção\Logs";
                    di = Directory.CreateDirectory(directory);
                }

                using (StreamWriter sw = new StreamWriter(directory + @"\LogFile" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt", true))
                {
                    sw.WriteLine(DateTime.Now.ToString() + ": " + dados);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string userName = "";

            try
            {
                WriteErrorLog("Entrou no método OnDisconnectedAsync.");
                WriteErrorLog("Dados recebidos em Context.GetHttpContext().Request.Query['user'] : " + Context.GetHttpContext().Request.Query["user"]);
                //WriteErrorLog("Context.User.Identity.IsAuthenticated : " + Context.User.Identity.IsAuthenticated);
                WriteErrorLog("Context.ConnectionId : " + Context.ConnectionId);

                //if (Context.User.Identity.IsAuthenticated)
                //{
                //ApplicationUser appUser = _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)Context.User).Result;
                //userName = appUser.FirstName + " " + appUser.LastName;

                _connections.Remove(Context.ConnectionId);
                //}

                return Clients.All.SendAsync("Send", $"{userName} saiu do Chat");
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex.StackTrace);
                return Task.FromException(ex);
            }
            finally
            {
                WriteErrorLog("Saiu no método OnDisconnectedAsync.");
            }
        }

        /// <summary>
        /// Método responsável por encaminhar as mensagens pelo hub
        /// </summary>
        /// <param name="ChatMessage">Este parâmetro é nosso objeto representando a mensagem e os usuários envolvidos</param>
        /// <returns></returns>
        public async Task SendMessage(Message chat)
        {
            try
            {
                WriteErrorLog("Entrou no método SendMessage.");
                WriteErrorLog("Dados recebidos no método SendMessage: " + JsonConvert.SerializeObject(chat));

                string receiverConnectionId = _connections.GetConnectionId(chat.receiverSignalRConnectionId);

                if (receiverConnectionId != null)
                {
                    WriteErrorLog("Encontrou o receiverConnectionId : " + receiverConnectionId);

                    await Clients.Client(receiverConnectionId).SendAsync("Receive", chat.sender, chat.message);
                }
                else
                {
                    WriteErrorLog("NÃO Encontrou o receiverConnectionId : " + receiverConnectionId);
                    //await Clients.All.SendAsync("Receive", chat.sender, chat.message);
                }

                //await Clients.Client(_connections.GetUserId(chat.destination)).SendAsync("Receive", chat.sender, chat.message);

                //Ao usar o método Client(_connections.GetUserId(chat.destination)) eu estou enviando a mensagem apenas para o usuário destino, não realizando broadcast

                //string receiverUserId = _connections.GetAllUser().Where(u => u.SignalRConnectionId == chat.receiverSignalRConnectionId).First().SignalRConnectionId;

                List<string> includePlayerIds = new List<string>();

                //includePlayerIds = new List<string>()
                //{
                //    "b2cd6694-4b04-4327-99d4-c17495936e91" // Use your playerId
                //};

                string item = "";

                if (receiverConnectionId != null)
                {
                    
                    item = _connections.GetAllUser().Where(u => u.signalRConnectionId == chat.receiverSignalRConnectionId).First().oneSignalId;
                    includePlayerIds.Add(item);

                    //item = _context.OneSignalPlayersUsers.Where(o => o.UserId == chat.receiverSignalRConnectionId).OrderByDescending(o => o.Id).Select(o => o.PlayerId).First();
                    //includePlayerIds.Add(item);

                    //    includePlayerIds = new string[1] { _connections.GetAllUser().Where(u => u.signalRConnectionId == chat.receiverSignalRConnectionId).First().oneSignalId };

                    //await Clients.Client(receiverConnectionId).SendAsync("Receive", chat.sender, chat.message);
                }
                else
                {
                    item = _context.OneSignalPlayersUsers.Where(o => o.UserId == chat.receiverSignalRConnectionId).OrderByDescending(o => o.Id).Select(o => o.PlayerId).First();                    
                    includePlayerIds.Add(item);
                }

                if (includePlayerIds[0] != null) {
                    CreateNotification(chat, includePlayerIds);
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex.StackTrace);
                throw ex;
            }
            finally
            {
                WriteErrorLog("Saiu no método SendMessage.");
            }
        }

        public async Task SendMessageToGroup(Message chat)
        {
            try
            {
                WriteErrorLog("Entrou no método SendMessage.");
                WriteErrorLog("Dados recebidos no método SendMessage: " + JsonConvert.SerializeObject(chat));

                string receiverConnectionId = _connections.GetConnectionId(chat.receiverSignalRConnectionId);

                if (receiverConnectionId != null)
                {
                    //AspNetUser aspnetUser = _context.AspNetUsers.Include(a => a.as) Where(a => a.Id == userSerialized.receiverConnectionId).FirstOrDefault();
                    WriteErrorLog("Encontrou o receiverConnectionId : " + receiverConnectionId + " - Message redeived: " + JsonConvert.SerializeObject(chat));

                    await Clients.Client(receiverConnectionId).SendAsync("Receive", chat.sender, chat.message);
                }
                else
                {
                    WriteErrorLog("NÃO Encontrou o receiverConnectionId : " + receiverConnectionId + " - Message redeived: " + JsonConvert.SerializeObject(chat));
                    //await Clients.All.SendAsync("Receive", chat.sender, chat.message);
                }

                //await Clients.Client(_connections.GetUserId(chat.destination)).SendAsync("Receive", chat.sender, chat.message);

                //Ao usar o método Client(_connections.GetUserId(chat.destination)) eu estou enviando a mensagem apenas para o usuário destino, não realizando broadcast

                //string receiverUserId = _connections.GetAllUser().Where(u => u.SignalRConnectionId == chat.receiverSignalRConnectionId).First().SignalRConnectionId;

                List<string> includePlayerIds = new List<string>();

                if (receiverConnectionId != null)
                {
                    includePlayerIds.Add(_connections.GetAllUser().Where(u => u.signalRConnectionId == chat.receiverSignalRConnectionId).First().oneSignalId);
                }
                else
                {
                    includePlayerIds.Add( _context.OneSignalPlayersUsers.Where(o => o.UserId == receiverConnectionId).OrderByDescending(o => o.Id).Select(o => o.PlayerId).FirstOrDefault());
                }

                if (includePlayerIds.Count > 0)
                {
                    CreateNotification(chat, includePlayerIds);
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex.StackTrace);
                throw ex;
            }
            finally
            {
                WriteErrorLog("Saiu no método SendMessage.");
            }
        }

        private Action JsonResult(string v)
        {
            throw new NotImplementedException();
        }

        private void CreateNotification(Message chat, List<string> includePlayerIds)
        {
            try
            {
                WriteErrorLog("Entrou no método  CreateNotification : " + includePlayerIds[0].ToString());

                OneSignalClient client = new OneSignalClient("NzdjYWM1NzktMzUyYi00NDRmLWIwYjktZjA5ZTA1ZmM4OTkw"); // Use your Api Key

                WriteErrorLog("Criou o OneSignalClient : " + includePlayerIds[0].ToString());

                var options = new NotificationCreateOptions
                {
                    AppId = new Guid("03644759-a342-4958-9f26-2a032da0485e"),   // Use your AppId
                    //IncludePlayerIds = includePlayerIds

                    IncludePlayerIds = new List<string>()
                    {
                        "b2cd6694-4b04-4327-99d4-c17495936e91" // Use your playerId
                    }
                };

                options.Headings.Add(LanguageCodes.English, "Nova mensagem de " + chat.sender.name + " no Chat!");
                options.Contents.Add(LanguageCodes.English, chat.message);

                WriteErrorLog("Montou  options");

                NotificationCreateResult result = client.Notifications.Create(options);

                WriteErrorLog("Mensagem Enviada" + result.Recipients);
                //_logger.LogInformation("Mensagem Enviada " + result.Recipients);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Erro no método CreateNotification: " + ex.StackTrace);
                WriteErrorLog("Erro no método CreateNotification.");
                WriteErrorLog(ex.StackTrace);
                throw ex;
            }
            finally
            {
                WriteErrorLog("Saiu no método CreateNotification.");
            }
        }

        private void CreateNotification_old(Message chat, string[] includePlayerIds)
        {
            var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;

            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";

            request.Headers.Add("authorization", "Basic NGEwMGZmMjItY2NkNy0xMWUzLTk5ZDUtMDAwYzI5NDBlNjJj");

            var serializer = new JavaScriptSerializer();

            //include_player_ids = new string[] { "6392d91a-b206-4b7b-a620-cd68e32c3a76", "76ece62b-bcfe-468c-8a78-839aeaa8c5fa", "8e0f21fa-9a5a-4ae7-a9a6-ca1f24294b86" };

            var obj = new
            {
                app_id = "03644759-a342-4958-9f26-2a032da0485e",
                contents = new { en = "Nova mensagem de" + chat.sender.name },
                include_player_ids = includePlayerIds
            };

            var param = serializer.Serialize(obj);
            byte[] byteArray = Encoding.UTF8.GetBytes(param);

            string responseContent = null;

            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
            }

            System.Diagnostics.Debug.WriteLine(responseContent);
        }

        private void CreateNotificationToChatAttendants()
        {
            var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;

            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";

            request.Headers.Add("authorization", "Basic NGEwMGZmMjItY2NkNy0xMWUzLTk5ZDUtMDAwYzI5NDBlNjJj");

            var serializer = new JavaScriptSerializer();

            var obj = new
            {
                app_id = "03644759-a342-4958-9f26-2a032da0485e",
                contents = new { en = "Olá Atendentes" },
                included_segments = new string[] { "Chat Attendants" }
            };

            var param = serializer.Serialize(obj);
            byte[] byteArray = Encoding.UTF8.GetBytes(param);

            string responseContent = null;

            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
            }

            System.Diagnostics.Debug.WriteLine(responseContent);
        }
    }
}