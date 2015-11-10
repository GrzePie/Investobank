using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Citi.Investobank.Broker.Shared
{
    [DataContract]
    public class BrokerDefinition
    {
        [DataMember]
        public CommissionTreshold[] Tresholds { get; private set; }
        [DataMember]
        public decimal PricePerDigicoin { get; private set; }
        [DataMember]
        public string BrokerId { get; private set; }

        public BrokerDefinition(string brokerId, decimal pricePerDigicoin, params CommissionTreshold[] tresholds)
        {
            tresholds = tresholds.OrderBy(t => t.From).ToArray();
            if(tresholds.Length == 0)
            {
                throw new ArgumentException("Specify at least one treshold");
            }
            if (tresholds[0].From != 0) {
                throw new ArgumentException("First treshold must start at 0");
            }
            for(int i = 1; i < tresholds.Length; i++)
            {
                if(tresholds[i - 1].To >= tresholds[i].From)
                {
                    throw new ArgumentException("Tresholds must not be overlapping");
                }
            }
            if(pricePerDigicoin <= 0)
            {
                throw new ArgumentOutOfRangeException("PricePerDigicoin must be of positive value");
            }
            if (string.IsNullOrWhiteSpace(brokerId))
            {
                throw new ArgumentException("Broker id cannot be empty");
            }
            this.Tresholds = tresholds;
            this.PricePerDigicoin = pricePerDigicoin;
            this.BrokerId = brokerId;
        }
    }
}
