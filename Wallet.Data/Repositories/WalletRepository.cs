using System;
using System.Collections.Generic;
using System.Text;
using Wallet.Data.Contexts;

namespace Wallet.Data.Repositories
{
    public interface IWalletRepository : IEntityRepository<Domain.Wallet>
    {
    }

    public class WalletRepository: EntityRepositoryBase<Domain.Wallet>, IWalletRepository
    {
        public WalletRepository(WalletDbContext dbContext) : base(dbContext)
        {
        }
    }
}
