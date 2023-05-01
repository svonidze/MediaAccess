namespace Kontur.Elba.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Common.Serialization.Json;
    using Common.System;

    using Kontur.Elba.Integration.Contracts;

    using ModulDengi.Contracts;
    using ModulDengi.Integration.Contracts.Responses;

    public static class ModulDengiToElbaConverter
    {
        private static readonly Regex ExtraWhiteSpacesRegex = new("\\s?\\n?\\s{2,}");

        public static IEnumerable<string> ConvertToJsFetchRequest(IEnumerable<AccountStatementResponse> accountStatements)
        {
            var transactions = accountStatements
                .Select(
                    statement => new
                        {
                            statement,
                            StatementType = statement.Type.StringToEnum<AccountStatementType>()
                        })
                .Where(pair => Constants.UsingAccountStatementType.Contains(pair.StatementType))
                .Select(
                    pair =>
                    {
                        var statement = pair.statement;
                        var transaction = new Transaction
                            {
                                TaxScheme = 1,
                                CurrencySymbol = "810",
                                CurrencyNumCode = "810",
                                //DocumentNumber = 10,
                                ContractorName = "ООО \"МОДУЛЬДЕНЬГИ\"",
                                ContractorId = "757affb0-3223-4c6f-9558-501bbb25d6f7",
                                IsNew = true,
                                FormOfMoney = 2,
                                //CbrCurrencyRate = new CbrCurrencyRate(),
                                //OperationTypeData = new OperationTypeData(),
                                DocumentType = "не указан",
                                OperationType = "",
                                LinkedDocumentViews = new object[0],
                                // 
                                Description = ExtraWhiteSpacesRegex.Replace(statement.Description.Replace("\"", "'"), " "),
                                UsnTaxSum = statement.Amount,
                                TaxSum = statement.Amount,
                                Date = statement.CreatedAt.ToString("u"), 
                            };

                        return transaction;
                    }).ToArray();

            foreach (var transaction in transactions)
            {
                yield return WrapToFetch(transaction);
            }
        }

        private static string WrapToFetch(Transaction transaction)
        {
            var headers = new Dictionary<string, object>
                {
                    { "accept", "text/plain, */*; q=0.01" },
                    { "accept-language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7" },
                    { "content-type", "application/json" },
                    { "save-data", "on" },
                    { "x-requested-with", "XMLHttpRequest" },
                };

            var transactionDate = DateTime.Parse(transaction.Date);

            var body = transaction.ToJson().Replace(
                $"\"Date\":\"{transaction.Date}\",",
                $"\"Date\":new Date({transactionDate.Year},{transactionDate.Month - 1},{transactionDate.Day},0,0,0,0),");
            
            var parameters = new Dictionary<string, object>
                {
                    { "headers", headers },
                    { "referrer", $"{Constants.ReferUrl}" },
                    { "referrerPolicy", "strict-origin-when-cross-origin" },
                    { "body", body },
                    { "method", "POST" },
                    { "mode", "cors" },
                    { "credentials", "include" },
                };
            return $"fetch(\"{Constants.Url}\", {parameters.ToJsonIndented()});";
        }
    }
}