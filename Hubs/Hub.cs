using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RIAT.UI.Web.Hubs
{
    public abstract class Hub : IHub, IDisposable
    {
        public Microsoft.AspNet.SignalR.Hubs.HubCallerContext Context { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IHubCallerConnectionContext<dynamic> Clients { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Microsoft.AspNet.SignalR.IGroupManager Groups { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task OnConnected()
        {
            throw new NotImplementedException();
        }

        public Task OnDisconnected(bool stopCalled)
        {
            throw new NotImplementedException();
        }

        public Task OnReconnected()
        {
            throw new NotImplementedException();
        }
    }
}
