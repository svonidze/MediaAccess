namespace Money.Cli.Parameters;

using CommandLine;

internal class DefaultParameters
{
    [Option("convert-from", Required = true)]
    public DataSourceType ConvertFrom { get; set; }
}