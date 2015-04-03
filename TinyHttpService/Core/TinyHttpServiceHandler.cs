using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.ActionResults;
using TinyHttpService.ActionResults.Interface;
using TinyHttpService.Core;
using TinyHttpService.Core.Interface;
using TinyHttpService.HttpData;
using TinyHttpService.RequestParser;
using TinyHttpService.RequestParser.Interface;
using TinyHttpService.Router;
using TinyHttpService.Router.Interface;

namespace TinyHttpService.Implement
{
    public class TinyHttpServiceHandler : IHttpServiceHandler
    {
        private IHttpRequestParser requestParser;
        private IRouteHandler routeHandler;

        public TinyHttpServiceHandler(IHttpRequestParser requestParser, IRouteHandler routeHandler)
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
