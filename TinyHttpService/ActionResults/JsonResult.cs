using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.ActionResults.Interface;
using TinyHttpService.HttpData;

namespace TinyHttpService.ActionResults
{
    public class JsonResult : ActionResult
    {
        public object Data { get; set; }

        public override void Execute(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = 200;

            if (Data == null)
            {
                response.Write(string.Empty);
                return;
            }

            var json = JsonSerializer.SerializeToString(Data, Data.GetType());
            response.AddHeader("Content-Length", Encoding.Default.GetByteCount(json).ToString());
            response.Write(json);
        }
    }
}
