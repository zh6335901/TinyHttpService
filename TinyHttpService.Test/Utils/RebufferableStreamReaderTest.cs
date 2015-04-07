using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using TinyHttpService.Utils;
using System.Threading.Tasks;

namespace TinyHttpService.Test.Utils
{
    [TestClass]
    public class RebufferableStreamReaderTest
    {
        private static readonly string readerTestString = "ni hao wo shi zhanghao\r\nthis is test!";
        private static readonly string lineBufferTestString = "hahaha\r\nhehe";
        private static readonly string noLineBufferTestString = "this buffer no line";

        [TestMethod]
        public async Task ReadLineWhenReaderNoBuffer()
        {
            using (var ms = new MemoryStream(Encoding.Default.GetBytes(readerTestString))) 
            {
                RebufferableStreamReader reader = new RebufferableStreamReader(ms, Encoding.Default);
                string line = await reader.ReadLineAsync();
                string nextLine = await reader.ReadLineAsync();

                Assert.AreEqual(line, "ni hao wo shi zhanghao");
                Assert.AreEqual(nextLine, "this is test!");
            }
        }

        [TestMethod]
        public async Task ReadLineWhenLineInBuffer() 
        {
            using (var ms = new MemoryStream(Encoding.Default.GetBytes(readerTestString)))
            {
                RebufferableStreamReader reader = new RebufferableStreamReader(ms, Encoding.Default);
                reader.Rebuffer(Encoding.Default.GetBytes(lineBufferTestString));

                var line = await reader.ReadLineAsync();
                var secondLine = await reader.ReadLineAsync();
                var thirdLine = await reader.ReadLineAsync();

                Assert.AreEqual(line, "hahaha");
                Assert.AreEqual(secondLine, "hehe" + "ni hao wo shi zhanghao");
                Assert.AreEqual(thirdLine, "this is test!");

                reader.Rebuffer(Encoding.Default.GetBytes("hello world"));
                var fourLine = await reader.ReadLineAsync();
                Assert.AreEqual(fourLine, "hello world");
            }
        }

        [TestMethod]
        public async Task ReadLineWhenReaderHaveBufferAndNoLineInBuffer() 
        {
            using (var ms = new MemoryStream(Encoding.Default.GetBytes(readerTestString)))
            {
                RebufferableStreamReader reader = new RebufferableStreamReader(ms, Encoding.Default);
                reader.Rebuffer(Encoding.Default.GetBytes(noLineBufferTestString));

                var line = await reader.ReadLineAsync();
                var secondLine = await reader.ReadLineAsync();
                var thirdLine = await reader.ReadLineAsync();

                Assert.AreEqual(line, noLineBufferTestString + "ni hao wo shi zhanghao");
                Assert.AreEqual(secondLine, "this is test!");
                Assert.IsTrue(string.IsNullOrEmpty(thirdLine));
            }
        }
    }
}
