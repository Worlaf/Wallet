using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Wallet.Data.Domain;

namespace Wallet.Data.Contexts
{
    public class WalletDbContext : DbContext
    {
        public const string ConnectionStringName = "WalletSqlServerConnectionString";

        public DbSet<Domain.Wallet> Wallets { get; set; }
        public DbSet<Domain.CurrencyAccount> CurrencyAccounts { get; set; }

        public WalletDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CurrencyAccount>().HasIndex(ca => new {ca.WalletId, ca.CurrencyCode}).IsUnique();
            modelBuilder.Entity<Domain.Wallet>().HasIndex(w => w.UserId).IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}