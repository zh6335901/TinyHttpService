using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyHttpService.Utils
{
    public class RebufferableStreamReader : IDisposable
    {
        private byte[] buffer;
        private Stream stream;

        public RebufferableStreamReader(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            this.stream = stream;
        }

        public void Rebuffer(byte[] bytes)
        {
            if (buffer == null)
            {
                buffer = bytes;
            }
            else
            {
                buffer = buffer.Concat(bytes).ToArray();
            }
        }

        public string ReadLine()
        {
            if (buffer == null || buffer.Length == 0)
            {
                return stream.ReadLine();
            }
            else 
            {
                for (int i = 0; i < buffer.Length; i++) 
                {
                    if (buffer[i] == '\n')
                    {
                        var lineBytes = new byte[i + 1];
                        var newBuffer = new byte[buffer.Length - i - 1];
                        Array.Copy(buffer, lineBytes, i + 1);
                        Array.Copy(buffer, i + 1, newBuffer, 0, buffer.Length - i - 1);
                        buffer = newBuffer;

                        var line = Encoding.Default.GetString(lineBytes);
                        if (line.Contains("\r\n"))
                        {
                            return line.Substring(0, line.Length - 2);
                        }
                        else
                        {
                            return line.Substring(0, line.Length - 1);
                        }
                    }
                }

                var l = Encoding.Default.GetString(buffer) + stream.ReadRawLine();
                buffer = null;
                if (l.Contains("\r\n"))
                {
                    return l.Substring(0, l.Length - 2);
                }
                else if (l.Contains('\n'))
                {
                    return l.Substring(0, l.Length - 1);
                }
                else 
                {
                    return l;
                }
            }
        }

        public int Read(byte[] bytes, int offset, int length)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }

            if (buffer != null)
            {
                if (buffer.Length > bytes.Length)
                {
                    Buffer.BlockCopy(buffer, 0, bytes, 0, bytes.Length);
                    var newBuffer = new byte[buffer.Length - bytes.Length];
                    Buffer.BlockCopy(buffer, bytes.Length, newBuffer, 0, newBuffer.Length);
                    buffer = newBuffer;
                    return bytes.Length;
                }
                else if (buffer.Length < bytes.Length)
                {
                    int readCount = stream.Read(bytes, buffer.Length, bytes.Length - buffer.Length);
                    Buffer.BlockCopy(buffer, 0, bytes, 0, buffer.Length);
                    readCount += buffer.Length;
                    buffer = null;
                    return readCount;
                }
                else
                {
                    Buffer.BlockCopy(buffer, 0, bytes, 0, buffer.Length);
                    buffer = null;
                    return bytes.Length;
                }
            }
            else
            {
                return stream.Read(bytes, offset, length);
            }
        }

        public void Dispose()
        {
            stream.Close();
        }
    }
}
