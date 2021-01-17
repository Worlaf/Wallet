using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wallet.API.Models.Wallet
{
    public class ConvertMoneyRequestModel
    {
        public string DestinationCurrency { get; set; }
        public decimal SourceCurrencyAmount { get; set; }
    }
}
