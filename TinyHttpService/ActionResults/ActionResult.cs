using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.HttpData;

namespace TinyHttpService.ActionResults
{
    public abstract class ActionResult
    {
        public abstract Task ExecuteAsync(HttpContext context);
    }
}
