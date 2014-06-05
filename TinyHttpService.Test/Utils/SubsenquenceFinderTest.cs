using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyHttpService.Utils;

namespace TinyHttpService.Test.Utils
{
    [TestClass]
    public class SubsenquenceFinderTest
    {
        [TestMethod]
        public void ReturnExpectPosWhenSubsenquenceFound()
        {
            var sub = new byte[] { 15, 20, 22, 23 };
            var sub1 = new byte[] { 11, 20, 22, 23, 15, 15, 20, 22, 23, 222, 111, 100 };
            var full = new byte[] { 11, 20, 22, 23, 15, 15, 20, 22, 23, 222, 111, 100 };

            int pos = SubsenquenceFinder.Search(full, sub);
            int pos1 = SubsenquenceFinder.Search(full, sub1);
            Assert.AreEqual(pos, 5);
            Assert.AreEqual(pos1, 0);
        }

        [TestMethod]
        public void ReturnNegativeOneWhenSubsenquenceNotFound() 
        {
            var sub = new byte[] { 15, 20, 22, 23 };
            var sub1 = new byte[] { 11, 20, 22, 23, 15, 15, 21, 22, 23, 222, 111, 100, 2 };
            var full = new byte[] { 11, 20, 22, 23, 15, 15, 21, 22, 23, 222, 111, 100 };

            int pos = SubsenquenceFinder.Search(full, sub);
            int pos1 = SubsenquenceFinder.Search(full, sub1);
            Assert.AreEqual(pos, -1);
            Assert.AreEqual(pos, -1);
        }
    }
}
