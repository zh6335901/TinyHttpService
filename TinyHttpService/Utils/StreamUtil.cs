using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public static string ReadLine(Stream stream)
        { 
            var bytes = new List<byte>();

            while (stream.CanRead)
            {
                int value = stream.ReadByte();
                if (value == -1) return string.Empty;

                bytes.Add(Convert.ToByte(value));

                if (value == '\n')
                {
                    var line = Encoding.Default.GetString(bytes.ToArray());
                    return line;
                }
            }

            throw new InvalidOperationException();
        }
    }
}
