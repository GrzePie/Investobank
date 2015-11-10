using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citi.Investobank.InvestobankModel
{
    public interface IInvestobankTradingModel
    {
        void RegisterSale(string clientId, int amount, QuotationResult result);
        void RegisterPurchase(string clientId, int amount, QuotationResult result);

        decimal ProjectClientNetAmount(string clientId);
        int ProjectBrokerNumberTraded(string brokerId);
    }
}
