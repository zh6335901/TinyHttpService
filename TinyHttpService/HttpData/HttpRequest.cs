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
        public string RequestMethod { get; set; }

        public string Uri { get; set; }

        public HttpHeader Header { get; set; }

        public HttpRequestBody Body { get; set; }

        public RouteData RouteData { get; set; }

        public Dictionary<string, string> QueryString { get; set; }
    }
}
