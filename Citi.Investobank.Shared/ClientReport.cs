using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Citi.Investobank.Shared
{
    [DataContract]
    public class ClientReport
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public decimal Amount { get; set; }
    }
}
