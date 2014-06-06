using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.ActionResults.Interface;
using TinyHttpService.HttpData;
using TinyHttpService.Router.Interface;

namespace TinyHttpService.Router
{
    public class RouteHandler : IRouteHandler
    {
        public RouteTable Routes
        {
            get
            {
                return RouteTable.Instance;
            }
        }

        public Func<HttpContext, ActionResult> Handle(HttpRequest request)
        {
            string method = request.RequestMethod;
            string url = request.Uri;

            var func = SelectAndParseRouteData(request, Routes[method], url);

            if (func != null) 
            {
                return func;
            }

            return SelectAndParseRouteData(request, Routes.AllActions, url);
        }

        private Func<HttpContext, ActionResult> SelectAndParseRouteData(
                                                HttpRequest request, 
                                                Dictionary<string, Func<HttpContext, ActionResult>> routes, 
                                                string requestUrl)
        {
            string[] urlArray = requestUrl.Split('/');

            foreach (var kv in routes)
            {
            startMatch:
                string[] routeArray = kv.Key.Split('/');
                if (routeArray.Length == urlArray.Length)
                {
                    for (int i = 0; i < routeArray.Length; i++)
                    {
                        if (routeArray[i].StartsWith(":"))
                        {
                            request.RouteData[routeArray[i].TrimStart(':')] = urlArray[i];
                        }
                        else
                        {
                            if (routeArray[i] != urlArray[i])
                            {
                                request.RouteData.DataTokens.Clear();
                                goto startMatch;
                            }
                        }
                    }
                    return Routes[request.RequestMethod][kv.Key];
                }
            }

            return null;
        }
    }
}
