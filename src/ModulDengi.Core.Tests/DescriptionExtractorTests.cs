namespace ModulDengi.Core.Tests
{
    using System.Collections;

    using ModulDengi.Contracts;

    using NUnit.Framework;

    [TestFixture]
    public class DescriptionExtractorTests
    {
        [TestCaseSource(typeof(MyFactoryClass))]
        public Description ExtractTest(string input)
        {
            return DescriptionExtractor.Extract(input);
        }
    }

    /*
    "title": "Вывод средств",
    "type": "outcome",
    "description": "Перевод денежных средств с лицевого счета 101260 на банковский счет клиента",
    
    "title": "Пополнение (счет)",
    "type": "income",
    "description": "Пополнение лицевого счета 101260 переводом с банковского счета клиента",
    
    "title": "Отмена сбора средств проекта 308311",
    "type": "unknown",
    "description": "Отмена сбора средств проекта 308311",
     */
    
    public class MyFactoryClass : IEnumerable
    {
        private static readonly TestCaseData[] TestCases =
            {
                TestCase("Отмена сбора средств проекта 308311", projectId: 308311),
                //"fund": "title": "Выдача займа",
                TestCase(
                    "Предоставление займа по договору займа 308972/301511823830 от 21.01.2021. "
                    + "Заемщик ООО \"ПЛАТОН\". Срок 60 дней. Процентная ставка 21%",
                    projectId: 308972,
                    borrowerName: "ООО \"ПЛАТОН\"",
                    percent: 21,
                    timePeriodInDays: 60),
                //"refundPenaltyPercentage": "title": "Получение процентов по займу (штрафные сверхсрочные)",
                TestCase(
                    "Частичное получение штрафного (сверхсрочного) дохода по договору займа 302332/301511823830. "
                    + "Заемщик ИП Толстых Эльвира Александровна. Срок 702 дня. Процентная ставка 51%",
                    projectId: 302332,
                    borrowerName: "ИП Толстых Эльвира Александровна",
                    percent: 51,
                    timePeriodInDays: 702),
                //"refundMain": "title": "Возврат займа",
                TestCase(
                    "Частичный возврат основной части долга по договору займа 302332/301511823830. "
                    + "Заемщик ИП Толстых Эльвира Александровна. Срок 674 дня.",
                    projectId: 302332,
                    borrowerName: "ИП Толстых Эльвира Александровна",
                    timePeriodInDays: 674),
                //"refundMain": "title": "Возврат займа",
                TestCase(
                    "Возврат основного долга по договору займа 308440/301511823830. "
                    + "Заемщик ООО \"КОННЕКТ\". Срок 63 дня. Процентная ставка 25%",
                    projectId: 308440,
                    borrowerName: "ООО \"КОННЕКТ\"",
                    timePeriodInDays: 63,
                    percent: 25),
                //"refundPercentage": "title": "Получение процентов по займу (срочные)",
                TestCase(
                    "Получение процентного дохода по договору займа 308440/301511823830. "
                    + "Заемщик ООО \"КОННЕКТ\". Срок 63 дня. Процентная ставка 25%",
                    projectId: 308440,
                    borrowerName: "ООО \"КОННЕКТ\"",
                    timePeriodInDays: 63,
                    percent: 25),
                //"cessionCommission": "title": "Комиссия по цессии",
                TestCase(
                    "Комиссия платформы по цессии (15%) по договору займа 302332/301511823830. Заемщик ИП Толстых Эльвира Александровна.",
                    projectId: 302332,
                    borrowerName: "ИП Толстых Эльвира Александровна"),
                //"refundPenaltyPercentage": "title": "Получение процентов по займу (штрафные сверхсрочные)",
                TestCase(
                    "Частичное получение штрафного (сверхсрочного) дохода по договору займа 302332/301511823830. "
                    + "Заемщик ИП Толстых Эльвира Александровна. Срок 702 дня. Процентная ставка 51%",
                    projectId: 302332,
                    borrowerName: "ИП Толстых Эльвира Александровна",
                    percent: 51,
                    timePeriodInDays: 702),
                //"refundPercentage": "title": "Получение процентов по займу (срочные)",
                TestCase(
                    "Получение процентного дохода по договору займа 308587/301511823830. Заемщик ООО \"СТРОИЛАЙН\". Срок 72 дня. Процентная ставка 27%",
                    projectId: 308587,
                    borrowerName: "ООО \"СТРОИЛАЙН\"",
                    percent: 27,
                    timePeriodInDays: 72),
                TestCase(
                    "Получение процентного дохода по договору займа 306519/301511823830. Заемщик ООО \"ТРАНС-ОЙЛ\". Срок 106 дней. Процентная ставка 25%",
                    projectId: 306519,
                    borrowerName: "ООО \"ТРАНС-ОЙЛ\"",
                    percent: 25,
                    timePeriodInDays: 106)
                    .SetName("With - in BorrowerName"),
            };

        private static TestCaseData TestCase(
            string input,
            int projectId,
            string borrowerName = null,
            int? timePeriodInDays = null,
            int? percent = null) =>
            new TestCaseData(input).Returns(new Description(projectId, borrowerName, timePeriodInDays, percent));

        IEnumerator IEnumerable.GetEnumerator() => TestCases.GetEnumerator();
    }
}