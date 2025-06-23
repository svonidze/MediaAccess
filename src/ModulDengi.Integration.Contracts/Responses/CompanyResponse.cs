namespace ModulDengi.Integration.Contracts.Responses;

using System;
using System.Collections.Generic;

using ModulDengi.Integration.Contracts.Types;

using Newtonsoft.Json;

public class CompanyResponse : ModulDengiResponse
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("typeId")]
    public string TypeId { get; set; }

    [JsonProperty("fullName")]
    public string FullName { get; set; }

    [JsonProperty("shortName")]
    public string ShortName { get; set; }

    [JsonProperty("formalAddress")]
    public string FormalAddress { get; set; }

    [JsonProperty("ogrn")]
    public string Ogrn { get; set; }

    [JsonProperty("inn")]
    public string Inn { get; set; }

    [JsonProperty("kpp")]
    public string Kpp { get; set; }

    [JsonProperty("registerDateAt")]
    public DateTime RegisterDateAt { get; set; }

    [JsonProperty("directorFio")]
    public string DirectorFio { get; set; }

    [JsonProperty("phone")]
    public string Phone { get; set; }

    [JsonProperty("isApproved")]
    public bool IsApproved { get; set; }

    [JsonProperty("isEulaApproved")]
    public bool IsEulaApproved { get; set; }

    [JsonProperty("isEulaBorrowerApproved")]
    public bool IsEulaBorrowerApproved { get; set; }

    [JsonProperty("isPhoneApproved")]
    public bool IsPhoneApproved { get; set; }

    [JsonProperty("isRealCompany")]
    public bool IsRealCompany { get; set; }

    [JsonProperty("isDefenseHolder")]
    public bool IsDefenseHolder { get; set; }

    [JsonProperty("qiStatus")]
    public string QiStatus { get; set; }

    [JsonProperty("qiComment")]
    public string QiComment { get; set; }

    [JsonProperty("otherPlatforms")]
    public string OtherPlatforms { get; set; }

    [JsonProperty("modulebankAccountNumber")]
    public string ModulebankAccountNumber { get; set; }

    [JsonProperty("modulebankBik")]
    public string ModulebankBik { get; set; }

    [JsonProperty("contractsListUrl")]
    public string ContractsListUrl { get; set; }

    [JsonProperty("contracts223ListUrl")]
    public string Contracts223ListUrl { get; set; }

    [JsonProperty("isEulaEdsNeeded")]
    public bool IsEulaEdsNeeded { get; set; }

    [JsonProperty("isEulaEdsSigned")]
    public bool IsEulaEdsSigned { get; set; }

    [JsonProperty("documents")]
    public List<Document> Documents { get; set; }

    [JsonProperty("borrowerAccountNumber")]
    public string BorrowerAccountNumber { get; set; }

    [JsonProperty("investorAccountNumber")]
    public string InvestorAccountNumber { get; set; }
}