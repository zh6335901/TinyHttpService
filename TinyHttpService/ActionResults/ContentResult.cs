using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.ActionResults;
using TinyHttpService.HttpData;

namespace TinyHttpService.ActionResults
{
    public class ContentResult : ActionResult
    {
        public string Content { get; set; }

        public ContentResult() { }

        public ContentResult(string content)
        {
            this.Content = content;
        }

        public override async Task ExecuteAsync(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var response = context.Response;
            response.ContentType = "text/plain; charset=utf-8";
            response.StatusCode = 200;
            response.AddHeader("Content-Length", Encoding.UTF8.GetByteCount(Content).ToString());
            await response.WriteAsync(Content);
        }
    }
}
