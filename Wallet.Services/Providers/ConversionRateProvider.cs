using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Wallet.Common;

namespace Wallet.Services.Providers
{
    public interface IConversionRateProvider
    {
        Task<decimal> GetConversionRateAsync(string sourceCurrencyCode, string destinationCurrencyCode);
    }

    public class ConversionRateProvider : IConversionRateProvider
    {
        public static string[] SupportedCurrencyCodes = {
            "RUB", "AUD", "AZN", "GBP", "AMD", "BYN", "BGN", "BRL", "HUF", "HKD", "DKK", "USD", "EUR", "INR", "KZT", "CAD", "KGS", "CNY", "MDL", "NOK", "PLN", "RON", "XDR", "SGD", "TJS", "TRY", "TMT", "UZS", "UAH", "CZK", "SEK", "CHF",
            "ZAR", "KRW", "JPY"
        };

        private const string ProviderUrl = @"https://www.cbr-xml-daily.ru/daily_json.js";

        private readonly IHttpClientFactory _httpClientFactory;

        public ConversionRateProvider(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<decimal> GetConversionRateAsync(string sourceCurrencyCode, string destinationCurrencyCode)
        {
            if (!SupportedCurrencyCodes.Contains(sourceCurrencyCode))
                throw new InvalidOperationException($"Неподдерживаемое значение {nameof(sourceCurrencyCode)}: {sourceCurrencyCode}.");

            if (!SupportedCurrencyCodes.Contains(destinationCurrencyCode))
                throw new InvalidOperationException($"Неподдерживаемое значение {nameof(destinationCurrencyCode)}: {destinationCurrencyCode}.");

            using var client = _httpClientFactory.CreateClient();
            
            var response = await client.GetStringAsync(ProviderUrl);
            var responseModel = response.FromJson<ProviderResponseModel>();

            const string baseCurrencyCode = "RUB";
            decimal sourceCurrencyToBaseCurrencyConversionRate = sourceCurrencyCode == baseCurrencyCode ? 1 : responseModel.Valute[sourceCurrencyCode].Value / responseModel.Valute[sourceCurrencyCode].Nominal;
            decimal destinationCurrencyToBaseCurrencyConversionRate = destinationCurrencyCode == baseCurrencyCode ? 1 : responseModel.Valute[destinationCurrencyCode].Value / responseModel.Valute[destinationCurrencyCode].Nominal;

            return sourceCurrencyToBaseCurrencyConversionRate / destinationCurrencyToBaseCurrencyConversionRate;
        }

        public class ProviderResponseModel
        {
            public Dictionary<string, CurrencyModel> Valute { get; set; }

            public class CurrencyModel
            {
                public string CharCode { get; set; }
                public int Nominal { get; set; }
                public decimal Value { get; set; }
            }
        }
    }
}