using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Citi.Investobank.Shared
{
    [DataContract]
    public class BrokerReport
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Amount { get; set; }
    }
}
