using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Wallet.Common;
using Wallet.Data.Domain;
using Wallet.Data.Infrastructure;
using Wallet.Data.Repositories;
using Wallet.Services.Providers;

namespace Wallet.Services.Handlers
{
    public class CreateWalletRequest : IRequest<int>
    {
        public int UserId { get; }
        public IEnumerable<string> AvailableCurrencyCodes { get; }

        public CreateWalletRequest(int userId, IEnumerable<string> availableCurrencyCodes)
        {
            UserId = userId;
            AvailableCurrencyCodes = availableCurrencyCodes;
        }
    }

    public class CreateWalletHandler : IRequestHandler<CreateWalletRequest, int>
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ICurrencyAccountRepository _currencyAccountRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateWalletHandler(IWalletRepository walletRepository, IUnitOfWork unitOfWork, ICurrencyAccountRepository currencyAccountRepository)
        {
            _walletRepository = walletRepository;
            _unitOfWork = unitOfWork;
            _currencyAccountRepository = currencyAccountRepository;
        }

        public async Task<int> Handle(CreateWalletRequest request, CancellationToken cancellationToken)
        {
            var notSupportedCurrencyCodes = request.AvailableCurrencyCodes.Where(c => !ConversionRateProvider.SupportedCurrencyCodes.Contains(c)).ToArray();
            if (notSupportedCurrencyCodes.Any())
                throw new BusinessException($"Не удалось создать кошелек. Невозможно создать кошелек со счетами для валют: {notSupportedCurrencyCodes.ToCommaSeparated()}.");

            var wallet = new Data.Domain.Wallet
            {
                UserId = request.UserId
            };

            _walletRepository.Save(wallet);
            await _unitOfWork.CommitAsync(cancellationToken);

            var currencyAccounts = request.AvailableCurrencyCodes.Select(c => new CurrencyAccount
            {
                CurrencyCode = c,
                Balance = 0,
                WalletId = wallet.Id
            });

            foreach (var currencyAccount in currencyAccounts)
            {
                _currencyAccountRepository.Save(currencyAccount);
            }

            await _unitOfWork.CommitAsync(cancellationToken);

            return wallet.Id;
        }
    }
}