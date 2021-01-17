using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wallet.API.Models.Wallet
{
    public class GetWalletResponseModel
    {
        public IEnumerable<CurrencyAccountModel> CurrencyAccounts { get; set; }
    }

    public class CurrencyAccountModel
    {
        public string CurrencyCode { get; set; }
        public decimal Balance { get; set; }
    }
}
