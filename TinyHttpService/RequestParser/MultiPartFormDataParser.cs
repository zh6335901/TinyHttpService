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

        private readonly string boundary;
        private readonly byte[] boundaryBytes;
        private readonly string endBoundary;
        private readonly byte[] endBoundaryBytes;

        private bool readEndBoundary = false;
        private RebufferableStreamReader reader;

        public int BinaryBufferSize { get; set; }
        public List<FilePart> Files { get; set; }
        public Dictionary<string, string> Parameters { get; set; }

        public MultiPartFormDataParser(Stream stream, string boundaryKey = null, int binaryBufferSize = DEFAULT_BUFFER_LENGTH)
        {
            reader = new RebufferableStreamReader(stream);
            if (string.IsNullOrEmpty(boundaryKey)) 
            {
                boundaryKey = DetectBoundaryKey(reader);
            }

            boundary = string.Format("--{0}", boundaryKey);
            endBoundary = string.Format("--{0}--", boundaryKey);
            boundaryBytes = Encoding.Default.GetBytes(boundary);
            endBoundaryBytes = Encoding.Default.GetBytes(endBoundary);

            BinaryBufferSize = binaryBufferSize;
            Files = new List<FilePart>();
            Parameters = new Dictionary<string, string>();
            Parse();
        }

        private string DetectBoundaryKey(RebufferableStreamReader reader)
        {
            var boundary = string.Concat(reader.ReadLine().Skip(2));
            reader.Rebuffer(Encoding.Default.GetBytes("--" + boundary + "\r\n"));
            return boundary;
        }

        private void Parse()
        {
            string line = reader.ReadLine();
            while (!this.readEndBoundary)
            {
                if (line == boundary)
                {
                    break;
                }

                string partHeader = string.Empty;
                while ((line = reader.ReadLine()) != string.Empty)
                {
                    partHeader += line;
                }

                Dictionary<string, string> partHeaders = partHeader.Replace("\r\n", ";").Split(';')
                                        .Select(x => x.Split(new[] { ':', '=' }))
                                        .ToDictionary(
                                            x => x[0].Trim().Replace("\"", string.Empty).ToLower(),
                                            x => x[1].Trim().Replace("\"", string.Empty));


                if (!partHeaders.ContainsKey("filename"))
                {
                    ParseParameterPart(reader, partHeaders);
                }
                else
                {
                    ParseFilePart(reader, partHeaders);
                }

            }
        }

        private void ParseParameterPart(RebufferableStreamReader reader, Dictionary<string, string> partHeaders)
        {
            StringBuilder value = new StringBuilder();
            string line = string.Empty;
            do
            {
                line = reader.ReadLine();
                value.Append(line);
            }
            while (line != endBoundary && line != boundary);

            if (line == endBoundary)
            {
                this.readEndBoundary = true;
            }
            Parameters[partHeaders["name"]] = value.ToString();
        }

        private void ParseFilePart(RebufferableStreamReader reader, Dictionary<string, string> partHeaders)
        {
            //先使用内存存储吧，遇到大文件肯定是不行的
            MemoryStream ms = new MemoryStream();
            var curBuffer = new byte[this.BinaryBufferSize];
            var prevBuffer = new byte[this.BinaryBufferSize];

            int curLength = 0;
            int prevLength = reader.Read(prevBuffer, 0, prevBuffer.Length);
            int endPos = -1;
            int endPosLength = 0;

            do
            {
                curLength = reader.Read(curBuffer, 0, curBuffer.Length);
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
                        endPosLength = boundaryBytes.Length;
                    }
                    else
                    {
                        endPos = endBoundaryPos;
                        endPosLength = endBoundaryBytes.Length;
                        this.readEndBoundary = true;
                    }
                }
                else if (boundaryPos >= 0 && endBoundaryPos < 0)
                {
                    endPos = boundaryPos;
                    endPosLength = boundaryBytes.Length;
                }
                else if (endBoundaryPos >= 0 && boundaryPos < 0)
                {
                    endPos = endBoundaryPos;
                    endPosLength = endBoundaryBytes.Length;
                    this.readEndBoundary = true;
                }

                if (endPos != -1)
                { 
                    //读到part结束，注意要处理多读出来的部分
                    ms.Write(fullBuffer, 0, endPos);
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
        }
    }
}
