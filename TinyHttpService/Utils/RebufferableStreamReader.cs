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
        private Encoding encoding;

        public RebufferableStreamReader(Stream stream, Encoding e)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            this.encoding = e ?? Encoding.UTF8;
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

        public async Task<string> ReadLineAsync()
        {
            if (buffer == null || buffer.Length == 0)
            {
                var bytes = new byte[4096];
                int readCount = await stream.ReadAsync(buffer, 0, bytes.Length);
                if (readCount < bytes.Length)
                {
                    return encoding.GetString(bytes);
                }
                else 
                {
                    for (int i = 0; i < bytes.Length; i++) 
                    {
                        if (bytes[i] == '\n') 
                        {
                            var lineBytes = new byte[i + 1];
                            buffer = new byte[bytes.Length - i - 1];
                            Array.Copy(bytes, lineBytes, i + 1);
                            Array.Copy(bytes, i + 1, buffer, 0, bytes.Length - i - 1);

                            var line = encoding.GetString(lineBytes);
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
                }
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

                        var line = encoding.GetString(lineBytes);
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

                var lineEnd = await streamReader.ReadLineAsync();
                var l = encoding.GetString(buffer) + lineEnd ?? string.Empty;
                buffer = null;
                return l;
            }
        }

        public async Task<int> ReadAsync(byte[] bytes, int offset, int length)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset必须大于0");
            }

            if (offset + length > bytes.Length) 
            {
                throw new ArgumentOutOfRangeException("bytes长度必须大于offset与length之和");
            }

            if (buffer != null)
            {
                if (buffer.Length > length)
                {
                    Buffer.BlockCopy(buffer, 0, bytes, offset, length);
                    var newBuffer = new byte[buffer.Length - bytes.Length];
                    Buffer.BlockCopy(buffer, bytes.Length, newBuffer, 0, newBuffer.Length);
                    buffer = newBuffer;
                    return length;
                }
                else if (buffer.Length < length)
                {             
                    int readCount = await stream.ReadAsync(bytes, buffer.Length, length - buffer.Length);
                    Buffer.BlockCopy(buffer, 0, bytes, offset, buffer.Length);
                    readCount += buffer.Length;
                    buffer = null;
                    return readCount;
                }
                else
                {
                    Buffer.BlockCopy(buffer, 0, bytes, offset, length);
                    buffer = null;
                    return length;
                }
            }
            else
            {
                return await stream.ReadAsync(bytes, offset, length);
            }
        }

        public void Dispose()
        {
            stream.Close();
        }
    }
}
