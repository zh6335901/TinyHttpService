using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyHttpService.HttpData
{
    public class HttpRequest
    {
        public string RequestMethod { get; internal set; }
        public string Uri { get; internal set; }

        public HttpHeader Header { get; internal set; }
        public HttpRequestBody Body { get; internal set; }
    }
}
