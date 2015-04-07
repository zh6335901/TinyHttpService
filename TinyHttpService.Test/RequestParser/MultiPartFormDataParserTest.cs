using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyHttpService.RequestParser;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TinyHttpService.Test.RequestParser
{
    [TestClass]
    public class MultiPartFormDataParserTest
    {
        private static readonly string multiParamsAndFilesTestData = "--boundry\r\n" +
              @"Content-Disposition: form-data; name=""text""" + 
              "\r\n\r\ntextdata" + 
              "\r\n--boundry\r\n" +
              @"Content-Disposition: form-data; name=""file""; filename=""first.txt"";" +
              "\r\nContent-Type: text/plain\r\n\r\n" +
              "aaaaaaa" + 
              "\r\n--boundry\r\n" + 
              @"Content-Disposition: form-data; name=""newfile""; filename=""second.txt"";" +
              "\r\nContent-Type: text/plain\r\n\r\n" +
              "bbbbbbb" +
              "\r\n--boundry\r\n" +
              @"Content-Disposition: form-data; name=""username""" +
              "\r\n\r\nzhang" +
              "\r\n--boundry--";

        private static readonly string multiParamsTestData = "--boundry\r\n" +
              @"Content-Disposition: form-data; name=""text""" +
              "\r\n\r\ntextdata" +
              "\r\n--boundry\r\n" +
              @"Content-Disposition: form-data; name=""username""" +
              "\r\n\r\nzhang" +
              "\r\n--boundry--";

        private static readonly string multiFilesTestData = 
              "--boundry\r\n" +
              @"Content-Disposition: form-data; name=""file""; filename=""first.txt"";" +
              "\r\nContent-Type: text/plain\r\n\r\n" +
              "aaaaaaa" +
              "\r\n--boundry\r\n" +
              @"Content-Disposition: form-data; name=""newfile""; filename=""second.txt"";" +
              "\r\nContent-Type: text/plain\r\n\r\n" +
              "bbbbbbb" +
              "\r\n--boundry--";

        [TestMethod]
        public async Task CanParseMultiParamsData() 
        {
            using (MemoryStream ms = new MemoryStream(Encoding.Default.GetBytes(multiParamsTestData)))
            {
                MultiPartFormDataParser parser = new MultiPartFormDataParser(ms, Encoding.Default);
                await parser.ParseAsync();
                Assert.AreEqual(parser.Parameters["text"], "textdata");
                Assert.AreEqual(parser.Parameters["username"], "zhang");
            }
        }

        [TestMethod]
        public async Task CanParseMultiFilesData() 
        {
            using (MemoryStream ms = new MemoryStream(Encoding.Default.GetBytes(multiFilesTestData)))
            {
                MultiPartFormDataParser parser = new MultiPartFormDataParser(ms, Encoding.Default);
                await parser.ParseAsync();
                Assert.AreEqual(parser.Files.Count, 2);
                Assert.AreEqual(parser.Files[0].Filename, "first.txt");
                Assert.AreEqual(parser.Files[0].Name, "file");
                Assert.AreEqual(parser.Files[1].Filename, "second.txt");
                Assert.AreEqual(parser.Files[1].Name, "newfile");

                using (var sw1 = new StreamReader(parser.Files[0].Data))
                using (var sw2 = new StreamReader(parser.Files[1].Data))
                {
                    string file1Str = sw1.ReadToEnd();
                    string file2Str = sw2.ReadToEnd();

                    Assert.AreEqual(file1Str, "aaaaaaa");
                    Assert.AreEqual(file2Str, "bbbbbbb");
                }
            }
        }

        [TestMethod]
        public async Task CanParseMultiParamsAndFilesData()
        {
            using (MemoryStream ms = new MemoryStream(Encoding.Default.GetBytes(multiParamsAndFilesTestData))) 
            {
                MultiPartFormDataParser parser = new MultiPartFormDataParser(ms, Encoding.Default);
                await parser.ParseAsync();
                Assert.AreEqual(parser.Parameters["text"], "textdata");
                Assert.AreEqual(parser.Parameters["username"], "zhang");

                Assert.AreEqual(parser.Files.Count, 2);
                Assert.AreEqual(parser.Files[0].Filename, "first.txt");
                Assert.AreEqual(parser.Files[0].Name, "file");
                Assert.AreEqual(parser.Files[1].Filename, "second.txt");
                Assert.AreEqual(parser.Files[1].Name, "newfile");

                using (var sw1 = new StreamReader(parser.Files[0].Data)) 
                using (var sw2 = new StreamReader(parser.Files[1].Data))
                {
                    string file1Str = sw1.ReadToEnd();
                    string file2Str = sw2.ReadToEnd();

                    Assert.AreEqual(file1Str, "aaaaaaa");
                    Assert.AreEqual(file2Str, "bbbbbbb");
                }
            }
        }
    }
}
