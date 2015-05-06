using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.HttpData;

namespace TinyHttpService.RequestParser.Commands
{
    public class MultiPartFormDataParseCommand : RequestBodyDataParseCommand
    {
        public override async Task<HttpRequestBody> ExecuteAsync(Stream stream, Encoding e)
        {
            MultiPartFormDataParser parser = new MultiPartFormDataParser(stream, e);
            await parser.ParseAsync();
            HttpRequestBody body = new HttpRequestBody();
            body.Properties = parser.Parameters;
            body.Files = parser.Files;
            return body;
        }
    }
}
