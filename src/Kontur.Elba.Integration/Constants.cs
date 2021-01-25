namespace Kontur.Elba.Integration
{
    using ModulDengi.Contracts;

    public static class Constants
    {
        public static readonly string Url = $"{BaseUrl}/PaymentsLightbox/Save?scope={Scope}";

        public static readonly string ReferUrl = $"{BaseUrl}//List/PaymentsList?scope={Scope}";

        public static readonly AccountStatementType[] UsingAccountStatementType =
            {
                AccountStatementType.RefundPercentage, AccountStatementType.RefundPenaltyPercentage,
                AccountStatementType.CessionCommission,
            };

        private const string BaseUrl = "https://elba.kontur.ru/Business/Payments";

        private const string Scope = "mtvfvuagvo3sanbvvglzya";
    }
}