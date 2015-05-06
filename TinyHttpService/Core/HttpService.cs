using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinyHttpService.Core;

namespace TinyHttpService.Core
{
    public class HttpService : IHttpService
    {
        private TcpListener listener;
        private IHttpServiceHandler httpServiceHandler;
        private bool active = true;

        public HttpService(IHttpServiceHandler handler)
        {
            this.httpServiceHandler = handler;
        }

        public virtual async void Bind(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            while (active)
            {
                using (var client = await listener.AcceptTcpClientAsync())
                {
                    while (client.Connected 
                                && client.Client.Poll(01, SelectMode.SelectRead)
                                && client.Client.Poll(01, SelectMode.SelectWrite)
                                && !client.Client.Poll(01, SelectMode.SelectError))
                    {
                        if (client.GetStream().DataAvailable)
                        {
                            try
                            {
                                await httpServiceHandler.ProcessRequestAsync(client.GetStream());
                            }
                            catch (IOException)
                            {
                                //when client interrupt connect, it would be catched.
                                //ignore...
                            }
                        }
                    }
                }
            }
        }

        public virtual void Close()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            active = false;

            if (listener != null)
            {
                listener.Stop();
            }
        }
    }
}
