namespace FreedomFinanceBank.Integration.Tests;

using System.Collections;

using Transaction = FreedomFinanceBank.Contracts.Transaction;

public class TransactionConverterTests
{
    [TestCaseSource(nameof(TestCases))]
    public bool Test(string input, Transaction? expectedTransaction)
    {
        var result = TransactionConverter.TryConvert(input, out Transaction? transaction);

        if (transaction is null && expectedTransaction is null)
        {
            return result;
        }

        Assert.Multiple(
            () =>
                {
                    Assert.That(transaction?.Amount, Is.EqualTo(expectedTransaction?.Amount));
                    Assert.That(transaction?.Currency, Is.EqualTo(expectedTransaction?.Currency));
                    Assert.That(transaction?.CreatedAt, Is.EqualTo(expectedTransaction?.CreatedAt));
                    Assert.That(transaction?.Payee, Is.EqualTo(expectedTransaction?.Payee));
                });
        return result;
    }

    public static IEnumerable TestCases()
    {
        yield return new TestCaseData("4700 Безвозмездный перевод", null).Returns(false);

        yield return new TestCaseData(
            "Дата транзакции: 30.12.2023 Код авторизации: 001026 Номер карты: 5269********5327 Сумма транзакции:\n"
            + "27.98 EUR Операция: Покупка с нашей карты в чужом устройстве 045875630\\KAZ\\VALENCIA \\0517-\n"
            + "SUP.EX.RAMON LLUL Учетный курс 454.56",
            new Transaction
                {
                    Amount = 27.98m,
                    Currency = "EUR",
                    CreatedAt = TransactionConverter.ConvertDate("30.12.2023"),
                    Payee = "0517-SUP.EX.RAMON LLUL"
                }).Returns(true);

        yield return new TestCaseData(
            "Дата транзакции: 13.01.2024 Код авторизации: 002879 Номер карты: 5269********5327 "
            + "Сумма транзакции: 44.1 EUR Операция: " + "Покупка с нашей карты в чужом устройстве Учетный курс 451.33",
            new Transaction
                {
                    Amount = 44.1m,
                    Currency = "EUR",
                    CreatedAt = TransactionConverter.ConvertDate("13.01.2024"),
                    Payee = null
                }).Returns(true);

        yield return new TestCaseData(
            "Дата транзакции: 11.01.2024 Код авторизации: 102161 Номер карты: 5269********5327 "
            + "Сумма транзакции: 8 EUR Операция: "
            + "Возврат покупки по своей карте в чужом ТСП 322904186\\AUS\\VALENCIA \\TEZENIS JUAN AUSTRIA T "
            + "Учетный курс 451.33",
            new Transaction
                {
                    Amount = 8m,
                    Currency = "EUR",
                    CreatedAt = TransactionConverter.ConvertDate("11.01.2024"),
                    Payee = "TEZENIS JUAN AUSTRIA T"
                }).Returns(true);

        yield return new TestCaseData(
            "Дата транзакции: 01.01.2024 Код авторизации: 214193 Номер карты: 5269********5327 Сумма транзакции:\n"
            + "461.07 EUR Операция: Покупка с нашей карты в чужом устройстве IBA000000003979\\ARE\\BAKI\\AZAL Учетный\n"
            + "курс 457.06",
            new Transaction
                {
                    Amount = 461.07m,
                    Currency = "EUR",
                    CreatedAt = TransactionConverter.ConvertDate("01.01.2024"),
                    Payee = "AZAL"
                }).Returns(true);

        yield return new TestCaseData(
            "Дата транзакции: 07.02.2024 Код авторизации: 918427 Номер карты: 5269********5327 Сумма транзакции: .49 EUR "
            + "Операция: Покупка с нашей карты в чужом устройстве 244559000156182\\ARE\\amazon.es/prm\\Amazon Prime*XI4 "
            + "Учетный курс 450.34",
            new Transaction
                {
                    Amount = 0.49m,
                    Currency = "EUR",
                    CreatedAt = TransactionConverter.ConvertDate("07.02.2024"),
                    Payee = "Amazon Prime*XI4"
                }).Returns(true).SetName("Amount .49 without 0");
    }
}

public class TestDataWrapper<T, TExp>
{
    public T? Value { get; set; }

    public TExp? Expected { get; set; }
}