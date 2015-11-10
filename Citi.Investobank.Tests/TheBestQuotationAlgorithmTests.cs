using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Citi.Investobank.QuotationAlgorithm;
using Citi.Investobank.Broker.Shared;
using System.Collections.Generic;
using System.Linq;
using Citi.Investobank.Shared;

namespace Citi.Investobank.Tests
{
    [TestClass]
    public class TheBestQuotationAlgorithmTests
    {
        private TheBestQuotationAlgorithm CreateAlgorithm()
        {
            return new TheBestQuotationAlgorithm();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetQuotation_ArgumentNullException_BrokerDefinitions()
        {
            var algorithm = CreateAlgorithm();
            algorithm.GetQuotation(null, TransactionType.Purchase, 10);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetQuotation_ArgumentOutOfRangeException_Amount()
        {
            var algorithm = CreateAlgorithm();
            algorithm.GetQuotation(new BrokerDefinition[] { }, TransactionType.Purchase, 0);
        }


        private ICollection<BrokerDefinition> CreateBrokerSet()
        {
            return new BrokerDefinition[]{
                new BrokerDefinition("A", 2, new CommissionTreshold(0, 100, 10)),
                new BrokerDefinition("B", 1, 
                    new CommissionTreshold(0, 10, 55),
                    new CommissionTreshold(20, 100, 50)
                    ),
                new BrokerDefinition("C", 3, new CommissionTreshold(0, 50, 10)),
            };
        }

        [TestMethod]
        public void GetQuotation_CorrectResult_OneBroker()
        {
            var brokers = CreateBrokerSet();
            var algorithm = CreateAlgorithm();

            QuotationResult result = algorithm.GetQuotation(brokers, TransactionType.Purchase, 5);
            Assert.AreEqual(result.TotalAmount, 5m * 1m * 1.55m);
            Assert.AreEqual(result.CanProcess, true);
        }

        [TestMethod]
        public void GetQuotation_CorrectResult_CombinedBrokers()
        {
            var brokers = CreateBrokerSet();
            var algorithm = CreateAlgorithm();

            QuotationResult result = algorithm.GetQuotation(brokers, TransactionType.Purchase, 220);
            var results = result.Result.OrderBy(r => r.BrokerId).ThenByDescending(r => r.Amount).ToArray();
            Assert.AreEqual(results[0].Amount, 100m);
            Assert.AreEqual(results[0].BrokerId, "A");
            Assert.AreEqual(results[1].Amount, 100m);
            Assert.AreEqual(results[1].BrokerId, "B");
            Assert.AreEqual(results[2].Amount, 20m);
            Assert.AreEqual(results[2].BrokerId, "C");
            Assert.AreEqual(result.TotalAmount, 150m + 220m + 66m);
        }

        [TestMethod]
        public void GetQuotation_CannotProcess_ValueTooBig()
        {
            var brokers = CreateBrokerSet();
            var algorithm = CreateAlgorithm();

            QuotationResult result = algorithm.GetQuotation(brokers, TransactionType.Purchase, 300);
            Assert.AreEqual(result.CanProcess, false);
        }
    }
}
