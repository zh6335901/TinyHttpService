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
                throw new InvalidOperationException("can't add header after headers sent.");
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
                    throw new InvalidOperationException("Invalid status code");
                }

                statusCode = value;
            }
        }

        private void WriteHead()
        {
            string head = string.Format("HTTP/1.1 {0} {1}\r\n{2}\r\n",
                                            StatusCode, HttpStatusCodes.StatusCodes[StatusCode], Header.ToString());

            ResponseStream.Write(head);
            isHeaderWritten = true;
        }

        public void Write(byte[] bytes)
        {
            if (!isHeaderWritten)
            {
                WriteHead();
            }

            if (bytes != null)
            {
                ResponseStream.Write(bytes, 0, bytes.Length);
            }
        }

        public void Write(string str)
        {
            if (!isHeaderWritten)
            {
                WriteHead();
            }

            if (!string.IsNullOrEmpty(str))
            {
                ResponseStream.Write(str);
            }
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
