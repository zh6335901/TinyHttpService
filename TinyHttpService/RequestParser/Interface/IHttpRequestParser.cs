using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.RequestData;

namespace TinyHttpService.RequestParser.Interface
{
    public interface IHttpRequestParser
    {
        HttpRequest Parse(NetworkStream stream);
    }
}
