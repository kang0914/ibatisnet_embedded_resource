using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBatisNet.Biz.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void MM_CODE_SELECT()
        {
            // 작업
            MainService mainService = new MainService();

            // 준비
            var result = mainService.MM_CODE_SELECT();

            // 확인
            Assert.IsTrue(result != null);
        }

        [TestMethod]
        public void Test()
        {
            // 작업
            MainService mainService = new MainService();

            // 준비
            mainService.Test();

            // 확인
            
        }
    }
}
