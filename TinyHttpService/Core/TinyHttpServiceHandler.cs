using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.Core;
using TinyHttpService.Core.Interface;
using TinyHttpService.HttpData;
using TinyHttpService.RequestParser.Interface;

namespace TinyHttpService.Implement
{
    public class TinyHttpServiceHandler : IHttpServiceHandler
    {
        private IHttpRequestParser requestParser;

        public TinyHttpServiceHandler(IHttpRequestParser requestParser)
        {
            this.requestParser = requestParser;
        }

        public void ProcessRequest(NetworkStream stream)
        {
            HttpRequest request = requestParser.Parse(stream);
            HttpResponse response = new HttpResponse(stream);

            HttpContext context = new HttpContext 
            {
                Request = request,
                Response = response
            };

            response.AddHeader("Content-Type", "text/html");
            response.Write("<html><head></head><body><div>Hello World!</div></body></html>");
        }
    }
}
