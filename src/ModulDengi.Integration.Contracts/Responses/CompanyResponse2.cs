namespace ModulDengi.Integration.Contracts.Responses
{
    using System;
    using System.Collections.Generic;

    using ModulDengi.Integration.Contracts.Types;

    using Newtonsoft.Json;

    public class CompanyResponse2 : ModulDengiResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("number")]
        public int Number { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("statusId")]
        public string StatusId { get; set; }

        [JsonProperty("tenderNumber")]
        public string TenderNumber { get; set; }

        [JsonProperty("tenderLaw")]
        public string TenderLaw { get; set; }

        [JsonProperty("tenderPlatformName")]
        public string TenderPlatformName { get; set; }

        [JsonProperty("tenderPlatformUri")]
        public string TenderPlatformUri { get; set; }

        [JsonProperty("gosTenderUri")]
        public string GosTenderUri { get; set; }

        [JsonProperty("tenderDocumentsUri")]
        public string TenderDocumentsUri { get; set; }

        [JsonProperty("tenderAmount")]
        public string TenderAmount { get; set; }

        [JsonProperty("contractStatusPageUrl")]
        public string ContractStatusPageUrl { get; set; }

        [JsonProperty("eulaVersion")]
        public string EulaVersion { get; set; }

        [JsonProperty("debtInfoCreditsSum")]
        public string DebtInfoCreditsSum { get; set; }

        [JsonProperty("debtInfoLoansSum")]
        public string DebtInfoLoansSum { get; set; }

        [JsonProperty("contractAmount")]
        public string ContractAmount { get; set; }

        [JsonProperty("minLoanAmount")]
        public string MinLoanAmount { get; set; }

        [JsonProperty("contractSignedAt")]
        public DateTime ContractSignedAt { get; set; }

        [JsonProperty("contractType")]
        public string ContractType { get; set; }

        [JsonProperty("contractFinishAt")]
        public DateTime ContractFinishAt { get; set; }

        [JsonProperty("loanUnderwriterApprovedAt")]
        public DateTime LoanUnderwriterApprovedAt { get; set; }

        [JsonProperty("contractAdvancePay")]
        public string ContractAdvancePay { get; set; }

        [JsonProperty("isContractPaymentsModulbank")]
        public bool IsContractPaymentsModulbank { get; set; }

        [JsonProperty("bankGuarantee")]
        public string BankGuarantee { get; set; }

        [JsonProperty("contractPaymentType")]
        public string ContractPaymentType { get; set; }

        [JsonProperty("contractContents")]
        public string ContractContents { get; set; }

        [JsonProperty("customerName")]
        public string CustomerName { get; set; }

        [JsonProperty("customerUri")]
        public string CustomerUri { get; set; }

        [JsonProperty("customerInn")]
        public string CustomerInn { get; set; }

        [JsonProperty("customerOgrn")]
        public string CustomerOgrn { get; set; }

        [JsonProperty("customerAddress")]
        public string CustomerAddress { get; set; }

        [JsonProperty("borrowerSite")]
        public string BorrowerSite { get; set; }

        [JsonProperty("borrowerDirector")]
        public string BorrowerDirector { get; set; }

        [JsonProperty("borrowerDirectorBirthAt")]
        public DateTime BorrowerDirectorBirthAt { get; set; }

        [JsonProperty("isDirectorOwner")]
        public bool IsDirectorOwner { get; set; }

        [JsonProperty("currentDebtAmount")]
        public string CurrentDebtAmount { get; set; }

        [JsonProperty("overdueDebtAmount")]
        public string OverdueDebtAmount { get; set; }

        [JsonProperty("overdueIn29Days")]
        public int OverdueIn29Days { get; set; }

        [JsonProperty("overdueIn89Days")]
        public int OverdueIn89Days { get; set; }

        [JsonProperty("overdueOver90Days")]
        public int OverdueOver90Days { get; set; }

        [JsonProperty("executionAmount")]
        public string ExecutionAmount { get; set; }

        [JsonProperty("arbitrageDebt")]
        public string ArbitrageDebt { get; set; }

        [JsonProperty("foundersHasForeign")]
        public bool FoundersHasForeign { get; set; }

        [JsonProperty("foundersHasConviction")]
        public bool FoundersHasConviction { get; set; }

        [JsonProperty("foundersHasInvalidPassport")]
        public bool FoundersHasInvalidPassport { get; set; }

        [JsonProperty("foundersHasOverdueDebt")]
        public bool FoundersHasOverdueDebt { get; set; }

        [JsonProperty("last24MonthContractsDone")]
        public int Last24MonthContractsDone { get; set; }

        [JsonProperty("last24MonthContractsDoneSameWork")]
        public int Last24MonthContractsDoneSameWork { get; set; }

        [JsonProperty("last24MonthContractsDoneSameCustomer")]
        public int Last24MonthContractsDoneSameCustomer { get; set; }

        [JsonProperty("last24MonthContractsDoneSimilarAmount")]
        public int Last24MonthContractsDoneSimilarAmount { get; set; }

        [JsonProperty("last24MonthContractsDoneAmount")]
        public int Last24MonthContractsDoneAmount { get; set; }

        [JsonProperty("last12MonthContractsDone")]
        public int Last12MonthContractsDone { get; set; }

        [JsonProperty("last24MonthMaxAmount")]
        public int Last24MonthMaxAmount { get; set; }

        [JsonProperty("last12MonthContractsDoneAmount")]
        public int Last12MonthContractsDoneAmount { get; set; }

        [JsonProperty("currentContractsAmount")]
        public int CurrentContractsAmount { get; set; }

        [JsonProperty("currentContracts")]
        public int CurrentContracts { get; set; }

        [JsonProperty("currentContractsSameWork")]
        public int CurrentContractsSameWork { get; set; }

        [JsonProperty("currentContractsSameCustomer")]
        public int CurrentContractsSameCustomer { get; set; }

        [JsonProperty("currentContractsSimilarAmount")]
        public int CurrentContractsSimilarAmount { get; set; }

        [JsonProperty("loanWantedAmount")]
        public string LoanWantedAmount { get; set; }

        [JsonProperty("loanAmount")]
        public string LoanAmount { get; set; }

        [JsonProperty("loanTerm")]
        public int LoanTerm { get; set; }

        [JsonProperty("loanDue")]
        public DateTime LoanDue { get; set; }

        [JsonProperty("initialLoanDue")]
        public DateTime InitialLoanDue { get; set; }

        [JsonProperty("initialLoanTerm")]
        public string InitialLoanTerm { get; set; }

        [JsonProperty("fundsStartedAt")]
        public DateTime FundsStartedAt { get; set; }

        [JsonProperty("loanFundedAt")]
        public DateTime LoanFundedAt { get; set; }

        [JsonProperty("loanCancelledAt")]
        public DateTime LoanCancelledAt { get; set; }

        [JsonProperty("loanRate")]
        public string LoanRate { get; set; }

        [JsonProperty("loanSchedule")]
        public string LoanSchedule { get; set; }

        [JsonProperty("loanRaisedFunds")]
        public string LoanRaisedFunds { get; set; }

        [JsonProperty("loanRaisingDue")]
        public DateTime LoanRaisingDue { get; set; }

        [JsonProperty("loanRisingTerm")]
        public int LoanRisingTerm { get; set; }

        [JsonProperty("loanMaturityValue")]
        public string LoanMaturityValue { get; set; }

        [JsonProperty("loanPenaltyRate")]
        public string LoanPenaltyRate { get; set; }

        [JsonProperty("loanPublicUnderwriterComment")]
        public string LoanPublicUnderwriterComment { get; set; }

        [JsonProperty("loanPrivateUnderwriterComment")]
        public string LoanPrivateUnderwriterComment { get; set; }

        [JsonProperty("loanRefundedAt")]
        public DateTime LoanRefundedAt { get; set; }

        [JsonProperty("loanPenaltyUpdatedAt")]
        public DateTime LoanPenaltyUpdatedAt { get; set; }

        [JsonProperty("mmLoansCount")]
        public int MmLoansCount { get; set; }

        [JsonProperty("mmRefundedLoansCount")]
        public int MmRefundedLoansCount { get; set; }

        [JsonProperty("mmLoansSameCustomer")]
        public int MmLoansSameCustomer { get; set; }

        [JsonProperty("mmOverdueLoansCount")]
        public int MmOverdueLoansCount { get; set; }

        [JsonProperty("mmLoansAmount")]
        public string MmLoansAmount { get; set; }

        [JsonProperty("mmRefundedLoansAmount")]
        public string MmRefundedLoansAmount { get; set; }

        [JsonProperty("mmCurrentAmount")]
        public string MmCurrentAmount { get; set; }

        [JsonProperty("mmInvestorsCount")]
        public int MmInvestorsCount { get; set; }

        [JsonProperty("mmFeeRate")]
        public string MmFeeRate { get; set; }

        [JsonProperty("mmCurrentLoansCount")]
        public int MmCurrentLoansCount { get; set; }

        [JsonProperty("myInvestmentAmount")]
        public int MyInvestmentAmount { get; set; }

        [JsonProperty("hasOverdue")]
        public bool HasOverdue { get; set; }

        [JsonProperty("isPartialPayoff")]
        public bool IsPartialPayoff { get; set; }

        [JsonProperty("isCessionPrefixed")]
        public bool IsCessionPrefixed { get; set; }

        [JsonProperty("cessionPrefixDateAt")]
        public DateTime CessionPrefixDateAt { get; set; }

        [JsonProperty("contractStatusId")]
        public string ContractStatusId { get; set; }

        [JsonProperty("contractPayoffAmount")]
        public string ContractPayoffAmount { get; set; }

        [JsonProperty("contractPayoffDateAt")]
        public DateTime ContractPayoffDateAt { get; set; }

        [JsonProperty("contractTermAt")]
        public DateTime ContractTermAt { get; set; }

        [JsonProperty("isDefenseEnabled")]
        public bool IsDefenseEnabled { get; set; }

        [JsonProperty("defenseLimit")]
        public int DefenseLimit { get; set; }

        [JsonProperty("defenseTariffs")]
        public List<object> DefenseTariffs { get; set; }

        [JsonProperty("investments")]
        public List<Investment> Investments { get; set; }

        [JsonProperty("probation")]
        public Probation Probation { get; set; }

        [JsonProperty("borrower")]
        public Borrower Borrower { get; set; }

        [JsonProperty("publicCommentForInvestor")]
        public string PublicCommentForInvestor { get; set; }

        [JsonProperty("loanOnlyBorrowerComment")]
        public string LoanOnlyBorrowerComment { get; set; }

        [JsonProperty("state")]
        public State State { get; set; }

        [JsonProperty("calcCommon")]
        public CalcCommon CalcCommon { get; set; }

        [JsonProperty("calcBorrower")]
        public CalcBorrower CalcBorrower { get; set; }

        [JsonProperty("calcInvestor")]
        public CalcInvestor CalcInvestor { get; set; }

        [JsonProperty("calcCession")]
        public CalcCession CalcCession { get; set; }

        [JsonProperty("mmLoansCountNow")]
        public int MmLoansCountNow { get; set; }

        [JsonProperty("mmLoansAmountNow")]
        public double MmLoansAmountNow { get; set; }

        [JsonProperty("mmCurrentLoansCountNow")]
        public int MmCurrentLoansCountNow { get; set; }

        [JsonProperty("mmCurrentAmountNow")]
        public double MmCurrentAmountNow { get; set; }

        [JsonProperty("mmRefundedLoansCountNow")]
        public int MmRefundedLoansCountNow { get; set; }

        [JsonProperty("mmRefundedLoansAmountNow")]
        public double MmRefundedLoansAmountNow { get; set; }

        [JsonProperty("mmOverdueLoansCountNow")]
        public int MmOverdueLoansCountNow { get; set; }

        [JsonProperty("otherProjects")]
        public OtherProjects OtherProjects { get; set; }

        [JsonProperty("borrowerHaveOtherInvestments")]
        public bool BorrowerHaveOtherInvestments { get; set; }

        [JsonProperty("borrowerOtherReservedInvestments")]
        public int BorrowerOtherReservedInvestments { get; set; }

        [JsonProperty("borrowerOtherReservedInvestmentsAmount")]
        public string BorrowerOtherReservedInvestmentsAmount { get; set; }

        [JsonProperty("borrowerOtherFundedInvestments")]
        public int BorrowerOtherFundedInvestments { get; set; }

        [JsonProperty("borrowerOtherFundedInvestmentsAmount")]
        public string BorrowerOtherFundedInvestmentsAmount { get; set; }

        [JsonProperty("documents")]
        public List<Document> Documents { get; set; }

        [JsonProperty("legalCostsPayments")]
        public LegalCostsPayments LegalCostsPayments { get; set; }
    }
}