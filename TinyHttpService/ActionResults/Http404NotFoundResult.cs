using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.ActionResults.Interface;

namespace TinyHttpService.ActionResults
{
    public class Http404NotFoundResult : ActionResult
    {
        public override void Execute(HttpData.HttpContext context)
        {
            throw new NotImplementedException();
        }
    }
}
