namespace ZenMoney.Integration
{
    using ModulDengi.Contracts;
    using ModulDengi.Core;

    public static class Constants
    {
        public const string ZenmoneyUrl = "https://zenmoney.ru";

        public static readonly string ZenmoneyApiUrl = $"{ZenmoneyUrl}/api/v2/transaction/";

        public const int ZeroValue = 0;
        public const string ZeroStringValue = "0";

        public static readonly AccountStatementType[] IgnoringAccountStatementType =
            {
                AccountStatementType.Unknown, AccountStatementType.Income, AccountStatementType.Outcome
            };

        public static class Categories
        {
            public const int Комиссия = 14916896;

            public const int ДолгПроценты = 14917238;
        }

        public static class Wallets
        {
            public const int МодульДеньгиRub = 4162463;

            public const int Долг = 4161152;

            public static class FreedomFinanceBank
            {
                public const int Eur = 11009254;
                public const int Usd = 11082421;
            }
        }
    }
}