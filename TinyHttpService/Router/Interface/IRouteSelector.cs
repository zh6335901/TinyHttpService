using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.ActionResults.Interface;
using TinyHttpService.HttpData;

namespace TinyHttpService.Router.Interface
{
    public interface IRouteSelector
    {
        RouteTable Routes { get; }
        Func<HttpContext, ActionResult> Select(HttpRequest request);
    }
}
