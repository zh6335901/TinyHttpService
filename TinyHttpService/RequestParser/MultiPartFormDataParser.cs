using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.HttpData;
using TinyHttpService.Utils;

namespace TinyHttpService.RequestParser
{
    public class MultiPartFormDataParser
    {
        const int DEFAULT_BUFFER_LENGTH = 4096;

        private string boundary;
        private byte[] boundaryBytes;
        private string endBoundary;
        private byte[] endBoundaryBytes;
        private readonly Encoding encoding;

        private bool readEndBoundary = false;
        private RebufferableStreamReader reader;

        public int BinaryBufferSize { get; set; }
        public List<FilePart> Files { get; set; }
        public Dictionary<string, string> Parameters { get; set; }

        public MultiPartFormDataParser(Stream stream, Encoding e, string boundaryKey = null, 
                                    int binaryBufferSize = DEFAULT_BUFFER_LENGTH)
        {
            encoding = e ?? Encoding.UTF8;
            reader = new RebufferableStreamReader(stream, encoding);

            BinaryBufferSize = binaryBufferSize;
            Files = new List<FilePart>();
            Parameters = new Dictionary<string, string>();
        }

        private async Task<string> DetectBoundaryKey(RebufferableStreamReader reader)
        {
            var line = await reader.ReadLineAsync();
            var boundary = string.Concat(line.Skip(2));
            reader.Rebuffer(encoding.GetBytes("--" + boundary + "\r\n"));
            return boundary;
        }

        public async Task ParseAsync(string boundaryKey = null)
        {
            if (string.IsNullOrEmpty(boundaryKey))
            {
                boundaryKey = await DetectBoundaryKey(reader);
            }

            boundary = string.Format("--{0}", boundaryKey);
            endBoundary = string.Format("--{0}--", boundaryKey);
            boundaryBytes = encoding.GetBytes(boundary);
            endBoundaryBytes = encoding.GetBytes(endBoundary);

            string line = await reader.ReadLineAsync();
            while (!this.readEndBoundary)
            {
                string partHeader = string.Empty;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (line != boundary) 
                    {
                        partHeader += line;
                    }
                }

                Dictionary<string, string> partHeaders;
                try
                {
                    partHeaders = partHeader.Replace("\r\n", ";").Split(';')
                                            .Select(x => x.Split(new[] { ':', '=' }))
                                            .ToDictionary(
                                                x => x[0].Trim().Replace("\"", string.Empty).ToLower(),
                                                x => x[1].Trim().Replace("\"", string.Empty));
                }
                catch 
                {
                    throw new HttpRequestParseException("http报文实体格式错误");
                }

                if (!partHeaders.ContainsKey("filename"))
                {
                    await ParseParameterPartAsync(reader, partHeaders);
                }
                else
                {
                    await ParseFilePartAsync(reader, partHeaders);
                }

            }
        }

        private async Task ParseParameterPartAsync(RebufferableStreamReader reader, Dictionary<string, string> partHeaders)
        {
            StringBuilder value = new StringBuilder();
            string line = await reader.ReadLineAsync();
            while (line != endBoundary && line != boundary)
            {
                value.Append(line);
                line = await reader.ReadLineAsync();
            }

            if (line == endBoundary)
            {
                this.readEndBoundary = true;
            }
            Parameters[partHeaders["name"]] = value.ToString();
        }

        private async Task ParseFilePartAsync(RebufferableStreamReader reader, Dictionary<string, string> partHeaders)
        {
            //先使用内存存储吧，遇到大文件肯定是不行的
            MemoryStream ms = new MemoryStream();
            var curBuffer = new byte[this.BinaryBufferSize];
            var prevBuffer = new byte[this.BinaryBufferSize];

            int curLength = 0;
            int prevLength = await reader.ReadAsync(prevBuffer, 0, prevBuffer.Length);
            int endPos = -1;

            do
            {
                curLength = await reader.ReadAsync(curBuffer, 0, curBuffer.Length);
                var fullBuffer = new Byte[prevLength + curLength];
                Buffer.BlockCopy(prevBuffer, 0, fullBuffer, 0, prevLength);
                Buffer.BlockCopy(curBuffer, 0, fullBuffer, prevLength, curLength);

                int endBoundaryPos = SubsenquenceFinder.Search(fullBuffer, endBoundaryBytes);
                int boundaryPos = SubsenquenceFinder.Search(fullBuffer, boundaryBytes);

                if (endBoundaryPos >= 0 && boundaryPos >= 0)
                {
                    if (boundaryPos < endBoundaryPos)
                    {
                        endPos = boundaryPos;
                    }
                    else
                    {
                        endPos = endBoundaryPos;
                        this.readEndBoundary = true;
                    }
                }
                else if (boundaryPos >= 0 && endBoundaryPos < 0)
                {
                    endPos = boundaryPos;
                }
                else if (endBoundaryPos >= 0 && boundaryPos < 0)
                {
                    endPos = endBoundaryPos;
                    this.readEndBoundary = true;
                }

                if (endPos != -1)
                { 
                    //这里endPos减2的原因是去除CRLF
                    ms.Write(fullBuffer, 0, endPos - 2);
                    var rebuffer = new byte[fullBuffer.Length - endPos];
                    Buffer.BlockCopy(fullBuffer, endPos, rebuffer, 0, rebuffer.Length);
                    reader.Rebuffer(rebuffer);
                    ms.Flush();
                    ms.Position = 0;
                    FilePart filePart = new FilePart()
                    {
                        Filename = partHeaders["filename"],
                        Name = partHeaders["name"],
                        Data = ms
                    };

                    this.Files.Add(filePart);
                    return;
                }

                ms.Write(prevBuffer, 0, prevLength);
                ms.Flush();
                prevBuffer = curBuffer;
                prevLength = curLength;
            }
            while (prevLength > 0);

            if (endPos == -1)
            {
                ms.Close();
                throw new HttpRequestParseException("http报文实体格式错误");
            }
        }
    }
}
