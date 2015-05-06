using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyHttpService.Core
{
    public interface IHttpService : IDisposable
    {
        /// <summary>
        /// 监听
        /// </summary>
        /// <param name="port">端口号</param>
        void Bind(int port);
    }
}
