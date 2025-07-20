namespace Money.Cli.Parameters;

using CommandLine;

internal class FreedomFinanceBankParameters
{
    [Option("convert-to", Required = true)]
    public DataSourceType ConvertTo { get; set; }
    
    [Option("input-file-path", Required = true)]
    public string InputFilePath { get; set; } = null!;
}