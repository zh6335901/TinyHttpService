using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.Core.Interface;

namespace TinyHttpService.Core
{
    public abstract class TinyHttpServiceBase : IHttpService
    {
        private TcpListener listener;
        private IHttpServiceHandler httpServiceHandler;

        public TinyHttpServiceBase(IHttpServiceHandler handler)
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
                    Task task = new Task(new Action<object>((obj) =>
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
                            //这边有疑问，现在http 1.1协议下默认是keep-alive的
                            //所以我是否可以使用timer来延迟一段时间再关闭连接呢
                            tcpClient.Close();
                        }

                    }), client);
                    task.Start();
                }
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
