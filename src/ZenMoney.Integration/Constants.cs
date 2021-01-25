namespace ZenMoney.Integration
{
    public static class Constants
    {
        public const string ZenmoneyUrl = "https://zenmoney.ru";
        public static readonly string ZenmoneyApiUrl = $"{ZenmoneyUrl}/api/v2/transaction/";
        public const string ZeroValue = "0";

        public static readonly string[] IgnoringTypes = {
                "unknown", "income", "outcome"
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
        }
    }
}