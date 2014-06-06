using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyHttpService.RequestParser
{
    public class HttpRequestParseException : Exception
    {
        public HttpRequestParseException() { }
        public HttpRequestParseException(string message) : base(message) { }
    }
}
