using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citi.Investobank
{
    public class QuotationResult
    {
        public ICollection<BrokerIdWithAmount> Result { get; set; }

        public decimal TotalAmount { get; set; }

        public bool CanProcess { get; set; }
    }
}
