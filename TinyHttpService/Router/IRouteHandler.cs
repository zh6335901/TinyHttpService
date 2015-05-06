using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.ActionResults;
using TinyHttpService.HttpData;

namespace TinyHttpService.Router
{
    public interface IRouteHandler
    {
        RouteTable Routes { get; }

        Func<HttpContext, ActionResult> Handle(HttpRequest request);
    }
}
