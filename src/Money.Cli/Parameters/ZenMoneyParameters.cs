namespace Money.Cli.Parameters;

using CommandLine;

using ZenMoney.Integration;

internal class ZenMoneyParameters : IZenMoneyConfiguration
{
    [Option("convert-to", Required = true)]
    public DataSourceType ConvertTo { get; set; }

    [Option("cookie", Required = true)]
    public string Cookie { get; set; }
}