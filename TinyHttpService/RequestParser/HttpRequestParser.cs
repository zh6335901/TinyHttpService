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
        public HttpRequest Parse(NetworkStream stream)
        {
            HttpRequest request = new HttpRequest();
            HttpHeader header = new HttpHeader();
            HttpRequestBody body;

            string startLine = ReadLine(stream);
            var startLineRule = new Regex(@"^(GET|HEAD|POST|PUT|DELETE|TRACE|CONNECT) (.+) HTTP/1.1$");

            if (startLineRule.IsMatch(startLine))
            {
                var match = startLineRule.Match(startLine);
                request.RequestMethod = match.Groups[1].Value.Trim();
                request.Uri = match.Groups[2].Value.Trim();
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

            request.Header = header;

            //非GET请求            
            if (request.RequestMethod != "GET")
            {
                body = new HttpRequestBody();
           
                if (header["Content-Type"] != null && header["Content-Type"].Contains("multipart/form-data"))
                {
                    var match = new Regex(@"boundary=(.+)").Match(header["Content-Type"]);
                    string boundary = match.Groups[1].Value;

                    string splitter = string.Format("--{0}", boundary);
                    string end = string.Format("--{0}--", boundary);

                    ParsePart(stream, body, splitter, end);
                }
                else
                {
                    while (string.IsNullOrEmpty(line = ReadLine(stream)))
                    {
                        var bodyProperties = line.Split('&');
                        foreach (var bodyProperty in bodyProperties)
                        {
                            var keyValuePair = bodyProperty.Split('=');
                            body[keyValuePair[0]] = keyValuePair[1];
                        }
                    }
                }
                request.Body = body;
            }

            return request;
        }

        private void ParsePart(NetworkStream stream, HttpRequestBody body, string splitter, string end)
        {
            string line = ReadLine(stream);
            while (line != end)
            {
                if (line == splitter)
                {
                    break;
                }

                string first = ReadLine(stream);
                string second = string.Empty;
                if (first != end)
                {
                    Dictionary<string, string> paramters = first.Split(';')
                                                              .Select(x => x.Split(new[] { ':', '=' }))
                                                              .ToDictionary(
                                                                    x => x[0].Trim().Replace("\"", string.Empty).ToLower(),
                                                                    x => x[1].Trim().Replace("\"", string.Empty));

                    if (!paramters.ContainsKey("filename"))
                    {
                        StringBuilder value = new StringBuilder();
                        do
                        {
                            line = ReadLine(stream);
                            value.AppendLine(line);
                        }
                        while (line != end && line != splitter);

                        body[paramters["name"]] = value.ToString();
                    }
                    else
                    {
                        //先使用内存存储吧，遇到大文件肯定是不行的
                        MemoryStream ms = new MemoryStream();
                    }
                }
            }
        }

        private string ReadLine(NetworkStream stream)
        {
            return stream.ReadLine();
        }
    }
}
