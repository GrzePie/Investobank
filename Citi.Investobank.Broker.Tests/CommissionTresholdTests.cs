using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Citi.Investobank.Broker.Shared;

namespace Citi.Investobank.Broker.Tests
{
    [TestClass]
    public class CommissionTresholdTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_InvalidFromTo_Test()
        {
            var commissionTreshold = new CommissionTreshold(10, 4, 10);
        }
    }
}
