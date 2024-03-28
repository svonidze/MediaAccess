namespace ModulDengi.Cli;

using System;

using CommandLine;

internal class LauncherParameters
{
    [Option("convert-from", Required = true)]
    public DataSourceType ConvertFrom { get; set; }
}

internal class ModulDengiParameters
{
    [Option("convert-to", Required = true)]
    public DataSourceType ConvertTo { get; set; }

    [Option("company-id", Required = true)]
    public string CompanyId { get; set; } = null!;

    [Option("uri", Required = true)]
    public string Uri { get; set; } = null!;

    [Option("login", Required = true)]
    public string Login { get; set; } = null!;

    [Option("password", Required = true)]
    public string Password { get; set; } = null!;

    [Option("since", Required = true)]
    public DateTime DateSince { get; set; }
}

internal class FreedomFinanceBankParameters
{
    [Option("input-file-path", Required = true)]
    public string InputFilePath { get; set; } = null!;
}