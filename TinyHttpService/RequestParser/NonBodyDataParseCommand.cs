using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TinyHttpService.HttpData;
using TinyHttpService.RequestParser.Interface;

namespace TinyHttpService.RequestParser
{
    public class NonBodyDataParseCommand : RequestBodyDataParseCommand
    {
        public override HttpRequestBody Execute(Stream stream, Encoding e)
        {
            return new HttpRequestBody();
        }
    }
}
