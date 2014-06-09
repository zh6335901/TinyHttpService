using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.ActionResults.Interface;
using TinyHttpService.HttpData;

namespace TinyHttpService.ActionResults
{
    public class Http404NotFoundResult : ActionResult
    {
        public override void Execute(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var response = context.Response;
            response.StatusCode = 404;
            response.ContentType = "text/html";
            response.Write("<h1>Not Found</h1>");
        }
    }
}
