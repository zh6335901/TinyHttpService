using System;
using ServiceStack.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyHttpService.RequestParser;
using System.IO;
using System.Text;

namespace TinyHttpService.Test.RequestParser
{
    [TestClass]
    public class JsonBodyDataParseCommandTest
    {
        [TestMethod]
        public void CanParseJsonToHttpRequestBody()
        {
            string json = JsonSerializer.SerializeToString(new { Username = "zhang", Password = "123456", Age = 1 });
            var command = new JsonBodyDataParseCommand();

            using (var ms = new MemoryStream(Encoding.Default.GetBytes(json)))
            {
                var body = command.Execute(ms, Encoding.Default);
                Assert.AreEqual(body["Age"], "1");
                Assert.AreEqual(body["Username"], "zhang");
                Assert.AreEqual(body["Password"], "123456");
            }
        }
    }
}
