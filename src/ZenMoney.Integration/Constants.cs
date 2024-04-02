namespace ZenMoney.Integration
{
    using ModulDengi.Contracts;
    using ModulDengi.Core;

    public static class Constants
    {
        public const string ZenmoneyUrl = "https://zenmoney.ru";

        public static readonly string ZenmoneyApiUrl = $"{ZenmoneyUrl}/api/v2/transaction/";

        public const string ZeroValue = "0";

        public static readonly AccountStatementType[] IgnoringAccountStatementType =
            {
                AccountStatementType.Unknown, AccountStatementType.Income, AccountStatementType.Outcome
            };

        public static class Categories
        {
            public const string Комиссия = "14916896";

            public const string ДолгПроценты = "14917238";
        }

        public static class Wallets
        {
            public const string МодульДеньгиRub = "4162463";

            public const string Долг = "4161152";

            public static class FreedomFinanceBank
            {
                public const string Eur = "11009254";
                public const string Usd = "11082421";
            }
        }
    }
}