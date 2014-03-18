using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyHttpService.Utils
{
    internal class RebufferableStreamReader
    {
        private byte[] buffer;
        private Stream stream;

        internal RebufferableStreamReader(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            this.stream = stream;
        }

        internal void Rebuffer(byte[] bytes)
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

        internal string ReadLine()
        {
            return stream.ReadLine();
        }

        internal int Read(byte[] bytes, int offset, int length)
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
    }
}
