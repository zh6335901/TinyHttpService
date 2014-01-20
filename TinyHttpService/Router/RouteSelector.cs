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
    public class RouteSelector : IRouteSelector
    {
        public RouteTable Routes
        {
            get
            {
                return RouteTable.Instance;
            }
        }

        public Func<HttpContext, ActionResult> Select(HttpRequest request)
        {
            string method = request.RequestMethod;
            string url = request.Uri;
            Func<HttpContext, ActionResult> func = null;

            switch (method.ToUpper())
            { 
                case "GET":
                    func = Routes.GetActions[url];
                    break;
                case "POST":
                    func = Routes.PostActions[url];
                    break;
                case "PUT":
                    func = Routes.DeleteActions[url];
                    break;
                case "DELETE":
                    func = Routes.DeleteActions[url];
                    break;
                default:
                    break;
            }

            if (func == null)
            {
                func = Routes.AllActions[url];
            }

            return func;
        }
    }
}
