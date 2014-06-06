using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.Router;

namespace TinyHttpService.HttpData
{
    public class HttpRequest
    {
        public string RequestMethod { get; internal set; }
        public string Uri { get; internal set; }

        public HttpHeader Header { get; internal set; }
        public HttpRequestBody Body { get; internal set; }
        public RouteData RouteData { get; internal set; }
    }
}
