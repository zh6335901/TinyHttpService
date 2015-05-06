using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TinyHttpService.ActionResults;
using TinyHttpService.HttpData;
using TinyHttpService.Router;
using TinyHttpService.Utils;

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

        private static Func<HttpContext, ActionResult> staticFileFuncCache;

        static RouteHandler() 
        {
            staticFileFuncCache = new Func<HttpContext, ActionResult>((context) =>
            {
                string requestFile = string.Empty;
                if (context.Request.Uri == "/favicon.ico") 
                {
                    requestFile = context.Request.Uri;
                }
                else 
                {
                    requestFile = TinyHttpServiceConfig.StaticFilePath + context.Request.Uri;
                }

                return new StaticResourceResult(requestFile);
            });
        }

        public Func<HttpContext, ActionResult> Handle(HttpRequest request)
        {
            if (IsRequestStaticFile(request.Uri)) 
            {
                return staticFileFuncCache;
            }

            var func = SelectAndParseRouteData(request, Routes[request.RequestMethod]);

            if (func != null) 
            {
                return func;
            }

            return SelectAndParseRouteData(request, Routes.AllActions);
        }

        private bool IsRequestStaticFile(string url) 
        {
            Regex rule = new Regex(@"\.(htm|html|js|css|ico|jpg|jpeg|png|bmp|txt|zip|mp4|mp3|avi|wma|wmv)$");
            return "/favicon.ico" == url.ToLower() || rule.IsMatch(url);
        }

        private Func<HttpContext, ActionResult> SelectAndParseRouteData(
                                                HttpRequest request, 
                                                Dictionary<string, Func<HttpContext, ActionResult>> routes)
        {
            string urlParam = request.Uri.Split('?')[0];
            string[] urlArray = urlParam.Split('/');
            request.RouteData = new RouteData();

            foreach (var kv in routes)
            {
                string[] routeArray = kv.Key.Split('/');
                bool isMatch = true;
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
                                isMatch = false;
                                continue;
                            }
                        }
                    }

                    if (isMatch)
                    {
                        request.RouteData.RouteUri = kv.Key;
                        return Routes[request.RequestMethod][kv.Key];
                    }
                }
            }

            return null;
        }
    }
}
