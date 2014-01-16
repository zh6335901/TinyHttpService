﻿using System;
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
        public HttpRequest Parse(NetworkStream stream)
        {
            const string CRLF = "\r\n";

            HttpRequest request = new HttpRequest();
            HttpHeader header = new HttpHeader();
            HttpRequestBody body;

            string startLine = ReadLine(stream);
            var startLineRule = new Regex(@"^(GET|HEAD|POST|PUT|DELETE|TRACE|CONNECT) (.+) HTTP/1.1\r\n$");

            if (startLineRule.IsMatch(startLine))
            {
                var match = startLineRule.Match(startLine);
                request.RequestMethod = match.Groups[1].Value.Trim();
                request.Uri = match.Groups[2].Value.Trim();
            }

            string line;
            var headerPropertyRule = new Regex(@"(.+?):(.+)");
            while ((line = ReadLine(stream)) != CRLF)
            {
                if (headerPropertyRule.IsMatch(line))
                {
                    var match = headerPropertyRule.Match(line);
                    header[match.Groups[1].Value.Trim()] = match.Groups[2].Value.Trim();
                }
            }

            request.Header = header;

            //非GET请求            
            if (request.RequestMethod != "GET")
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
                        if (line != CRLF)
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
            return stream.ReadLine();
        }
    }
}