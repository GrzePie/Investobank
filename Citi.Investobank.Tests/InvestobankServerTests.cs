using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Citi.Investobank.QuotationAlgorithm;
using Citi.Investobank.Broker.Shared;
using Citi.Investobank.InvestobankModel;

namespace Citi.Investobank.Tests
{
    [TestClass]
    public class InvestobankServerTests
    {
        private InvestobankServer CreateServer()
        {
            var server = new InvestobankServer(new TheBestQuotationAlgorithm(), new InvestobankTradingModelInMemory(), (string a) => new BrokerClientMock());
            return server;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterBroker_ArgumentNullException_WhenArgumentNull()
        {
            InvestobankServer server = CreateServer();
            server.RegisterBroker(null, "");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExecuteOrder_ArgumentException_AmountNotMultiply10()
        {
            InvestobankServer server = CreateServer();
            server.ExecutePurchaseOrder("A", 12);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExecuteOrder_ArgumentException_Amount0()
        {
            InvestobankServer server = CreateServer();
            server.ExecuteSaleOrder("A", 0);
        }

        [TestMethod]
        public void GetClientNetPositionsReport_CorrectValue()
        {
            InvestobankServer server = CreateServer();
            server.RegisterBroker(new BrokerDefinition("A", 1, new CommissionTreshold(0, 100, 10)), string.Empty);
            server.ExecutePurchaseOrder("A", 100);
            server.ExecutePurchaseOrder("B", 100);
            server.ExecuteSaleOrder("A", 50);
            var result = server.GetClientNetPositionsReport();
            result = result.OrderBy(r => r.Name).ToList();
            Assert.AreEqual(result.First().Amount, 110m - 55m);
        }
    }
}
