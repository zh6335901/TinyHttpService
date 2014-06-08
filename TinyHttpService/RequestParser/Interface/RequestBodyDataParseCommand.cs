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
        public abstract HttpRequestBody Execute(Stream stream, Encoding e);
    }
}
