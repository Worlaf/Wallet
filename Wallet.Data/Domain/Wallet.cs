using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Data.Domain
{
    public class Wallet : EntityBase
    {
        public int UserId { get; set; }

        public ICollection<CurrencyAccount> CurrencyAccounts { get; set; }
    }
}
