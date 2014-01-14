using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyHttpService.RequestData
{
    public class HttpRequest
    {
        public HttpRequestHeader Header { get; internal set; }
        public HttpRequestBody Body { get; internal set; }
    }
}
