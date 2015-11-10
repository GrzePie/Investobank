using Citi.Investobank.Broker.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citi.Investobank.Broker
{
    public class BrokerService : IBrokerService
    {
        private BrokerDefinition definition;

        public BrokerService(BrokerDefinition definition)
        {
            this.definition = definition;
        }

        public void ProcessTransaction(decimal amount)
        {
            // doing nothing, but could potentially take time or return faults
        }
    }
}
