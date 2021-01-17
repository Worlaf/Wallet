using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Data.Domain
{
    public class CurrencyAccount : EntityBase
    {
        public int WalletId { get; set; }
        public Wallet Wallet { get; set; }

        public string CurrencyCode { get; set; }
        public decimal Balance { get; set; }
    }
}
