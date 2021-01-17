using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wallet.API.Models.Wallet
{
    public class ConvertMoneyRequestModel
    {
        /// <summary>
        /// Код валюты - на счёт, привязанный к этой валюте будут переведены деньги
        /// </summary>
        public string DestinationCurrencyCode { get; set; }

        /// <summary>
        /// Сумма в исходной валюте
        /// </summary>
        public decimal SourceCurrencyAmount { get; set; }
    }
}
