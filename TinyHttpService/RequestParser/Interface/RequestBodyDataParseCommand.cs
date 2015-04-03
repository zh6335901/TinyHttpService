using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.HttpData;

namespace TinyHttpService.RequestParser.Interface
{
    public abstract class RequestBodyDataParseCommand
    {
        public abstract Task<HttpRequestBody> ExecuteAsync(Stream stream, Encoding e);
    }
}
