using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.HttpData;
using TinyHttpService.RequestParser.Interface;

namespace TinyHttpService.RequestParser
{
    public class MultiPartFormDataParseCommand : RequestBodyDataParseCommand
    {
        public override HttpRequestBody Execute(Stream stream, Encoding e)
        {
            MultiPartFormDataParser parser = new MultiPartFormDataParser(stream, e);
            HttpRequestBody body = new HttpRequestBody();
            body.Properties = parser.Parameters;
            body.Files = parser.Files;
            return body;
        }
    }
}
