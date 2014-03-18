using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyHttpService.Utils
{
    public static class SubsenquenceFinder
    {
        public static int Search(byte[] bytes, byte[] target)
        {
            if (bytes == null || target == null || 
                    bytes.Length == 0 || target.Length == 0)
            {
                return -1;
            }

            if (target.Length == 1)
            {
                for (int index = 0; index < bytes.Length; index++)
                {
                    if (bytes[index] == target[0])
                    {
                        return index;
                    }
                }
                return -1;
            }

            int m = 0;
            int i = 0;
            int pos = m;

            while (pos <= bytes.Length - target.Length)
            {
                while (bytes[m] == target[i])
                {
                    m++;
                    i++;
                    if (i == target.Length)
                    {
                        return pos;
                    }

                    if (m == bytes.Length)
                    {
                        return -1;
                    }
                }

                pos++;
                m = pos;
                i = 0;
            }

            return -1;
        }
    }
}
