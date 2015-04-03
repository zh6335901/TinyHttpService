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
        public override async Task<HttpRequestBody> ExecuteAsync(Stream stream, Encoding e)
        {
            string line;
            StringBuilder formBodySb = new StringBuilder();
            HttpRequestBody body = new HttpRequestBody();

            var reader = new StreamReader(stream, e);
            while (!string.IsNullOrEmpty(line = await reader.ReadLineAsync()))
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
