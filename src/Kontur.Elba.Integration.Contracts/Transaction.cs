namespace Kontur.Elba.Integration.Contracts
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class Transaction
    {
        [JsonProperty("IsCredit")]
        public bool IsCredit { get; set; }

        [JsonProperty("IsPaidByElbank")]
        public bool IsPaidByElbank { get; set; }

        [JsonProperty("LinkedDocumentViews")]
        public object[] LinkedDocumentViews { get; set; }

        [JsonProperty("IsContractorLocked")]
        public bool IsContractorLocked { get; set; }

        [JsonProperty("Appendix")]
        public object Appendix { get; set; }

        [JsonProperty("Description")]
        public object Description { get; set; }

        [JsonProperty("TaxSum")]
        public double TaxSum { get; set; }

        [JsonProperty("FeeSum")]
        public int FeeSum { get; set; }

        [JsonProperty("ContractorName")]
        public string ContractorName { get; set; }

        [JsonProperty("Sum")]
        public int Sum { get; set; }

        [JsonProperty("IsIntegratedBankPayment")]
        public bool IsIntegratedBankPayment { get; set; }

        [JsonProperty("IsIntegratedCashPayment")]
        public bool IsIntegratedCashPayment { get; set; }

        [JsonProperty("IsIntegratedPayment")]
        public bool IsIntegratedPayment { get; set; }

        [JsonProperty("BankAccountId")]
        public object BankAccountId { get; set; }

        [JsonProperty("BankAccountNumber")]
        public object BankAccountNumber { get; set; }

        [JsonProperty("BankAccountComment")]
        public object BankAccountComment { get; set; }

        [JsonProperty("BankAccountType")]
        public object BankAccountType { get; set; }

        [JsonProperty("LinkedPaymentId")]
        public object LinkedPaymentId { get; set; }

        [JsonProperty("ContractorId")]
        public string ContractorId { get; set; }

        [JsonProperty("ContractorIsWithoutClosingDocuments")]
        public bool ContractorIsWithoutClosingDocuments { get; set; }

        [JsonProperty("EmployeeName")]
        public object EmployeeName { get; set; }

        [JsonProperty("EmployeeId")]
        public object EmployeeId { get; set; }

        [JsonProperty("DocumentType")]
        public string DocumentType { get; set; }

        [JsonProperty("OperationType")]
        public string OperationType { get; set; }

        [JsonProperty("ContributionSubtype")]
        public object ContributionSubtype { get; set; }

        [JsonProperty("FormOfMoney")]
        public int FormOfMoney { get; set; }

        [JsonProperty("ContractorType")]
        public int ContractorType { get; set; }

        [JsonProperty("ContractorBankAccountId")]
        public object ContractorBankAccountId { get; set; }

        [JsonProperty("CbrCurrencyRate")]
        public CbrCurrencyRate CbrCurrencyRate { get; set; }

        [JsonProperty("CurrencyRate")]
        public object CurrencyRate { get; set; }

        [JsonProperty("CurrencyNumCode")]
        public string CurrencyNumCode { get; set; }

        [JsonProperty("TaxType")]
        public object TaxType { get; set; }

        [JsonProperty("PenaltyType")]
        public object PenaltyType { get; set; }

        [JsonProperty("CorrespondingAccount")]
        public object CorrespondingAccount { get; set; }

        [JsonProperty("WageDistributionItems")]
        public List<object> WageDistributionItems { get; set; }

        [JsonProperty("TaxScheme")]
        public int TaxScheme { get; set; }

        [JsonProperty("ContributionYear")]
        public object ContributionYear { get; set; }

        [JsonProperty("Id")]
        public object Id { get; set; }

        [JsonProperty("DocumentNumber")]
        public object DocumentNumber { get; set; }

        [JsonProperty("Date")]
        public string Date { get; set; }

        [JsonProperty("IsNew")]
        public bool IsNew { get; set; }

        [JsonProperty("AllowedOperationTypes")]
        public object AllowedOperationTypes { get; set; }

        [JsonProperty("IsCurrencyAccount")]
        public bool IsCurrencyAccount { get; set; }

        [JsonProperty("DirectionOfMoneyCaption")]
        public string DirectionOfMoneyCaption { get; set; }

        [JsonProperty("BankAccountTaxScheme")]
        public object BankAccountTaxScheme { get; set; }

        [JsonProperty("CbrCurrencyValue")]
        public int CbrCurrencyValue { get; set; }

        [JsonProperty("DocumentNumberLimit")]
        public int DocumentNumberLimit { get; set; }

        [JsonProperty("TaxLabel")]
        public string TaxLabel { get; set; }

        [JsonProperty("AppendixTooltipContent")]
        public string AppendixTooltipContent { get; set; }

        [JsonProperty("TaxSumTooltipContent")]
        public string TaxSumTooltipContent { get; set; }

        [JsonProperty("CurrencyRateLabel")]
        public string CurrencyRateLabel { get; set; }

        [JsonProperty("CbrCurrencyRateNominal")]
        public int CbrCurrencyRateNominal { get; set; }

        [JsonProperty("CurrencyRateIsCustom")]
        public bool CurrencyRateIsCustom { get; set; }

        [JsonProperty("CbrCurrencyRateValue")]
        public int CbrCurrencyRateValue { get; set; }

        [JsonProperty("CurrencyNominal")]
        public int CurrencyNominal { get; set; }

        [JsonProperty("CbrCurrencyNominal")]
        public int CbrCurrencyNominal { get; set; }

        [JsonProperty("CurrencyRateDifferenceTooltipContent")]
        public string CurrencyRateDifferenceTooltipContent { get; set; }

        [JsonProperty("ActionTypeInTooltip")]
        public string ActionTypeInTooltip { get; set; }

        [JsonProperty("DateCaption")]
        public string DateCaption { get; set; }

        [JsonProperty("FormOfMoneyKind")]
        public bool FormOfMoneyKind { get; set; }

        [JsonProperty("ContractorIsWithoutClosingDocumentsCheckbox")]
        public bool ContractorIsWithoutClosingDocumentsCheckbox { get; set; }

        [JsonProperty("ContractorCanBeWithoutClosingDocuments")]
        public bool ContractorCanBeWithoutClosingDocuments { get; set; }

        [JsonProperty("CashOrderLink")]
        public string CashOrderLink { get; set; }

        [JsonProperty("OperationTypeData")]
        public OperationTypeData OperationTypeData { get; set; }

        [JsonProperty("IsPaymentWithTaxType")]
        public bool IsPaymentWithTaxType { get; set; }

        [JsonProperty("IsPaymentWithCorrespondingAccount")]
        public bool IsPaymentWithCorrespondingAccount { get; set; }

        [JsonProperty("IsContributionsOperationType")]
        public bool IsContributionsOperationType { get; set; }

        [JsonProperty("IsOperationWithContributionYear")]
        public bool IsOperationWithContributionYear { get; set; }

        [JsonProperty("IsPaymentWithPenaltyType")]
        public bool IsPaymentWithPenaltyType { get; set; }

        [JsonProperty("EmployeeLabel")]
        public string EmployeeLabel { get; set; }

        [JsonProperty("IsWageDistributionOperationType")]
        public bool IsWageDistributionOperationType { get; set; }

        [JsonProperty("HasTaxSum")]
        public bool HasTaxSum { get; set; }

        [JsonProperty("CanChooseTaxScheme")]
        public bool CanChooseTaxScheme { get; set; }

        [JsonProperty("IsEditableCurrencyRate")]
        public bool IsEditableCurrencyRate { get; set; }

        [JsonProperty("IsDocumentNumberIsTooLong")]
        public bool IsDocumentNumberIsTooLong { get; set; }

        [JsonProperty("CurrencySymbol")]
        public string CurrencySymbol { get; set; }

        [JsonProperty("UsnTaxSum")]
        public double UsnTaxSum { get; set; }

        [JsonProperty("PatentTaxSum")]
        public int PatentTaxSum { get; set; }
    }
}