using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TinyHttpService.Utils
{
    public static class StreamUtil
    {
        /// <summary>
        /// 从当前流的位置开始读取一行
        /// </summary>
        /// <returns></returns>
        public static string ReadLine(this Stream stream, Encoding e)
        {
            var line = stream.ReadRawLine(e);
            if (line.Contains("\r\n"))
            {
                line = line.Substring(0, line.Length - 2);
            }
            else if (line.Contains('\n'))
            {
                line = line.Substring(0, line.Length - 1);
            }
            return line;
        }

        /// <summary>
        /// 从流中读取一行，并且去除换行符
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static String ReadRawLine(this Stream stream, Encoding e)
        {
            var bytes = new List<byte>();
            int value = -1;
            string line = string.Empty;

            NetworkStream networkStream = stream as NetworkStream;
            if (networkStream != null)
            {
                if (!networkStream.DataAvailable)
                {
                    return string.Empty;
                }
            }

            while (stream.CanRead 
                    && (networkStream == null || networkStream.DataAvailable) 
                    && (value = stream.ReadByte()) != -1)
            {
                bytes.Add(Convert.ToByte(value));

                if (value == '\n')
                {
                    break;
                }
            }

            return e.GetString(bytes.ToArray());
        }

        public static void Write(this Stream stream, string str)
        {
            if (stream.CanWrite)
            {
                byte[] bytes = Encoding.Default.GetBytes(str);
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
                return;
            }

            throw new InvalidOperationException("stream can't write");
        }
    }
}
