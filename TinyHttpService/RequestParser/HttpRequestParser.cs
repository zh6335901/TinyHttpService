using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using TinyHttpService.HttpData;
using TinyHttpService.RequestParser.Interface;
using TinyHttpService.Utils;

namespace TinyHttpService.RequestParser
{
    public class HttpRequestParser : IHttpRequestParser
    {
        public HttpRequest Parse(Stream stream)
        {
            HttpRequest request = new HttpRequest();
            HttpHeader header = new HttpHeader();

            string startLine = stream.ReadLine();
            var startLineRule = new Regex(@"^(GET|HEAD|POST|PUT|DELETE|TRACE|CONNECT|OPTION) (.+) HTTP/1.1$");

            if (startLineRule.IsMatch(startLine))
            {
                var match = startLineRule.Match(startLine);
                request.RequestMethod = match.Groups[1].Value.Trim();
                request.Uri = match.Groups[2].Value.Trim();
            }

            string line;
            var headerPropertyRule = new Regex(@"(.+?):(.+)");
            while ((line = stream.ReadLine()) != Environment.NewLine)
            {
                if (headerPropertyRule.IsMatch(line))
                {
                    var match = headerPropertyRule.Match(line);
                    header[match.Groups[1].Value.Trim()] = match.Groups[2].Value.Trim();
                }
            }

            request.Header = header;

            HttpRequestBody body = new HttpRequestBody();
           
            if (header["Content-Type"] != null)
            {
                RequestBodyDataParseCommand command = 
                            BodyParseCommandFactory.GetBodyParseCommand(header["Content-Type"]);
                body = command.Execute(stream);
            }
            else
            {
                throw new InvalidDataException("Content-Type can't be null");
            }
            request.Body = body;

            return request;
        }
    }
}
