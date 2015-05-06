using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.Core;
using TinyHttpService.RequestParser;
using TinyHttpService.Router;

namespace TinyHttpService
{
    public static class DefaultTinyHttpServiceFactory
    {
        public static HttpService GetDefaultTinyHttpService() 
        {
            IRouteHandler routeHandler = new RouteHandler();
            IHttpRequestParser parser = new HttpRequestParser();
            IHttpServiceHandler handler = new HttpServiceHandler(parser, routeHandler);

            return new HttpService(handler);
        }
    }
}
