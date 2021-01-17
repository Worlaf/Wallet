# Тестовое задание ASP.NET:

Необходимо создать WebApi ASP.Net Core приложение, реализующие функции
кошелька пользователя.
1. Пользователь задается идентификатором (произвольного вида на выбор
разработчика)
2. Пользователю доступен кошелек, в котором он может хранить деньги в различных
валютах. Одновременно может быть несколько валют. Например:
- Пользователь1: 10000 RUB, 100 USD, 200 EUR
- Пользователь2: 300000 IDR
3. Пользователь должен иметь возможность выполнять следующие операции через API:
- Пополнить кошелек в одной из валют
- Снять деньги в одной из валют
- Перевести деньги из одной валюты в другую
- Получить состояние своего кошелька (сумму денег в каждой из валют)
4. Актуальный курс валют должен получаться с публичного API с возможностью
замены. Например, https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml
Примечания:
- Аутентификация пользователя не требуется
- UI не требуется
- Необходимо, чтобы код был промышленного качества
- При возникновении вопросов по поводу задания, можно сделать допущение
самостоятельно, описав в сопроводительном документе

# Запуск:
Для запуска необходим MS Sql Server, строка подключения задается в appsettings.json (https://github.com/Worlaf/Wallet/blob/master/Wallet.API/appsettings.Development.json#L10).

При запуске приложения создается база данных с пустыми таблицами, для создания контента можно использовать непосредственно Api (для удобства добавлен SwaggerGen).
