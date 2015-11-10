using Citi.Investobank.Broker.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Citi.Investobank.Shared
{
    [ServiceContract]
    public interface IInvestobankServer
    {
        void RegisterBroker(BrokerDefinition brokerDefinition, string endpointUrl);

        decimal ExecutePurchaseOrder(string clientName, int amount);

        decimal ExecuteSaleOrder(string clientName, int amount);

        ICollection<BrokerReport> GetBrokerDigicoinsTransactedReport();

        ICollection<ClientReport> GetClientNetPositionsReport();
    }
}
