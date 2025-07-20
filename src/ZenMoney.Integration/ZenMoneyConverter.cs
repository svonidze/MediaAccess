namespace ZenMoney.Integration;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Common.Serialization.Json;
using Common.System;
using Common.Text;

using FreedomFinanceBank.Contracts;

using ModulDengi.Contracts;
using ModulDengi.Core;
using ModulDengi.Integration.Contracts.Responses;

using FreedomFinanceTransaction = FreedomFinanceBank.Contracts.Transaction;
using ZenMoneyTransaction = ZenMoney.Integration.Contracts.Types.Transaction;

public static class ZenMoneyConverter
{
    private const string DateFormat = "dd.MM.yyyy";

    public static IEnumerable<ZenMoneyTransaction> ConvertToTransactions(
        IEnumerable<AccountStatementResponse> accountStatements) =>
        accountStatements
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

                        var transaction = new Contracts.Types.Transaction
                            {
                                Category = Constants.ZeroValue,
                                Comment = comment.ToString(),
                                Date = statement.CreatedAt.ToString(DateFormat),
                                Payee = description.BorrowerName.Replace("\"", null)
                            };

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
                                transaction.Outcome = transaction.Income = _GetFormattedAmount(statement.Amount);
                                return transaction;
                            case AccountStatementType.Fund:
                                if (statement.Amount > 0)
                                {
                                    throw new NotSupportedException(
                                        $"Not expected that Amount is positive: {new { statement.Amount, statement.Type }.ToJson()}");
                                }

                                transaction.AccountIncome = Constants.Wallets.Долг;
                                transaction.AccountOutcome = Constants.Wallets.МодульДеньгиRub;
                                transaction.Outcome = transaction.Income = _GetFormattedAmount(statement.Amount);
                                return transaction;
                            case AccountStatementType.CessionCommission:
                                transaction.TagGroups =
                                    [
                                        Constants.Categories.Комиссия
                                    ];
                                break;
                            case AccountStatementType.RefundPercentage:
                            case AccountStatementType.RefundPenaltyPercentage:
                                transaction.TagGroups =
                                    [
                                        Constants.Categories.ДолгПроценты
                                    ];
                                break;
                            default:
                                throw new NotSupportedException(statement.Type);
                        }

                        transaction.AccountIncome = transaction.AccountOutcome = Constants.Wallets.МодульДеньгиRub;

                        if (statement.Amount < 0)
                        {
                            transaction.Outcome = _GetFormattedAmount(statement.Amount);
                            transaction.Income = Constants.ZeroStringValue;
                        }
                        else
                        {
                            transaction.Income = _GetFormattedAmount(statement.Amount);
                            transaction.Outcome = Constants.ZeroStringValue;
                        }

                        return transaction;
                    });

    public static IEnumerable<ZenMoneyTransaction> ConvertToTransactions(
        IEnumerable<FreedomFinanceTransaction> transactions)
    {
        foreach (var sourceTransaction in transactions)
        {
            var transaction = new ZenMoneyTransaction
                {
                    Category = Constants.ZeroValue,
                    Comment = sourceTransaction.Description,
                    Date = sourceTransaction.CreatedAt.ToString(DateFormat),
                    Payee = sourceTransaction.Payee,
                };
            var account = sourceTransaction.Currency switch
                {
                    "EUR" => Constants.Wallets.FreedomFinanceBank.Eur,
                    "USD" => Constants.Wallets.FreedomFinanceBank.Usd,
                    _ => throw new NotSupportedException(sourceTransaction.Currency)
                };

            var amount = _GetFormattedAmount(sourceTransaction.Amount);
            if (sourceTransaction.Amount < 0)
            {
                transaction.Income = Constants.ZeroStringValue;
                transaction.Outcome = amount;
                transaction.AccountOutcome = account;
            }
            else
            {
                transaction.Income = amount;
                transaction.Outcome = Constants.ZeroStringValue;
                transaction.AccountIncome = account;
            }
                
            yield return transaction;
        }
    }
        
    private static string _GetFormattedAmount(double value) =>
        Math.Abs(value).ToString(CultureInfo.InvariantCulture);
        
    private static string _GetFormattedAmount(decimal value) =>
        Math.Abs(value).ToString(CultureInfo.InvariantCulture);
}