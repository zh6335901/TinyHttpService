using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.Utils;

namespace TinyHttpService.HttpData
{
    public class HttpResponse
    {
        public Stream ResponseStream { get; private set; }
        public HttpHeader Header { get; set; }

        private int statusCode;
        private bool isHeaderWritten;

        public HttpResponse(Stream stream)
        {
            this.ResponseStream = stream;
            this.Header = new HttpHeader();
            this.ContentType = "text/html";
            this.statusCode = 200;
            this.isHeaderWritten = false;
        }

        public void AddHeader(string key, string value)
        {
            if (isHeaderWritten)
            {
                throw new InvalidOperationException("http头部已经发送，无法填加新的http头部");
            }

            this.Header[key] = value;
        }

        public int StatusCode
        {
            get
            {
                return statusCode;
            }
            set
            {
                if (HttpStatusCodes.StatusCodes[value] == null)
                {
                    throw new InvalidOperationException("无效的状态码: " + value.ToString());
                }

                statusCode = value;
            }
        }

        private async Task WriteHeadAsync()
        {
            string head = string.Format("HTTP/1.1 {0} {1}\r\n{2}\r\n",
                                            StatusCode, HttpStatusCodes.StatusCodes[StatusCode], Header.ToString());

            var headBytes = Encoding.UTF8.GetBytes(head);
            await ResponseStream.WriteAsync(Encoding.UTF8.GetBytes(head), 0, headBytes.Length);
            ResponseStream.Flush();
            isHeaderWritten = true;
        }

        public async Task WriteAsync(byte[] bytes)
        {
            if (!isHeaderWritten)
            {
                await WriteHeadAsync();
            }

            if (bytes != null)
            {
                await ResponseStream.WriteAsync(bytes, 0, bytes.Length);
            }
        }

        public async Task WriteAsync(string str)
        {
            if (!isHeaderWritten)
            {
                await WriteHeadAsync();
            }

            if (!string.IsNullOrEmpty(str))
            {
                var bytes = Encoding.UTF8.GetBytes(str);
                await ResponseStream.WriteAsync(bytes, 0, bytes.Length);
            }

            await ResponseStream.FlushAsync();
        }

        private string contentType;
        public string ContentType 
        {
            get 
            {
                return contentType;
            }
            set 
            {
                Header["Content-Type"] = value;
                this.contentType = value;
            }
        }
    }
}
