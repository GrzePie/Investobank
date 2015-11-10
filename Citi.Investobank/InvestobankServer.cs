using Citi.Investobank.Broker;
using Citi.Investobank.Broker.Shared;
using Citi.Investobank.QuotationAlgorithm;
using Citi.Investobank.Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Citi.Investobank.BrokerClient;
using Citi.Investobank.InvestobankModel;

namespace Citi.Investobank
{
    public class InvestobankServer : IInvestobankServer
    {
        private IDictionary<string, BrokerDefinition> brokerDefinitionsById;
        private IDictionary<string, IBrokerService> brokerClientsById;
        private IQuotationAlgorithm quotationAlgorithm;
        private Func<string, IBrokerService> brokerClientFactory;
        private IInvestobankTradingModel dataModel;

        // easiest way to have concurrent hash set
        private ConcurrentDictionary<string, byte> clientIds;

        public InvestobankServer(IQuotationAlgorithm quotationAlgorithm, IInvestobankTradingModel dataModel, Func<string, IBrokerService> brokerClientFactory)
        {
            if(quotationAlgorithm == null)
            {
                throw new ArgumentNullException("quotationAlgorithm cannot be null");
            }
            if (dataModel == null)
            {
                throw new ArgumentNullException("dataModel cannot be null");
            }
            if (brokerClientFactory == null)
            {
                throw new ArgumentNullException("brokerClientFactory cannot be null");
            }
            this.quotationAlgorithm = quotationAlgorithm;
            this.brokerClientFactory = brokerClientFactory;
            this.dataModel = dataModel;

            Initialize();
        }

        private void Initialize()
        {
            brokerDefinitionsById = new ConcurrentDictionary<string, BrokerDefinition>();
            brokerClientsById = new ConcurrentDictionary<string, IBrokerService>();
            clientIds = new ConcurrentDictionary<string, byte>();
        }

        public void RegisterBroker(BrokerDefinition brokerDefinition, string endpointUrl)
        {
            if(brokerDefinition == null)
            {
                throw new ArgumentNullException("Broker definition cannot be null");
            }
            this.brokerDefinitionsById[brokerDefinition.BrokerId] = brokerDefinition;
            this.brokerClientsById[brokerDefinition.BrokerId] = this.brokerClientFactory(endpointUrl);
        }

        public decimal ExecutePurchaseOrder(string clientName, int amount)
        {
            if(amount <= 0)
            {
                throw new ArgumentException("Amount must be greater than 0");
            }
            return ExecuteOrder(clientName, TransactionType.Purchase, amount);
        }

        public decimal ExecuteSaleOrder(string clientName, int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be greater than 0");
            }
            return ExecuteOrder(clientName, TransactionType.Sale, amount);
        }

        private decimal ExecuteOrder(string clientName, TransactionType transactionType, int amount)
        {
            if(amount <= 0)
            {
                throw new ArgumentException("Amount must be greater than 0");
            }
            if(amount % 10 != 0)
            {
                throw new ArgumentException("Amount must be a multiply of 10");
            }

            clientIds[clientName] = 0;

            bool isSale = transactionType == TransactionType.Sale;

            QuotationResult plan = quotationAlgorithm.GetQuotation(this.brokerDefinitionsById.Values, transactionType, amount);

            if (isSale)
            {
                dataModel.RegisterSale(clientName, amount, plan);
            }
            else
            {
                dataModel.RegisterPurchase(clientName, amount, plan);
            }

            if (!plan.CanProcess)
            {
                throw new Exception("Cannot process");
            }
            Parallel.ForEach(plan.Result, r =>
            {
                this.brokerClientsById[r.BrokerId].ProcessTransaction(isSale ? -r.Amount : r.Amount);
            });

            return plan.TotalAmount;
        }

        public ICollection<BrokerReport> GetBrokerDigicoinsTransactedReport()
        {
            ICollection<BrokerReport> result = new List<BrokerReport>();
            foreach (string brokerId in this.brokerDefinitionsById.Keys) {
                result.Add(new BrokerReport()
                {
                    Name = brokerId,
                    Amount = dataModel.ProjectBrokerNumberTraded(brokerId)
                });
            }
            return result;
        }

        public ICollection<ClientReport> GetClientNetPositionsReport()
        {
            ICollection<ClientReport> result = new List<ClientReport>();
            foreach (string clientId in this.clientIds.Keys)
            {
                result.Add(new ClientReport()
                {
                    Name = clientId,
                    Amount = dataModel.ProjectClientNetAmount(clientId)
                });
            }
            return result;
        }
    }
}
