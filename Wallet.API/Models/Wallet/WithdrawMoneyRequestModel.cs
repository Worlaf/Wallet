using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wallet.API.Models.Wallet
{
    public class WithdrawMoneyRequestModel
    {
        /// <summary>
        /// Сумма в валюте целевого счета
        /// </summary>
        public decimal Amount { get; set; }
    }
}
