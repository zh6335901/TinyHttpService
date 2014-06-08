using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.ActionResults.Interface;
using TinyHttpService.HttpData;

namespace TinyHttpService.Router
{
    public class RouteTable
    {
        private static RouteTable routeTable;

        public static RouteTable Instance 
        {
            get 
            {
                if (routeTable == null)
                {
                    routeTable = new RouteTable();
                }

                return routeTable;
            }
        }

        private RouteTable() 
        {
            GetActions = new Dictionary<string, Func<HttpContext, ActionResult>>();
            PostActions = new Dictionary<string, Func<HttpContext, ActionResult>>();
            PutActions = new Dictionary<string, Func<HttpContext, ActionResult>>();
            DeleteActions = new Dictionary<string, Func<HttpContext, ActionResult>>();
            AllActions = new Dictionary<string, Func<HttpContext, ActionResult>>();
        }

        public Dictionary<string, Func<HttpContext, ActionResult>> GetActions { get; private set; }
        public Dictionary<string, Func<HttpContext, ActionResult>> PostActions { get; private set; }
        public Dictionary<string, Func<HttpContext, ActionResult>> PutActions { get; private set; }
        public Dictionary<string, Func<HttpContext, ActionResult>> DeleteActions { get; private set; }
        public Dictionary<string, Func<HttpContext, ActionResult>> AllActions { get; private set; }

        public RouteTable All(string url, Func<HttpContext, ActionResult> func)
        {
            AllActions[url] = func;
            return this;
        }

        public RouteTable Get(string url, Func<HttpContext, ActionResult> func)
        {
            GetActions[url] = func;
            return this;
        }

        public RouteTable Post(string url, Func<HttpContext, ActionResult> func)
        {
            PostActions[url] = func;
            return this;
        }

        public RouteTable Put(string url, Func<HttpContext, ActionResult> func)
        {
            PutActions[url] = func;
            return this;
        }

        public RouteTable Delete(string url, Func<HttpContext, ActionResult> func)
        {
            DeleteActions[url] = func;
            return this;
        }

        public Dictionary<string, Func<HttpContext, ActionResult>> this[string method] 
        {
            get 
            {
                method = method ?? string.Empty;
                switch (method.ToUpper()) 
                {
                    case "GET":
                        return GetActions;
                    case "POST":
                        return PostActions;
                    case "PUT":
                        return PutActions;
                    case "DELETE":
                        return DeleteActions;
                    default:
                        return new Dictionary<string,Func<HttpContext,ActionResult>>();
                }
            }
        }
    }
}
