using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;

namespace Citi.Investobank.InvestobankModel
{
    public class InvestobankTradingModelInMemory : IInvestobankTradingModel
    {
        private readonly Dictionary<string, ConcurrentStack<int>> brokerTransactionsById;
        private readonly Dictionary<string, ConcurrentStack<TransactionData>> clientTransactionsById;
        private readonly ReaderWriterLockSlim brokersLock = new ReaderWriterLockSlim();
        private readonly ReaderWriterLockSlim clientsLock = new ReaderWriterLockSlim();

        public InvestobankTradingModelInMemory() {
            brokerTransactionsById = new Dictionary<string, ConcurrentStack<int>>();
            clientTransactionsById = new Dictionary<string, ConcurrentStack<TransactionData>>();
        }

        private ConcurrentStack<int> GetBrokerTransactions(string brokerId, bool createIfNotExists)
        {
            brokersLock.EnterUpgradeableReadLock();
            try
            {
                if (!brokerTransactionsById.ContainsKey(brokerId))
                {
                    if (createIfNotExists)
                    {
                        var broker = new ConcurrentStack<int>();
                        brokersLock.EnterWriteLock();
                        try
                        {
                            brokerTransactionsById[brokerId] = broker;
                        }
                        finally
                        {
                            brokersLock.ExitWriteLock();
                        }
                    } else
                    {
                        return null;
                    }
                }
                return brokerTransactionsById[brokerId];
            }
            finally
            {
                brokersLock.ExitUpgradeableReadLock();
            }
        }

        private ConcurrentStack<TransactionData> GetClientTransactions(string clientId, bool createIfNotExists)
        {
            clientsLock.EnterUpgradeableReadLock();
            try
            {
                if (!clientTransactionsById.ContainsKey(clientId)) {
                    if (createIfNotExists)
                    {
                        var client = new ConcurrentStack<TransactionData>();
                        clientsLock.EnterWriteLock();
                        try
                        {
                            clientTransactionsById[clientId] = client;
                        }
                        finally
                        {
                            clientsLock.ExitWriteLock();
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                return clientTransactionsById[clientId];
            }
            finally
            {
                clientsLock.ExitUpgradeableReadLock();
            }
        }

        public int ProjectBrokerNumberTraded(string brokerId)
        {
            var broker = GetBrokerTransactions(brokerId, false);
            if (broker == null) return 0;
            return broker.Sum(b => Math.Abs(b));
        }

        public decimal ProjectClientNetAmount(string clientId)
        {
            var client = GetClientTransactions(clientId, false);
            if (clientId == null) return 0m;
            decimal average = client.Average(c => c.QuotationResult.TotalAmount / c.RequestedAmount);
            decimal netAmount = client.Sum(c => (c.IsSale ? -1m : 1m) * c.RequestedAmount * average);
            return Math.Round(netAmount, 3);
        }

        public void RegisterPurchase(string clientId, int amount, QuotationResult result)
        {
            foreach(var quotation in result.Result)
            {
                var broker = GetBrokerTransactions(quotation.BrokerId, true);
                broker.Push(quotation.Amount);
            }
            var client = GetClientTransactions(clientId, true);
            client.Push(new TransactionData
            {
                EventDate = DateTime.Now,
                IsSale = false,
                QuotationResult = result,
                RequestedAmount = amount
            });
        }

        public void RegisterSale(string clientId, int amount, QuotationResult result)
        {
            foreach (var quotation in result.Result)
            {
                var broker = GetBrokerTransactions(quotation.BrokerId, true);
                broker.Push(-quotation.Amount);
            }
            var client = GetClientTransactions(clientId, true);
            client.Push(new TransactionData
            {
                EventDate = DateTime.Now,
                IsSale = true,
                QuotationResult = result,
                RequestedAmount = amount
            });
        }
    }
}
