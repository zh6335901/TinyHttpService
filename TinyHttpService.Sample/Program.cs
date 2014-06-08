using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.ActionResults;
using TinyHttpService.Core;
using TinyHttpService.Router;

namespace TinyHttpService.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            RegisteRoute();
            var service = DefaultTinyHttpServiceFactory.GetDefaultTinyHttpService();
            service.Bind(5000);

            Console.ReadKey();
        }

        private static void RegisteRoute()
        {
            var routes = RouteTable.Instance;

            routes.Get("/user/:id", (context) =>
            {
                Console.WriteLine(context.Request.RouteData["id"]);

                return new ContentResult("张浩");
            });

            routes.Post("/user", (context) =>
            {
                Console.WriteLine(context.Request.Body.ToString());
                
                //如果你的程序是wpf程序，需要控制UI元素，你需要使用:
                //Element.Dispatcher.Invoke方法或者Elment.Dispatcher.BeginInvoke方法

                return new ContentResult("haha");
            });

            routes.Get("/user", (context) =>
            {
                var user = new User();
                user.UserName = "zhang";
                return new ViewResult<User>("/view/razor.cshtml", user);
            });

            routes.Get("/download", (context) =>
            {
                return new DownloadResult("/file/8.png");
            });
        }
    }
}
