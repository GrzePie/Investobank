using Citi.Investobank.Broker.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Citi.Investobank.BrokerClient
{
    public class BrokerWcfClient : IBrokerService
    {
        private string endpoint;
        private IBrokerService client;

        public BrokerWcfClient(string endpoint)
        {
            this.endpoint = endpoint;
        }

        private void OpenAndExecute(Action<IBrokerService> method)
        {
            var myBinding = new BasicHttpBinding();
            var myEndpoint = new EndpointAddress(endpoint);
            var myChannelFactory = new ChannelFactory<IBrokerService>(myBinding, myEndpoint);

            try
            {
                client = myChannelFactory.CreateChannel();
                method.Invoke(client);
                ((ICommunicationObject)client).Close();
            }
            catch
            {
                if (client != null)
                {
                    ((ICommunicationObject)client).Abort();
                }
            }
        }

        public void ProcessTransaction(decimal amount)
        {
            OpenAndExecute(c => c.ProcessTransaction(amount));
        }
    }
}
