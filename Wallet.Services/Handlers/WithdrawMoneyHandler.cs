using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Wallet.Common;
using Wallet.Data.Infrastructure;
using Wallet.Data.Repositories;

namespace Wallet.Services.Handlers
{
    public class WithdrawMoneyRequest : IRequest
    {
        public int CurrencyAccountId { get; }
        public decimal Amount { get; }

        public WithdrawMoneyRequest(int currencyAccountId, decimal amount)
        {
            CurrencyAccountId = currencyAccountId;
            Amount = amount;
        }
    }

    public class WithdrawMoneyHandler : IRequestHandler<WithdrawMoneyRequest>
    {
        private readonly ICurrencyAccountRepository _currencyAccountRepository;
        private readonly IUnitOfWork _unitOfWork;

        public WithdrawMoneyHandler(ICurrencyAccountRepository currencyAccountRepository, IUnitOfWork unitOfWork)
        {
            _currencyAccountRepository = currencyAccountRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(WithdrawMoneyRequest request, CancellationToken cancellationToken)
        {
            if (request.Amount <= 0)
                throw new BusinessException("Сумма должна быть дольше нуля.");

            var account = await _currencyAccountRepository.GetByIdAsync(request.CurrencyAccountId);

            if (account.Balance < request.Amount)
                throw new BusinessException($"На счете {account.CurrencyCode} недостаточно средств.");

            account.Balance -= request.Amount;

            _currencyAccountRepository.Save(account);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Unit.Value;
        }
    }
}