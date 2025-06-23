namespace ModulDengi.Cli;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using CommandLine;

using Common.DI;
using Common.Http;
using Common.Serialization.Json;
using Common.System;
using Common.System.Collections;

using FreedomFinanceBank.Integration;

using Kontur.Elba.Integration;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using ModulDengi.Contracts;
using ModulDengi.Integration;
using ModulDengi.Integration.Contracts;

using ZenMoney.Integration;

internal class Program
{
    private static async Task Main(string[] args)
    {
        await _ParseArgumentsAndRun<LauncherParameters>(args, parameters => _Run(parameters, args));
    }

    private static async Task _Run(LauncherParameters launcherParameters, string[] args)
    {
        //TextWriter oldOut = Console.Out;
        const string LogFilePath = "./run.log";

        Console.WriteLine($@"Console log will be redirected to file {LogFilePath}");

        using var fileStream = new FileStream(LogFilePath, FileMode.Create, FileAccess.Write);
        using var writer = new StreamWriter(fileStream);
        Console.SetOut(writer);

        switch (launcherParameters.ConvertFrom)
        {
            case DataSourceType.ModulDengi:
                await _ParseArgumentsSetUpAndRun<ModulDengiParameters>(args, _HandleModulDengi);
                break;
            case DataSourceType.FreedomFinanceBank:
                await _ParseArgumentsSetUpAndRun<FreedomFinanceBankParameters>(args, _HandleFreedomFinanceBank);
                break;
            default:
                throw new NotSupportedException($"{launcherParameters.ConvertFrom}");
        }
    }

    private static async Task _ParseArgumentsAndRun<TParam>(IEnumerable<string> args, Func<TParam, Task> action)
    {
        using var parser = new Parser(settings => { settings.IgnoreUnknownArguments = true; });
        var parserResult = parser.ParseArguments<TParam>(args);
        await parserResult.WithParsedAsync(action);
        parserResult.WithNotParsed(errors => Console.WriteLine(
            errors.Select(e => $"{e.GetType().Name}: {e.ToJson()}").JoinToString(Environment.NewLine)));
    }

    private static async Task _ParseArgumentsSetUpAndRun<TParam>(
        IEnumerable<string> args,
        Func<TParam, Lazy<ServiceProvider>, IServiceCollection, Task> action)
    {
        var serviceCollection = new ServiceCollection().AddOptions();
        serviceCollection.AddTransient(typeof(Lazy<>), typeof(Lazier<>));

        _SetUpLogging(serviceCollection);

        serviceCollection.AddTransient<IHttpRequestBuilder, HttpRequestBuilder>();

        await _ParseArgumentsAndRun<TParam>(
            args,
            parameters => action(
                parameters,
                LazyExtensions.InitLazy(() => serviceCollection.BuildServiceProvider()),
                serviceCollection));
    }

    private static async Task _HandleModulDengi(
        ModulDengiParameters parameters,
        Lazy<ServiceProvider> serviceProvider,
        IServiceCollection serviceCollection)
    {
        var config = CreateModulDengiAccessConfig();

        serviceCollection.AddSingleton(Options.Create(config)).AddTransient<IModulDengiApi, ModulDengiApi>();

        var api = serviceProvider.Value.GetRequiredService<IModulDengiApi>();
        var accountStatements =
            (await api.GetAccountStatements(config.MyCompanyId, dateSince: parameters.DateSince))!.OrderBy(x =>
                x.CreatedAt);

        var items = parameters.ConvertTo switch
            {
                DataSourceType.Elba => ModulDengiToElbaConverter.ConvertToJsFetchRequest(accountStatements),
                DataSourceType.ZenMoney => ZenMoneyConverter.ConvertToJsFetchRequest(accountStatements),
                _ => throw new NotSupportedException($"{parameters.ConvertTo}")
            };
        foreach (var item in items)
        {
            Console.WriteLine(item);
        }

        return;

        ModulDengiAccessConfig CreateModulDengiAccessConfig() =>
            new()
                {
                    SiteUrl = parameters.Uri,
                    MyCompanyId = parameters.CompanyId,
                    Credential = new Credential
                        {
                            Login = parameters.Login,
                            Password = parameters.Password
                        }
                };
    }

    /// <summary>
    /// 1 download PDF statement from FFIN mobile application
    /// 2 convert the PDF to XLSX
    /// open https://www.adobe.com/acrobat/online/pdf-to-excel.html in a browser with the incognito mode
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="serviceProvider"></param>
    /// <param name="serviceCollection"></param>
    /// <exception cref="NotSupportedException"></exception>
    private static Task _HandleFreedomFinanceBank(
        FreedomFinanceBankParameters parameters,
        Lazy<ServiceProvider> serviceProvider,
        IServiceCollection serviceCollection)
    {
        switch (parameters.ConvertTo)
        {
            case DataSourceType.ZenMoney:
                var transactions = ExcelStatementReader.Extract(parameters.InputFilePath).ToArray();

                foreach (var item in ZenMoneyConverter.ConvertToJsFetchRequest(transactions))
                {
                    Console.WriteLine(item);
                }

                break;
            default:
                throw new NotSupportedException($"{nameof(_HandleFreedomFinanceBank)} {parameters.ConvertTo}");
        }
        return Task.CompletedTask;
    }

    private static void _SetUpLogging(IServiceCollection serviceCollection)
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Trace));

        serviceCollection.AddSingleton(loggerFactory).AddTransient(typeof(ILogger<>), typeof(Logger<>))
            .AddTransient<ILogger>(_ => loggerFactory.CreateLogger<Program>());
    }
}