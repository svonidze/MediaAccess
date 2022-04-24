namespace ModulDengi.Cli;

using System;

using CommandLine;

internal class LauncherParameters
{
    [Option("convert", Required = true)]
    public ConversionDirectionType ConversionDirectionType { get; set; }

    [Option("company-id", Required = true)]
    public string CompanyId { get; set; } = null!;

    [Option("url")]
    public string? Url { get; set; }

    [Option("login", Required = true)]
    public string Login { get; set; } = null!;

    [Option("password", Required = true)]
    public string Password { get; set; } = null!;

    [Option("since", Required = true)]
    public DateTime SinceDate { get; set; }
}