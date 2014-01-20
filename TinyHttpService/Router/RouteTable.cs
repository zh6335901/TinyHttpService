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
            var action = AllActions[url];
            if (action != null)
            {
                action += func;
            }
            else
            {
                action = func;
            }

            return this;
        }

        public RouteTable Get(string url, Func<HttpContext, ActionResult> func)
        {
            var action = GetActions[url];
            if (action != null)
            {
                action += func;
            }
            else
            {
                action = func;
            }

            return this;
        }

        public RouteTable Post(string url, Func<HttpContext, ActionResult> func)
        {
            var action = GetActions[url];
            if (action != null)
            {
                action += func;
            }
            else
            {
                action = func;
            }

            return this;
        }

        public RouteTable Put(string url, Func<HttpContext, ActionResult> func)
        {
            var action = GetActions[url];
            if (action != null)
            {
                action += func;
            }
            else
            {
                action = func;
            }

            return this;
        }

        public RouteTable Delete(string url, Func<HttpContext, ActionResult> func)
        {
            var action = GetActions[url];
            if (action != null)
            {
                action += func;
            }
            else
            {
                action = func;
            }

            return this;
        }
    }
}
