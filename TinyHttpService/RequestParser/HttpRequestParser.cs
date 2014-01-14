using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using TinyHttpService.RequestData;
using TinyHttpService.RequestParser.Interface;
using TinyHttpService.Utils;

namespace TinyHttpService.RequestParser
{
    public class HttpRequestParser : IHttpRequestParser
    {
        public HttpRequest Parse(NetworkStream stream)
        {
            HttpRequest request = new HttpRequest();
            HttpRequestHeader header = new HttpRequestHeader();
            HttpRequestBody body;

            string startLine = ReadLine(stream);
            var startLineRule = new Regex(@"^(GET|HEAD|POST|PUT|DELETE|TRACE|CONNECT) (.+) HTTP/1.1$");

            if (startLineRule.IsMatch(startLine))
            {
                var match = startLineRule.Match(startLine);
                header.RequestMethod = match.Groups[1].Value.Trim();
                header.Uri = match.Groups[2].Value.Trim();
            }

            string line;
            var headerPropertyRule = new Regex(@"(.+?):(.+)");
            while ((line = ReadLine(stream)) != Environment.NewLine)
            {
                if (headerPropertyRule.IsMatch(line))
                {
                    var match = headerPropertyRule.Match(line);
                    header[match.Groups[1].Value.Trim()] = match.Groups[2].Value.Trim();
                }
            }

            //非GET请求            
            if (header.RequestMethod != "GET")
            {
                body = new HttpRequestBody();
                if (header["Content-Type"] != null && header["Content-Type"].Contains("multipart/form-data"))
                { 
                    //待实现
                }
                else
                {
                    while (string.IsNullOrEmpty(line = ReadLine(stream)))
                    {
                        if (line != Environment.NewLine)
                        {
                            var bodyProperties = line.Split('&');
                            foreach (var bodyProperty in bodyProperties)
                            {
                                var keyValuePair = bodyProperty.Split('=');
                                body[keyValuePair[0]] = keyValuePair[1];
                            }
                        }
                    }
                }
                request.Body = body;
            }

            return request;
        }

        private string ReadLine(NetworkStream stream)
        {
            return StreamUtil.ReadLine(stream);
        }
    }
}
