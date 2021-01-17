using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Wallet.API.Models.Wallet;
using Wallet.Data.Repositories;
using Wallet.Services.Handlers;
using Wallet.Services.Providers;

namespace Wallet.API.Controllers
{
    [ApiController]
    [Route("[controller]/{userId}")]
    public class WalletController : ControllerBase
    {
        private readonly ICurrencyAccountRepository _currencyAccountRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IMediator _mediator;

        public WalletController(ICurrencyAccountRepository currencyAccountRepository, IMediator mediator, IWalletRepository walletRepository)
        {
            _currencyAccountRepository = currencyAccountRepository;
            _mediator = mediator;
            _walletRepository = walletRepository;
        }

        /// <summary>
        /// Получить счета пользователя для различных валют
        /// </summary>
        /// <param name="userId">Числовой идентификатор пользователя</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<GetWalletResponseModel>> GetWallet(int userId)
        {
            var wallet = await _walletRepository.SingleOrDefaultAsync(w => w.UserId == userId, w => new GetWalletResponseModel
            {
                CurrencyAccounts = w.CurrencyAccounts.Select(ca => new CurrencyAccountModel {CurrencyCode = ca.CurrencyCode, Balance = ca.Balance})
            });

            if (wallet == null)
                return NotFound();

            return Ok(wallet);
        }

        /// <summary>
        /// Создать кошелек со счетами для всех поддерживаемых валют
        /// </summary>
        /// <param name="userId">Произвольный числовой идентификатор пользователя</param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<ActionResult> CreateWallet(int userId)
        {
            await _mediator.Send(new CreateWalletRequest(userId, ConversionRateProvider.SupportedCurrencyCodes));

            return NoContent();
        }

        /// <summary>
        /// Снять деньги со счёта
        /// </summary>
        /// <param name="userId">Числовой идентификатор пользователя</param>
        /// <param name="sourceCurrencyCode">Код валюты - со счета, привязанного к этой валюте будут сняты деньги</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("{sourceCurrencyCode}/withdraw")]
        public async Task<ActionResult> WithdrawMoney(int userId, string sourceCurrencyCode, WithdrawMoneyRequestModel model)
        {
            var account = await _currencyAccountRepository.SingleOrDefaultAsync(ca => ca.CurrencyCode == sourceCurrencyCode && ca.Wallet.UserId == userId, ca => new {ca.Id});
            if (account == null)
                return NotFound($"Не существует '{sourceCurrencyCode}' счёта для пользователя {userId}.");

            await _mediator.Send(new WithdrawMoneyRequest(account.Id, model.Amount));

            return NoContent();
        }

        /// <summary>
        /// Перевести деньги из одной валюты в другую
        /// </summary>
        /// <param name="userId">Числовой идентификатор пользователя</param>
        /// <param name="sourceCurrencyCode">Код валюты - со счета, привязанного к этой валюте будут сняты деньги</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("{sourceCurrencyCode}/convert")]
        public async Task<ActionResult> ConvertMoney(int userId, string sourceCurrencyCode, ConvertMoneyRequestModel model)
        {
            var sourceAccount = await _currencyAccountRepository.SingleOrDefaultAsync(ca => ca.CurrencyCode == sourceCurrencyCode && ca.Wallet.UserId == userId, ca => new {ca.Id});
            if (sourceAccount == null)
                return NotFound($"Не существует '{sourceCurrencyCode}' счёта для пользователя {userId}.");

            var destinationAccount = await _currencyAccountRepository.SingleOrDefaultAsync(ca => ca.CurrencyCode == model.DestinationCurrencyCode && ca.Wallet.UserId == userId, ca => new {ca.Id});
            if (destinationAccount == null)
                return NotFound($"Не существует '{model.DestinationCurrencyCode}' счёта для пользователя {userId}.");

            await _mediator.Send(new ConvertMoneyRequest(userId, model.SourceCurrencyAmount, sourceAccount.Id, destinationAccount.Id));

            return NoContent();
        }

        /// <summary>
        /// Пополнить счёт
        /// </summary>
        /// <param name="userId">Числовой идентификатор пользователя</param>
        /// <param name="sourceCurrencyCode">Код валюты - на счёт, привязанный к этой валюте будут переведены деньги</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("{sourceCurrencyCode}/deposit")]
        public async Task<ActionResult> DepositMoney(int userId, string sourceCurrencyCode, DepositMoneyRequestModel model)
        {
            var account = await _currencyAccountRepository.SingleOrDefaultAsync(ca => ca.CurrencyCode == sourceCurrencyCode && ca.Wallet.UserId == userId, ca => new {ca.Id});
            if (account == null)
                return NotFound($"Не существует '{sourceCurrencyCode}' счёта для пользователя {userId}.");

            await _mediator.Send(new DepositMoneyRequest(account.Id, model.Amount));

            return NoContent();
        }
    }
}