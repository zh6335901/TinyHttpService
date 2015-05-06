using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyHttpService.Core
{
    public interface IHttpServiceHandler
    {
        Task ProcessRequestAsync(Stream stream);
    }
}
