using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.HttpData;

namespace TinyHttpService.RequestParser
{
    public interface IHttpRequestParser
    {
        Task<HttpRequest> ParseAsync(Stream stream);
    }
}
