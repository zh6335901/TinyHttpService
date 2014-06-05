using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyHttpService.Core.Interface
{
    public interface IHttpServiceHandler
    {
        void ProcessRequest(Stream stream);
    }
}
