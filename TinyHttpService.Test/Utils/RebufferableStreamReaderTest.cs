using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using TinyHttpService.Utils;

namespace TinyHttpService.Test.Utils
{
    [TestClass]
    public class RebufferableStreamReaderTest
    {
        private static readonly string readerTestString = "ni hao wo shi zhanghao\r\nthis is test!";
        private static readonly string lineBufferTestString = "hahaha\r\nhehe";
        private static readonly string noLineBufferTestString = "this buffer no line";

        [TestMethod]
        public void ReadLineWhenReaderNoBuffer()
        {
            using (var ms = new MemoryStream(Encoding.Default.GetBytes(readerTestString))) 
            {
                RebufferableStreamReader reader = new RebufferableStreamReader(ms);
                string line = reader.ReadLine();
                string nextLine = reader.ReadLine();

                Assert.AreEqual(line, "ni hao wo shi zhanghao");
                Assert.AreEqual(nextLine, "this is test!");
            }
        }

        [TestMethod]
        public void ReadLineWhenLineInBuffer() 
        {
            using (var ms = new MemoryStream(Encoding.Default.GetBytes(readerTestString)))
            {
                RebufferableStreamReader reader = new RebufferableStreamReader(ms);
                reader.Rebuffer(Encoding.Default.GetBytes(lineBufferTestString));

                var line = reader.ReadLine();
                var secondLine = reader.ReadLine();
                var thirdLine = reader.ReadLine();

                Assert.AreEqual(line, "hahaha");
                Assert.AreEqual(secondLine, "hehe" + "ni hao wo shi zhanghao");
                Assert.AreEqual(thirdLine, "this is test!");

                reader.Rebuffer(Encoding.Default.GetBytes("hello world"));
                var fourLine = reader.ReadLine();
                Assert.AreEqual(fourLine, "hello world");
            }
        }

        public void ReadLineWhenReaderHaveBufferAndNoLineInBuffer() 
        {
            using (var ms = new MemoryStream(Encoding.Default.GetBytes(readerTestString)))
            {
                RebufferableStreamReader reader = new RebufferableStreamReader(ms);
                reader.Rebuffer(Encoding.Default.GetBytes(noLineBufferTestString));

                var line = reader.ReadLine();
                var secondLine = reader.ReadLine();
                var thirdLine = reader.ReadLine();

                Assert.AreEqual(line, noLineBufferTestString + "ni hao wo shi zhanghao");
                Assert.AreEqual(secondLine, "this is test!");
                Assert.AreEqual(thirdLine, string.Empty);
            }
        }
    }
}
