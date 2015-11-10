using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Citi.Investobank.InvestobankModel;

namespace Citi.Investobank.Tests
{
    [TestClass]
    public class InvestobankTradingModelInMemoryTests
    {
        [TestMethod]
        public void ProjectBrokerNumberTraded_Test()
        {
            InvestobankTradingModelInMemory model = new InvestobankTradingModelInMemory();
            model.RegisterPurchase("A", 100, new QuotationResult()
            {
                CanProcess = true,
                TotalAmount = 110,
                Result = new BrokerIdWithAmount[]
                {
                    new BrokerIdWithAmount() { BrokerId = "A", Amount = 100  }
                }
            });
            model.RegisterPurchase("A", 50, new QuotationResult()
            {
                CanProcess = true,
                TotalAmount = 110,
                Result = new BrokerIdWithAmount[]
                {
                    new BrokerIdWithAmount() { BrokerId = "A", Amount = 50  }
                }
            });

            var result = model.ProjectBrokerNumberTraded("A");
            Assert.AreEqual(150, result);
        }
    }
}
