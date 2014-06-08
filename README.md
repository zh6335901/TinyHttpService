TinyHttpService
===============

  简单的Http服务,可以根据http请求进行常见的操作。只需在你的程序（这个程序可以是WPF/Winform/Console等等）中引用并使用，你的程序就变成了一个简单的Http服务器了。这样你的程序就可以根据http请求做你想做的事了,比如：在web页面上操纵你的应用程序,进行文件上传下载等等。

使用方法
========

首先注册路由:
//全局路由表
var routes = RouteTable.Instance;

//GET /user
routes.Get("/user", (context) =>
{
    var user = new User();
    user.UserName = "zhang";
    return new ViewResult<User>("/view/razor.cshtml", user);
});

//POST /user
routes.Post("/user", (context) =>
{
    Console.WriteLine(context.Request.Body.ToString());
  
    //如果你的程序是wpf程序，需要控制UI元素，你需要使用:
    //Element.Dispatcher.Invoke方法或者Elment.Dispatcher.BeginInvoke方法
    return new ContentResult("haha");
});

然后监听请求:
//监听
TinyHttpService service = DefaultTinyHttpServiceFactory.GetDefaultTinyHttpService();
service.Bind(5000);

就这样！

