using System;
using ServiceStack.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.RequestParser.Commands;

namespace TinyHttpService.Test.RequestParser
{
    [TestClass]
    public class JsonBodyDataParseCommandTest
    {
        [TestMethod]
        public async Task CanParseJsonToHttpRequestBody()
        {
            string json = JsonSerializer.SerializeToString(new { Username = "zhang", Password = "123456", Age = 1 });
            var command = new JsonBodyDataParseCommand();

            using (var ms = new MemoryStream(Encoding.Default.GetBytes(json)))
            {
                var body = await command.ExecuteAsync(ms, Encoding.Default);
                Assert.AreEqual(body["Age"], "1");
                Assert.AreEqual(body["Username"], "zhang");
                Assert.AreEqual(body["Password"], "123456");
            }
        }
    }
}
