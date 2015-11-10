using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Citi.Investobank.Broker.Shared
{
    [ServiceContract]
    public interface IBrokerService
    {
        [OperationContract]
        void ProcessTransaction(decimal amount);
    }
}
