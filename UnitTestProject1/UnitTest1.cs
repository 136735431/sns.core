using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jinher.AMP.SNS.Service.BE.Neighbor;
using Jinher.AMP.SNS.IService.Store;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            NeighborStore.CreateInstance().Init();
        }
    }
}
