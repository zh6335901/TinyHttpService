using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.Core.Interface;
using TinyHttpService.Implement;
using TinyHttpService.RequestParser;
using TinyHttpService.RequestParser.Interface;
using TinyHttpService.Router;
using TinyHttpService.Router.Interface;

namespace TinyHttpService.Core
{
    public static class DefaultTinyHttpServiceFactory
    {
        public static TinyHttpService GetDefaultTinyHttpService() 
        {
            IRouteHandler routeHandler = new RouteHandler();
            IHttpRequestParser parser = new HttpRequestParser();
            IHttpServiceHandler handler = new TinyHttpServiceHandler(parser, routeHandler);

            return new TinyHttpService(handler);
        }
    }
}
