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
    public class DepositMoneyRequest : IRequest
    {
        public int CurrencyAccountId { get; }
        public decimal Amount { get; }

        public DepositMoneyRequest(int currencyAccountId, decimal amount)
        {
            CurrencyAccountId = currencyAccountId;
            Amount = amount;
        }
    }

    public class DepositMoneyHandler : IRequestHandler<DepositMoneyRequest>
    {
        private readonly ICurrencyAccountRepository _currencyAccountRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DepositMoneyHandler(ICurrencyAccountRepository currencyAccountRepository, IUnitOfWork unitOfWork)
        {
            _currencyAccountRepository = currencyAccountRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DepositMoneyRequest request, CancellationToken cancellationToken)
        {
            if (request.Amount <= 0)
                throw new BusinessException("Сумма должна быть дольше нуля.");

            var account = await _currencyAccountRepository.GetByIdAsync(request.CurrencyAccountId);
            account.Balance += request.Amount;

            _currencyAccountRepository.Save(account);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Unit.Value;
        }
    }
}