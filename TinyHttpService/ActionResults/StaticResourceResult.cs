using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.ActionResults;
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

        public override async Task ExecuteAsync(HttpContext context)
        {
            var fullpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FilePath.TrimStart('/', '\\'));
            var response = context.Response;
            var request = context.Request;

            if (File.Exists(fullpath)) 
            {
                if (request.Header["If-Modified-Since"] != null)
                {
                    DateTime time;
                    if (DateTime.TryParse(request.Header["If-Modified-Since"], out time))
                    {
                        if (time.Ticks >= File.GetLastWriteTimeUtc(fullpath).Ticks) 
                        {
                            response.StatusCode = 304;
                            await response.WriteAsync(string.Empty);
                            return;
                        }
                    }
                }

                var ext = Path.GetExtension(fullpath);
                response.StatusCode = 200;
                response.ContentType = Mime.Get(ext) ?? "application/octet-stream";
                response.AddHeader("Last-Modified", File.GetLastWriteTime(fullpath).ToUniversalTime().ToString("r"));

                var buffer = new byte[4096];
                int readBytes = 0;
                using (var fs = new FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.Read)) 
                {
                    response.AddHeader("Content-Length", fs.Length.ToString());
                    while ((readBytes = await fs.ReadAsync(buffer, 0, 4096)) > 0)
                    {
                        await response.WriteAsync(buffer);
                    }
                }
            }
            else 
            {
                await (new Http404NotFoundResult()).ExecuteAsync(context);
            }
        }
    }
}
