using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.ActionResults;
using TinyHttpService.HttpData;

namespace TinyHttpService.ActionResults
{
    public class JsonResult : ActionResult
    {
        public object Data { get; set; }

        public JsonResult(object data) 
        {
            this.Data = data;
        }

        public JsonResult() { }

        public override async Task ExecuteAsync(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var response = context.Response;
            response.ContentType = "application/json; charset=utf-8";
            response.StatusCode = 200;

            if (Data == null)
            {
                await response.WriteAsync(string.Empty);
                return;
            }

            var json = JsonSerializer.SerializeToString(Data, Data.GetType());
            response.AddHeader("Content-Length", Encoding.UTF8.GetByteCount(json).ToString());
            await response.WriteAsync(json);
        }
    }
}
