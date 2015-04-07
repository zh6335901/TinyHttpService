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

        public void Rebuffer(byte[] bytes, bool bufferEnd = false)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }

            if (buffer == null)
            {
                buffer = bytes;
            }
            else
            {
                if (!bufferEnd)
                {
                    buffer = bytes.Concat(buffer).ToArray();
                }
                else
                {
                    buffer = buffer.Concat(bytes).ToArray();
                }
            }
        }

        public async Task<string> ReadLineAsync()
        {
            string line;
            int newLineIndex;
            buffer = buffer ?? new byte[0];

            if (TryGetLine(buffer, buffer.Length, out line, out newLineIndex)) 
            {
                byte[] newBuffer = new byte[buffer.Length - newLineIndex];
                Array.Copy(buffer, newLineIndex, newBuffer, 0, newBuffer.Length);
                buffer = newBuffer;

                return line;
            }

            byte[] readBytes = new byte[4096];
            int readCount = 0;
            do
            {
                readCount = await stream.ReadAsync(readBytes, 0, readBytes.Length);

                if (TryGetLine(readBytes, readCount, out line, out newLineIndex)) 
                {
                    line = encoding.GetString(buffer) + line;

                    buffer = new byte[readCount - newLineIndex];
                    Array.Copy(readBytes, newLineIndex, buffer, 0, buffer.Length);

                    return line;
                }
    
                if (readCount > 0) 
                {
                    buffer = buffer.Concat(readBytes).ToArray();
                }
            }
            while (readCount > 0);

            if (buffer.Length == 0)
            {
                //end of stream
                return null;
            }

            line = encoding.GetString(buffer);
            buffer = null;
            return line;
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

        private bool TryGetLine(byte[] bytes, int count, out string line, out int newLineIndex)
        {
            for (int i = 0; i < count; i++)
            {
                if (bytes[i] == '\n')
                {
                    int lineBytesCount = i;
                    if (i > 0 && bytes[i - 1] == '\r')
                    {
                        lineBytesCount -= 1;
                    }

                    byte[] lineBytes = new byte[lineBytesCount];
                    byte[] newBuffer = new byte[count - i - 1];
                    Array.Copy(bytes, lineBytes, lineBytesCount);

                    line = encoding.GetString(lineBytes);
                    newLineIndex = i + 1;
                    return true;
                }
            }

            line = string.Empty;
            newLineIndex = -1;
            return false;
        }
    }
}
