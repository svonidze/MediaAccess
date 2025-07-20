namespace Money.Cli;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using CommandLine;

using Common.DI;
using Common.DI.Contracts;
using Common.Http;
using Common.Reflection;
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

using Money.Cli.Parameters;

using ZenMoney.Integration;

using Constants = ZenMoney.Integration.Constants;

internal class Program
{
    private static async Task Main(string[] args)
    {
        await _ParseArgumentsAndRun<DefaultParameters>(args, parameters => _Run(parameters, args));
    }

    private static async Task _Run(DefaultParameters launcherParameters, string[] args)
    {
        switch (launcherParameters.ConvertFrom)
        {
            case DataSourceType.ModulDengi:
                await _ParseArgumentsSetUpAndRun<ModulDengiParameters>(args, _HandleModulDengi);
                break;
            case DataSourceType.FreedomFinanceBank:
                await _ParseArgumentsSetUpAndRun<FreedomFinanceBankParameters>(args, _HandleFreedomFinanceBank);
                break;
            case DataSourceType.ZenMoney:
                await _ParseArgumentsSetUpAndRun<ZenMoneyParameters>(args, _HandleZenMoney);
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
        serviceCollection.AddTransient(typeof(IFactory<>), typeof(Factory<>))
            .AddTransient(typeof(Lazy<>), typeof(Lazier<>));

        _SetUpLogging(serviceCollection);

        serviceCollection.AddTransient<HttpMessageHandler, HttpClientHandler>()
            .AddTransient<LoggingHandler>()
            .AddTransient<HttpClient>(provider => new HttpClient(provider.GetRequiredService<LoggingHandler>()))
            .AddTransient<IHttpRequestBuilder, HttpRequestBuilder>();

        serviceCollection.AddTransient<ZenMoneyClient>();
        serviceCollection.AddTransient<ExcelStatementReader>();
        
        var serviceProviderLazy = LazyExtensions.InitLazy(() => serviceCollection.BuildServiceProvider());
        
        await _ParseArgumentsAndRun<TParam>(
            args,
            parameters => action(
                parameters,
                serviceProviderLazy,
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
                // DataSourceType.ZenMoney => ZenMoneyConverter.ConvertToJsFetchRequest(accountStatements),
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
    private static async Task _HandleFreedomFinanceBank(
        FreedomFinanceBankParameters parameters,
        Lazy<ServiceProvider> serviceProvider,
        IServiceCollection serviceCollection)
    {
        serviceCollection.PostConfigure<ZenMoneyConfiguration>(
            options =>
                {
                    options.LogAndUpdatePropertiesOf<IZenMoneyConfiguration>(
                        parameters,
                        serviceProvider.Value.GetRequiredService<ILogger<ZenMoneyParameters>>(),
                        ignoreNullValues: true);
                });
        
        switch (parameters.ConvertTo)
        {
            case DataSourceType.ZenMoney:
                var excelStatementReader = serviceProvider.Value.GetRequiredService<ExcelStatementReader>();
                var ffinTransactions = excelStatementReader.Extract(parameters.InputFilePath).ToArray();
                var zenMoneyClient = serviceProvider.Value.GetRequiredService<ZenMoneyClient>();

                foreach (var transaction in ZenMoneyConverter.ConvertToTransactions(
                             ffinTransactions,
                             accountId: Constants.Wallets.FreedomFinanceBank.Eur))
                {
                    await zenMoneyClient.CreateTransaction(transaction);
                }

                break;
            default:
                throw new NotSupportedException($"{nameof(_HandleFreedomFinanceBank)} {parameters.ConvertTo}");
        }
    }

    private static Task _HandleZenMoney(
        ZenMoneyParameters parameters,
        Lazy<ServiceProvider> serviceProvider,
        IServiceCollection serviceCollection)
    {
        serviceCollection.PostConfigure<ZenMoneyConfiguration>(
            options =>
                {
                    options.LogAndUpdatePropertiesOf<IZenMoneyConfiguration>(
                        parameters,
                        serviceProvider.Value.GetRequiredService<ILogger<ZenMoneyParameters>>(),
                        ignoreNullValues: true);
                });
        
        var zenMoneyClient = serviceProvider.Value.GetRequiredService<ZenMoneyClient>();
        throw new NotImplementedException();
        return Task.CompletedTask;
    }

    private static void _SetUpLogging(IServiceCollection serviceCollection)
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Trace));

        serviceCollection
            .AddSingleton(loggerFactory)
            .AddTransient(typeof(ILogger<>), typeof(Logger<>))
            .AddTransient<ILogger>(_ => loggerFactory.CreateLogger<Program>());
    }
}