using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Citi.Investobank.Broker.Shared
{
    [DataContract]
    public class CommissionTreshold
    {
        [DataMember]
        public uint From { get; set; }
        [DataMember]
        public uint To { get; set; }
        [DataMember]
        public decimal CommissionPercent { get; set; }

        public CommissionTreshold(uint from, uint to, decimal commissionPercent)
        {
            if(from >= to)
            {
                throw new ArgumentException("From needs to be less than to");
            }
            this.From = from;
            this.To = to;
            this.CommissionPercent = commissionPercent;
        }
    }
}
