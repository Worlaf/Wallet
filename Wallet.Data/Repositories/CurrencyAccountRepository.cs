using System;
using System.Collections.Generic;
using System.Text;
using Wallet.Data.Contexts;
using Wallet.Data.Domain;

namespace Wallet.Data.Repositories
{
    public interface ICurrencyAccountRepository : IEntityRepository<CurrencyAccount>
    {
    }

    public class CurrencyAccountRepository : EntityRepositoryBase<CurrencyAccount>, ICurrencyAccountRepository
    {
        public CurrencyAccountRepository(WalletDbContext dbContext) : base(dbContext)
        {
        }
    }
}
