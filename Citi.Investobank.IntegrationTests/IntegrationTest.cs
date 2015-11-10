using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using Citi.Investobank.Broker.Shared;
using Citi.Investobank.Broker;
using Citi.Investobank.QuotationAlgorithm;
using Citi.Investobank.BrokerClient;
using Citi.Investobank.InvestobankModel;

namespace Citi.Investobank.IntegrationTests
{
    [TestClass]
    public class IntegrationTest
    {
        private static ServiceHost broker1Host;
        private static ServiceHost broker2Host;

        private static InvestobankServer server;

        [ClassInitialize]
        public static void CreateSUT(TestContext context)
        {
            var broker1Definition = new BrokerDefinition("Broker 1", 1.49m, new CommissionTreshold(0, 100, 5m));
            var broker2Definition = new BrokerDefinition("Broker 2", 1.52m,
                    new CommissionTreshold(0, 40, 3m),
                    new CommissionTreshold(50, 80, 2.5m),
                    new CommissionTreshold(90, 100, 2m));

            string baseUrl = "http://localhost:8733/Design_Time_Addresses/";

            var broker1 = new BrokerService(broker1Definition);
            string broker1Url = baseUrl + "broker1";

            broker1Host = new ServiceHost(broker1, new Uri(broker1Url));
            PrepareServiceHost(broker1Host);
            broker1Host.Open();

            var broker2 = new BrokerService(broker1Definition);
            string broker2Url = baseUrl + "broker2";

            broker2Host = new ServiceHost(broker2, new Uri(broker2Url));
            PrepareServiceHost(broker2Host);
            broker2Host.Open();

            server = new InvestobankServer(new TheBestQuotationAlgorithm(), new InvestobankTradingModelInMemory(), (string a) => new BrokerWcfClient(a));
            server.RegisterBroker(broker1Definition, broker1Url);
            server.RegisterBroker(broker2Definition, broker2Url);
        }

        private static void PrepareServiceHost(ServiceHost host)
        {
            var behaviour = host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            behaviour.InstanceContextMode = InstanceContextMode.Single;
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            broker1Host.Close();
            broker2Host.Close();
        }

        [TestMethod]
        public void Purchase_A_10()
        {
            decimal result = server.ExecutePurchaseOrder("A", 10);
            Assert.AreEqual(result, 15.645m);
        }

        [TestMethod]
        public void Purchase_B_40()
        {
            decimal result = server.ExecutePurchaseOrder("B", 40);
            Assert.AreEqual(result, 62.58m);
        }

        [TestMethod]
        public void Purchase_A_50()
        {
            decimal result = server.ExecutePurchaseOrder("A", 50);
            Assert.AreEqual(result, 77.9m);
        }

        [TestMethod]
        public void Purchase_B_100()
        {
            decimal result = server.ExecutePurchaseOrder("B", 100);
            Assert.AreEqual(result, 155.04m);
        }

        [TestMethod]
        public void Sell_B_80()
        {
            decimal result = server.ExecuteSaleOrder("B", 80);
            Assert.AreEqual(result, 124.64m);
        }

        [TestMethod]
        public void Sell_C_70()
        {
            decimal result = server.ExecuteSaleOrder("C", 70);
            Assert.AreEqual(result, 109.06m);
        }

        [TestMethod]
        public void Purchase_A_130()
        {
            decimal result = server.ExecutePurchaseOrder("A", 130);
            Assert.AreEqual(result, 201.975m);
        }

        [TestMethod]
        public void Sell_B_60()
        {
            decimal result = server.ExecuteSaleOrder("B", 60);
            Assert.AreEqual(result, 93.48m);
        }

        [TestMethod]
        public void GetClientNetPositionsReport_CorrectValue()
        {
            var result = server.GetClientNetPositionsReport();
            var resultsByName = result.OrderBy(r => r.Name).ToList();
            Assert.AreEqual(296.156m, resultsByName[0].Amount);
            Assert.AreEqual(0m, resultsByName[1].Amount);
            Assert.AreEqual(-109.06m, resultsByName[2].Amount);
        }

        [TestMethod]
        public void GetBrokerNumberOfDigicoinsTransacted_CorrectValue()
        {
            var result = server.GetBrokerDigicoinsTransactedReport();
            var resultsByName = result.OrderBy(r => r.Name).ToList();
            Assert.AreEqual(80, resultsByName[0].Amount);
            Assert.AreEqual(460, resultsByName[1].Amount);
        }
    }
}
