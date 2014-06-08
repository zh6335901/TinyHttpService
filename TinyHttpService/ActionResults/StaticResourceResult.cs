using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.ActionResults.Interface;
using TinyHttpService.HttpData;

namespace TinyHttpService.ActionResults
{
    public class StaticResourceResult : ActionResult
    {
        public string FilePath { get; set; }

        public StaticResourceResult(string path) 
        {
            this.FilePath = path;
        }

        public override void Execute(HttpContext context)
        {
            var fullpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FilePath.TrimStart('/', '\\'));

            if (File.Exists(fullpath)) 
            {
                var ext = Path.GetExtension(fullpath);

                var response = context.Response;
                response.StatusCode = 200;
                response.ContentType = Mime.Get(ext) ?? "application/octet-stream";

                var buffer = new byte[4096];
                int readBytes = 0;
                using (var fs = new FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.Read)) 
                {
                    response.AddHeader("Content-Length", fs.Length.ToString());
                    while ((readBytes = fs.Read(buffer, 0, 4096)) > 0)
                    {
                        response.Write(buffer);
                    }
                }

                response.End();
            }
            else 
            {
                (new Http404NotFoundResult()).Execute(context);
            }
        }
    }
}
