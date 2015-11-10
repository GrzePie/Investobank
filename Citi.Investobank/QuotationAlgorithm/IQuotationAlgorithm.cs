using Citi.Investobank.Broker.Shared;
using Citi.Investobank.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citi.Investobank.QuotationAlgorithm
{
    public interface IQuotationAlgorithm
    {
        QuotationResult GetQuotation(ICollection<BrokerDefinition> brokerDefinitions, TransactionType transactionType, int amount);
    }
}
