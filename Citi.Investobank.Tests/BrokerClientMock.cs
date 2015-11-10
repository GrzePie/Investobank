using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Citi.Investobank.Broker.Shared;

namespace Citi.Investobank.Tests
{
    class BrokerClientMock : IBrokerService
    {
        public void ProcessTransaction(decimal amount)
        {
        }
    }
}
