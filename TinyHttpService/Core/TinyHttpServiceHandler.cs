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
using TinyHttpService.RequestParser.Interface;
using TinyHttpService.Router;
using TinyHttpService.Router.Interface;

namespace TinyHttpService.Implement
{
    public class TinyHttpServiceHandler : IHttpServiceHandler
    {
        private IHttpRequestParser requestParser;
        private IRouteSelector routeSelector;

        public TinyHttpServiceHandler(IHttpRequestParser requestParser, IRouteSelector routeSelector)
        {
            this.requestParser = requestParser;
            this.routeSelector = routeSelector;
        }

        public void ProcessRequest(Stream stream)
        {
            RouteTable.Instance.GetActions["/login"] = (httpContext) =>
            {
                JsonResult result = new JsonResult();
                result.Data = new { Name = "zhanghao", Phone = "111111" };
                return result;
            };


            HttpRequest request = requestParser.Parse(stream);
            HttpResponse response = new HttpResponse(stream);

            HttpContext context = new HttpContext 
            {
                Request = request,
                Response = response
            };

            var func = routeSelector.Select(request);
            if (func != null)
            {
                ActionResult actionResult = func(context);
                actionResult.Execute(context);
            }
            else
            {
                (new Http404NotFoundResult()).Execute(context);
            }
        }
    }
}
