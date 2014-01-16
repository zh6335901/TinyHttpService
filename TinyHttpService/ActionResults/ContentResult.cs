using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.ActionResults.Interface;
using TinyHttpService.HttpData;

namespace TinyHttpService.ActionResults
{
    public class ContentResult : ActionResult
    {
        public string Content { get; set; }

        public override void Execute(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var response = context.Response;
            response.ContentType = "text/plain";
            //待实现
        }
    }
}
