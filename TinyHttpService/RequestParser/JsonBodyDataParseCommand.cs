using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.HttpData;
using TinyHttpService.RequestParser.Interface;
using TinyHttpService.Utils;

namespace TinyHttpService.RequestParser
{
    public class JsonBodyDataParseCommand : RequestBodyDataParseCommand
    {
        public override HttpRequestBody Execute(Stream stream, Encoding e)
        {
            Dictionary<string, string> dict = JsonSerializer.DeserializeFromStream<Dictionary<string, string>>(stream);
            HttpRequestBody body = new HttpRequestBody();
            body.Properties = dict;
            return body;
        }
    }
}
