using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citi.Investobank.InvestobankModel
{
    internal class TransactionData
    {
        public decimal RequestedAmount { get; set; }

        public QuotationResult QuotationResult { get; set; }

        public DateTime EventDate { get; set; }

        public bool IsSale { get; set; }
    }
}
