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
        public async Task<HttpRequest> ParseAsync(Stream stream)
        {
            var streamReader = new StreamReader(stream);
            string startLine = await streamReader.ReadLineAsync();

            HttpRequest request = new HttpRequest();
            var startLineRule = new Regex(@"^(GET|HEAD|POST|PUT|DELETE|TRACE|CONNECT|OPTIONS) (.+) HTTP/1.1$");

            if (startLineRule.IsMatch(startLine))
            {
                var match = startLineRule.Match(startLine);
                request.RequestMethod = match.Groups[1].Value.Trim();
                request.Uri = UrlHelper.UrlDecode(match.Groups[2].Value.Trim(), Encoding.UTF8);
                request.QueryString = UrlHelper.GetQueryString(request.Uri);
            }
            else 
            {
                throw new HttpRequestParseException("http格式错误");
            }

            request.Header = await GetHttpHeaderAsync(streamReader);

            Encoding encoding = GetBodyEncoding(request.Header["Content-Type"] ?? string.Empty);
           
            RequestBodyDataParseCommand command = 
                        BodyParseCommandFactory.GetBodyParseCommand(request.Header["Content-Type"]);
            HttpRequestBody body = await command.ExecuteAsync(stream, encoding);

            request.Body = body;
            return request;
        }

        private static async Task<HttpHeader> GetHttpHeaderAsync(StreamReader reader)
        {
            HttpHeader header = new HttpHeader();
            string line;
            var headerPropertyRule = new Regex(@"(.+?):(.+)");
            while ((line = await reader.ReadLineAsync()) != String.Empty)
            {
                if (headerPropertyRule.IsMatch(line))
                {
                    var match = headerPropertyRule.Match(line);
                    header[match.Groups[1].Value.Trim()] = match.Groups[2].Value.Trim();
                }
                else 
                {
                    throw new HttpRequestParseException("http头部格式错误");
                }
            }

            return header;
        }

        private static Encoding GetBodyEncoding(string contentType)
        {
            var regex = new Regex(@"charset=([A-Za-z0-9\-]+)");
            string charset = string.Empty;

            if (regex.IsMatch(contentType))
            {
                var matcher = regex.Match(contentType);
                charset = matcher.Groups[1].Value;
            }
            Encoding encoding = charset == string.Empty ? Encoding.UTF8 : Encoding.GetEncoding(charset);
            return encoding;
        }
    }
}
