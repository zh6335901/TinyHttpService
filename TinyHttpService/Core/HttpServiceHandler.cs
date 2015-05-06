using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.ActionResults;
using TinyHttpService.Core;
using TinyHttpService.HttpData;
using TinyHttpService.RequestParser;
using TinyHttpService.Router;

namespace TinyHttpService
{
    public class HttpServiceHandler : IHttpServiceHandler
    {
        private IHttpRequestParser requestParser;
        private IRouteHandler routeHandler;

        public HttpServiceHandler(IHttpRequestParser requestParser, IRouteHandler routeHandler)
        {
            this.requestParser = requestParser;
            this.routeHandler = routeHandler;
        }

        public async Task ProcessRequestAsync(Stream stream)
        {
            HttpResponse response = new HttpResponse(stream);
            HttpRequest request = null;

            bool isParseSuccess;
            try
            {
                request = await requestParser.ParseAsync(stream);
                isParseSuccess = true;
            }
            catch (HttpRequestParseException e) 
            {
                response.StatusCode = 400;
                isParseSuccess = false;
            }

            if (!isParseSuccess) 
            {
                await response.WriteAsync("Bad Request");
            }

            HttpContext context = new HttpContext 
            {
                Request = request,
                Response = response
            };

            var func = routeHandler.Handle(request);
            if (func != null)
            {
                ActionResult actionResult = func(context);
                await actionResult.ExecuteAsync(context);
            }
            else
            {
                await (new Http404NotFoundResult()).ExecuteAsync(context);
            }
        }
    }
}
