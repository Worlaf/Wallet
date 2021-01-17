using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wallet.API.Models.Wallet
{
    public class DepositMoneyRequestModel
    {
        /// <summary>
        /// Сумма в валюте целевого счёта
        /// </summary>
        public decimal Amount { get; set; }
    }
}
