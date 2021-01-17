using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Wallet.Common;
using Wallet.Data.Infrastructure;
using Wallet.Data.Repositories;
using Wallet.Services.Providers;

namespace Wallet.Services.Handlers
{
    public class ConvertMoneyRequest : IRequest
    {
        public int UserId { get; }
        public int SourceAccountId { get; }
        public int DestinationAccountId { get; }
        public decimal SourceCurrencyAmount { get; }

        public ConvertMoneyRequest(int userId, decimal sourceCurrencyAmount, int sourceAccountId, int destinationAccountId)
        {
            UserId = userId;
            SourceCurrencyAmount = sourceCurrencyAmount;
            SourceAccountId = sourceAccountId;
            DestinationAccountId = destinationAccountId;
        }
    }

    public class ConvertMoneyHandler : IRequestHandler<ConvertMoneyRequest>
    {
        private readonly ICurrencyAccountRepository _currencyAccountRepository;
        private readonly IConversionRateProvider _conversionRateProvider;
        private readonly IUnitOfWork _unitOfWork;

        public ConvertMoneyHandler(ICurrencyAccountRepository currencyAccountRepository, IConversionRateProvider conversionRateProvider, IUnitOfWork unitOfWork)
        {
            _currencyAccountRepository = currencyAccountRepository;
            _conversionRateProvider = conversionRateProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(ConvertMoneyRequest request, CancellationToken cancellationToken)
        {
            if (request.SourceCurrencyAmount <= 0)
                throw new BusinessException("Сумма должна быть дольше нуля.");

            var sourceAccount = await _currencyAccountRepository.GetByIdAsync(request.SourceAccountId);
            var destinationAccount = await _currencyAccountRepository.GetByIdAsync(request.DestinationAccountId);

            if (sourceAccount.Balance < request.SourceCurrencyAmount)
                throw new BusinessException($"На счете {sourceAccount.CurrencyCode} недостаточно средств.");

            var conversionRate = await _conversionRateProvider.GetConversionRateAsync(sourceAccount.CurrencyCode, destinationAccount.CurrencyCode);
            sourceAccount.Balance -= request.SourceCurrencyAmount;
            destinationAccount.Balance += request.SourceCurrencyAmount * conversionRate;

            _currencyAccountRepository.Save(sourceAccount);
            _currencyAccountRepository.Save(destinationAccount);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Unit.Value;
        }
    }
}