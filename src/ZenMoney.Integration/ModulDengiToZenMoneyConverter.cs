namespace ZenMoney.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    using Common.Serialization.Json;
    using Common.System;
    using Common.Text;

    using ModulDengi.Contracts;
    using ModulDengi.Core;
    using ModulDengi.Integration.Contracts.Responses;

    using ZenMoney.Integration.Contracts.Types;

    public static class ModulDengiToZenMoneyConverter
    {
        public static IEnumerable<string> ConvertToJsFetchRequest(IEnumerable<AccountStatementResponse> accountStatements)
        {
            var transactions = accountStatements
                .Select(
                    statement => new
                        {
                            statement,
                            StatementType = statement.Type.StringToEnum<AccountStatementType>()
                        })
                .Where(pair => !Constants.IgnoringAccountStatementType.Contains(pair.StatementType))
                .Select(
                    pair =>
                    {
                        var statement = pair.statement;
                        var description = DescriptionExtractor.Extract(statement.Description);
                        if (description.BorrowerName == null)
                        {
                            throw new NotSupportedException(
                                $"{nameof(Description.BorrowerName)} must be provided for {statement.Type}");
                        }

                        var comment = new StringBuilder().Append($"{statement.Title}: проект {description.ProjectId}")
                            .AppendIf(description.TimePeriodInDays.HasValue, $" срок {description.TimePeriodInDays} д.")
                            .AppendIf(description.Percent.HasValue, $" под {description.Percent}%");

                        var transaction = new Transaction
                            {
                                Category = Constants.ZeroValue,
                                Comment = comment.ToString(),
                                Date = statement.CreatedAt.ToString("dd.MM.yyyy"),
                                Payee = description.BorrowerName.Replace("\"", null)
                            };

                        string GetFormattedAmount() =>
                            Math.Abs(statement.Amount).ToString(CultureInfo.InvariantCulture);

                        var statementType = statement.Type.StringToEnum<AccountStatementType>();
                        switch (statementType)
                        {
                            case AccountStatementType.RefundMain:
                                if (statement.Amount < 0)
                                {
                                    throw new NotSupportedException(
                                        $"Not expected that Amount is negative: {new { statement.Amount, statement.Type }.ToJson()}");
                                }

                                transaction.AccountIncome = Constants.Wallets.МодульДеньгиRub;
                                transaction.AccountOutcome = Constants.Wallets.Долг;
                                transaction.Outcome = transaction.Income = GetFormattedAmount();
                                return transaction;
                            case AccountStatementType.Fund:
                                if (statement.Amount > 0)
                                {
                                    throw new NotSupportedException(
                                        $"Not expected that Amount is positive: {new { statement.Amount, statement.Type }.ToJson()}");
                                }

                                transaction.AccountIncome = Constants.Wallets.Долг;
                                transaction.AccountOutcome = Constants.Wallets.МодульДеньгиRub;
                                transaction.Outcome = transaction.Income = GetFormattedAmount();
                                return transaction;
                            case AccountStatementType.CessionCommission:
                                transaction.TagGroups = new[]
                                    {
                                        Constants.Categories.Комиссия
                                    };
                                break;
                            case AccountStatementType.RefundPercentage:
                            case AccountStatementType.RefundPenaltyPercentage:
                                transaction.TagGroups = new[]
                                    {
                                        Constants.Categories.ДолгПроценты
                                    };
                                break;
                            default:
                                throw new NotSupportedException(statement.Type);
                        }

                        transaction.AccountIncome = transaction.AccountOutcome = Constants.Wallets.МодульДеньгиRub;

                        if (statement.Amount < 0)
                        {
                            transaction.Outcome = GetFormattedAmount();
                            transaction.Income = Constants.ZeroValue;
                        }
                        else
                        {
                            transaction.Income = GetFormattedAmount();
                            transaction.Outcome = Constants.ZeroValue;
                        }

                        return transaction;
                    }).ToArray();

            foreach (var transaction in transactions)
            {
                yield return WrapToFetch(
                    new[]
                        {
                            transaction
                        });
            }
        }

        private static string WrapToFetch(Transaction[] transactions)
        {
            var headers = new Dictionary<string, object>
                {
                    { "accept", "*/*" },
                    { "accept-language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7" },
                    { "content-type", "application/x-www-form-urlencoded" },
                    { "sec-ch-ua", "\"Google Chrome\";v=\"87\", \" Not;A Brand\";v=\"99\", \"Chromium\";v=\"87\"" },
                    { "sec-ch-ua-mobile", "?0" },
                    { "sec-fetch-dest", "empty" },
                    { "sec-fetch-mode", "cors" },
                    { "sec-fetch-site", "same-origin" },
                    { "x-requested-with", "XMLHttpRequest" },
                };

            var parameters = new Dictionary<string, object>
                {
                    { "headers", headers },
                    { "referrer", $"{Constants.ZenmoneyUrl}/a/" },
                    { "referrerPolicy", "strict-origin-when-cross-origin" },
                    { "body", transactions.ToJson() },
                    { "method", "PUT" },
                    { "mode", "cors" },
                    { "credentials", "include" },
                };
            return $"fetch(\"{Constants.ZenmoneyApiUrl}\", {parameters.ToJsonIndented()});";
        }
    }
}