using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.ActionResults.Interface;
using TinyHttpService.HttpData;
using RazorEngine;
using System.IO;

namespace TinyHttpService.ActionResults
{
    public class ViewResult<T> : ActionResult
    {
        public string ViewPath { get; set; }
        public T Model { get; set; }

        public ViewResult(string viewPath, T model)
        {
            this.ViewPath = viewPath;
            this.Model = model;
        }

        public override void Execute(HttpContext context)
        {
            var fullpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ViewPath.TrimStart('/', '\\'));
            if (File.Exists(fullpath)) 
            {
                var razor = File.ReadAllText(fullpath, System.Text.Encoding.UTF8);
                string html = Razor.Parse(razor, this.Model);

                var response = context.Response;
                response.StatusCode = 200;
                response.ContentType = "text/html; charset=utf-8";
                response.AddHeader("Content-Length", html.Length.ToString());
                response.Write(html);
            }
            else 
            {
                (new Http404NotFoundResult()).Execute(context);
            }
        }
    }
}
