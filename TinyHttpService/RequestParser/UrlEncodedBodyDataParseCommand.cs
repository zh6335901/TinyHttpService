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
    public class UrlEncodedBodyDataParseCommand : RequestBodyDataParseCommand
    {
        public override HttpRequestBody Execute(Stream stream, Encoding e)
        {
            string line;
            StringBuilder formBodySb = new StringBuilder();
            HttpRequestBody body = new HttpRequestBody();

            while (!string.IsNullOrEmpty(line = stream.ReadLine(e)))
            {
                formBodySb.Append(line);
            }

            var bodyString = formBodySb.ToString();
            bodyString = UrlHelper.UrlDecode(bodyString, e);
            var bodyProperties = bodyString.Split('&');
            foreach (var bodyProperty in bodyProperties)
            {
                var keyValuePair = bodyProperty.Split('=');
                if (keyValuePair.Length >= 2)
                {
                    body[keyValuePair[0]] = keyValuePair[1];
                }
            }

            return body;
        }
    }
}
