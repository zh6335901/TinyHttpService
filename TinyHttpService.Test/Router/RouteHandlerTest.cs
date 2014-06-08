using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyHttpService.Router;
using TinyHttpService.ActionResults;
using TinyHttpService.HttpData;

namespace TinyHttpService.Test.Router
{
    [TestClass]
    public class RouteHandlerTest
    {
        [TestInitialize]
        public void InitRouteTable() 
        {
            RouteTable.Instance.Get("/user/:zhang/:id", (context) =>
            {
                return new ContentResult("zhang");
            });

            RouteTable.Instance.Get("/index", (context) => 
            {
                return new ViewResult<User>("/index.cshtml", new User());
            });
        }

        [TestMethod]
        public void SelectExceptFuncAndCanParseRouteData()
        {
            HttpRequest request = new HttpRequest();
            request.Uri = "/user/zhang/123456?aaa=2";
            request.RequestMethod = "GET";
            RouteHandler handler = new RouteHandler();
            var func = handler.Handle(request);

            Assert.IsNotNull(func);
            var actionResult = func(null);
            Assert.AreEqual(actionResult.GetType(), typeof(ContentResult));

            Assert.AreEqual(request.RouteData.RouteUri, "/user/:zhang/:id");
            Assert.AreEqual(request.RouteData["zhang"], "zhang");
            Assert.AreEqual(request.RouteData["id"], "123456");
        }

        [TestCleanup]
        public void CleanRouteTable()
        {
            RouteTable.Instance.GetActions.Clear();
        }

        public class User 
        {
            public string UserName { get; set; }
        }
    }
}
