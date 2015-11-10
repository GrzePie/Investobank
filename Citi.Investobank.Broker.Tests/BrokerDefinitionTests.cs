using Citi.Investobank.Broker.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citi.Investobank.Broker.Tests
{
    [TestClass]
    public class BrokerDefinitionTests
    {
        private BrokerDefinition CreateBroker()
        {
            return new BrokerDefinition(
                "test",
                1.5m,
                new CommissionTreshold(0, 10, 5),
                new CommissionTreshold(10, 20, 4),
                new CommissionTreshold(20, 55, 3),
                new CommissionTreshold(55, 100, 2)
            );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_ArgumentException_NoTresholds()
        {
            new BrokerDefinition("test", 1.5m);
        }

        [TestMethod]
        public void Constructor_CorrectTresholds()
        {
            new BrokerDefinition(
                    "test",
                    1.5m,
                    new CommissionTreshold(0, 20, 10),
                    new CommissionTreshold(30, 50, 10)
                );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_ArgumentException_TresholdsDontStartFrom0()
        {
            new BrokerDefinition(
                    "test",
                    1.5m,
                    new CommissionTreshold(2, 10, 10),
                    new CommissionTreshold(10, 20, 10)
                );
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_ArgumentException_TresholdsOvelapping()
        {
            new BrokerDefinition(
                    "test",
                    1.5m,
                    new CommissionTreshold(2, 10, 10),
                    new CommissionTreshold(10, 20, 10),
                    new CommissionTreshold(18, 10, 10)
                );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_ArgumentException_PriceNegative()
        {
            new BrokerDefinition(
                    "test",
                    -1.5m,
                    new CommissionTreshold(0, 10, 10),
                    new CommissionTreshold(20, 30, 10)
                );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_ArgumentException_NameEmpty()
        {
            new BrokerDefinition(
                    string.Empty,
                    1.5m,
                    new CommissionTreshold(0, 10, 10),
                    new CommissionTreshold(10, 20, 10)
                );
        }


        //[TestMethod]
        //[ExpectedException(typeof(ArgumentOutOfRangeException))]
        //public void Calculate_ArgumentOutOfRangeException_AmountNotMultiply10()
        //{
        //    var broker = CreateBroker();
        //    broker.ProcessRequest(12);
        //}

        //[TestMethod]
        //public void Calculate_CannotProcess_ValueTooBig()
        //{
        //    var broker = CreateBroker();
        //    var response = broker.ProcessRequest(120);
        //    Assert.AreEqual(response.CanProcess, false);
        //}

        //[TestMethod]
        //public void Calculate_Commission()
        //{
        //    var broker = CreateBroker();
        //    var response = broker.ProcessRequest(30);
        //    Assert.AreEqual(response.CanProcess, true);
        //    Assert.AreEqual(response.Commission, (decimal)3.0 * 30 / 100);
        //}

    }
}
