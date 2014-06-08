using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinyHttpService.Core.Interface;

namespace TinyHttpService.Core
{
    public class TinyHttpService : IHttpService
    {
        private TcpListener listener;
        private IHttpServiceHandler httpServiceHandler;

        public TinyHttpService(IHttpServiceHandler handler)
        {
            this.httpServiceHandler = handler;
        }

        public virtual void Bind(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            listener.BeginAcceptTcpClient(AcceptCallback, listener);
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            var listener = ar.AsyncState as TcpListener;

            try
            {
                var client = listener.EndAcceptTcpClient(ar);

                if (client != null)
                {
                    Thread task = new Thread((obj) =>
                    {
                        var tcpClient = obj as TcpClient;
                        var stream = tcpClient.GetStream();
                        try
                        {
                            httpServiceHandler.ProcessRequest(stream);
                        }
                        finally
                        {
                            stream.Close();
                            tcpClient.Close();
                        }

                    });
                    task.Start(client);
                }
            }
            catch (ObjectDisposedException)
            {
                // ignore
            }
            finally
            {
                listener.BeginAcceptTcpClient(AcceptCallback, listener);
            }
        }

        public virtual void Close()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            if (listener != null)
            {
                listener.Stop();
            }
        }
    }
}
