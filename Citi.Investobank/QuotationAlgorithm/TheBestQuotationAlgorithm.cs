using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Citi.Investobank.Broker.Shared;
using Citi.Investobank.Shared;

namespace Citi.Investobank.QuotationAlgorithm
{
    public class TheBestQuotationAlgorithm : IQuotationAlgorithm
    {
        public QuotationResult GetQuotation(ICollection<BrokerDefinition> brokerDefinitions, TransactionType transactionType, int amount)
        {
            if(brokerDefinitions == null)
            {
                throw new ArgumentNullException("Broker Definitions cannot be null");
            }
            if(amount <= 0)
            {
                throw new ArgumentOutOfRangeException("Amount must be greater than 0");
            }

            ICollection<BrokerIdWithAmount> result = new List<BrokerIdWithAmount>();
            decimal calculatedAmount = 0m;
            int processedAmount = 0;
            List<TresholdWithBroker> tresholds = brokerDefinitions.SelectMany(
                    broker => broker.Tresholds.Select(treshold => 
                            new TresholdWithBroker(treshold, broker))
                ).ToList();

            // other way around for sales? See readme
            tresholds = tresholds.OrderBy(t => t.CalculateRatePerCoin(transactionType)).ToList();

            while (processedAmount < amount)
            {
                int amountLeft = amount - processedAmount;

                var treshold = tresholds.FirstOrDefault(t => t.Treshold.From <= amountLeft);
                if(treshold == null)
                {
                    return new QuotationResult
                    {
                        CanProcess = false
                    };
                }

                int amountInThisTreshold = (int)Math.Min(amountLeft, treshold.Treshold.To);

                result.Add(new BrokerIdWithAmount()
                {
                    Amount = amountInThisTreshold,
                    BrokerId = treshold.Broker.BrokerId
                });
                processedAmount += amountInThisTreshold;
                calculatedAmount += amountInThisTreshold * treshold.CalculateRatePerCoin(transactionType);

                // in case given broker is maxed out
                tresholds = tresholds.Where(t => t.Broker != treshold.Broker).ToList();
            }

            return new QuotationResult() {
                Result = result,
                TotalAmount = calculatedAmount,
                CanProcess = true
            };
        }

        private class TresholdWithBroker
        {
            public readonly CommissionTreshold Treshold;
            public readonly BrokerDefinition Broker;

            public TresholdWithBroker(CommissionTreshold treshold, BrokerDefinition broker)
            {
                this.Treshold = treshold;
                this.Broker = broker;
            }

            public decimal CalculateRatePerCoin(TransactionType transactionType)
            {
                // satisfies test cases, but is that correct? See readme
                return (1 + Treshold.CommissionPercent / 100) * Broker.PricePerDigicoin;
            }
        }
    }
}
