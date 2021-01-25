namespace Kontur.Elba.Integration.Contracts
{
    using Newtonsoft.Json;

    public class OperationTypeData
    {
        [JsonProperty("Key")]
        public string Key { get; set; }

        [JsonProperty("FormOfMoney")]
        public int FormOfMoney { get; set; }

        [JsonProperty("Caption")]
        public string Caption { get; set; }

        [JsonProperty("IsIncome")]
        public bool IsIncome { get; set; }

        [JsonProperty("ContractorType")]
        public int ContractorType { get; set; }

        [JsonProperty("DontUseInTax")]
        public bool DontUseInTax { get; set; }

        [JsonProperty("CashBookCorrespondingAccountingRecords")]
        public object CashBookCorrespondingAccountingRecords { get; set; }

        [JsonProperty("ForBankAccountTypes")]
        public object ForBankAccountTypes { get; set; }

        [JsonProperty("EditableCurrencyRate")]
        public bool EditableCurrencyRate { get; set; }

        [JsonProperty("HideTaxSum")]
        public bool HideTaxSum { get; set; }

        [JsonProperty("TaxSubject")]
        public object TaxSubject { get; set; }

        [JsonProperty("ShowContractorWithoutClosingDocuments")]
        public bool ShowContractorWithoutClosingDocuments { get; set; }

        [JsonProperty("CanBePlanned")]
        public bool CanBePlanned { get; set; }

        [JsonProperty("ExactLegalForm")]
        public object ExactLegalForm { get; set; }

        [JsonProperty("SupportPatent")]
        public bool SupportPatent { get; set; }

        [JsonProperty("WithFee")]
        public bool WithFee { get; set; }

        [JsonProperty("CanBeLinkedToDocument")]
        public bool CanBeLinkedToDocument { get; set; }

        [JsonProperty("MinimumAllowedMobileApiVersion")]
        public object MinimumAllowedMobileApiVersion { get; set; }
    }
}