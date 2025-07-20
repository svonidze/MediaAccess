namespace Money.Cli.Parameters;

using CommandLine;

using ZenMoney.Integration;

internal class FreedomFinanceBankParameters : IZenMoneyConfiguration
{
    [Option("convert-to", Required = true)]
    public DataSourceType ConvertTo { get; set; }

    [Option("input-file-path", Required = true)]
    public string InputFilePath { get; set; } = null!;

    [Option("cookie", Required = true)]

    public string Cookie { get; set; }
}