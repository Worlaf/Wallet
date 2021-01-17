using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wallet.API.Models.Wallet
{
    public class GetWalletResponseModel
    {
        /// <summary>
        /// Счета для различных валют
        /// </summary>
        public IEnumerable<CurrencyAccountModel> CurrencyAccounts { get; set; }
    }

    public class CurrencyAccountModel
    {
        /// <summary>
        /// Валюта счёта
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Баланс
        /// </summary>
        public decimal Balance { get; set; }
    }
}
