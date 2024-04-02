namespace ModulDengi.Cli
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using CommandLine;

    using Common.Collections;
    using Common.Http;
    using Common.Serialization.Json;
    using Common.System;

    using FreedomFinanceBank.Integration;

    using Kontur.Elba.Integration;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    using ModulDengi.Contracts;
    using ModulDengi.Integration;
    using ModulDengi.Integration.Contracts;

    using ZenMoney.Integration;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            ParseArgumentsAndRun<LauncherParameters>(args, parameters => Run(parameters, args));
        }

        private static void Run(LauncherParameters launcherParameters, string[] args)
        {
            //TextWriter oldOut = Console.Out;
            const string LogFilePath = "./run.log";

            Console.WriteLine($"Console log will be redirected to file {LogFilePath}");

            using var fileStream = new FileStream(LogFilePath, FileMode.Create, FileAccess.Write);
            using var writer = new StreamWriter(fileStream);
            Console.SetOut(writer);

            switch (launcherParameters.ConvertFrom)
            {
                case DataSourceType.ModulDengi:
                    ParseArgumentsSetUpAndRun<ModulDengiParameters>(
                        args,
                        HandleModulDengi);
                    break;
                case DataSourceType.FreedomFinanceBank:
                    ParseArgumentsSetUpAndRun<FreedomFinanceBankParameters>(
                        args,
                        HandleFreedomFinanceBank);
                    break;
                default:
                    throw new NotSupportedException($"{launcherParameters.ConvertFrom}");
            }
        }

        private static void ParseArgumentsAndRun<TParam>(IEnumerable<string> args, Action<TParam> action)
        {
            using var parser = new Parser(settings => { settings.IgnoreUnknownArguments = true; });
            var parserResult = parser.ParseArguments<TParam>(args);
            parserResult.WithParsed(action);
            parserResult.WithNotParsed(
                errors => Console.WriteLine(
                    errors.Select(e => $"{e.GetType().Name}: {e.ToJson()}").JoinToString(Environment.NewLine)));
        }

        private static void ParseArgumentsSetUpAndRun<TParam>(
            IEnumerable<string> args,
            Action<TParam, Lazy<ServiceProvider>, IServiceCollection> action)
        {
            var serviceCollection = new ServiceCollection().AddOptions()
                .AddTransient(_ => new HttpRequestBuilder(enableLogging: true));

            ParseArgumentsAndRun<TParam>(
                args,
                parameters => action(
                    parameters,
                    LazyExtensions.InitLazy(() => serviceCollection.BuildServiceProvider()),
                    serviceCollection));
        }

        private static void HandleModulDengi(
            ModulDengiParameters parameters,
            Lazy<ServiceProvider> serviceProvider,
            IServiceCollection serviceCollection)
        {
            var config = CreateModulDengiAccessConfig();

            serviceCollection.AddSingleton(Options.Create(config)).AddTransient<IModulDengiApi, ModulDengiApi>();

            var accountStatements = serviceProvider.Value.GetRequiredService<IModulDengiApi>()
                .GetAccountStatements(config.MyCompanyId, dateSince: parameters.DateSince).OrderBy(x => x.CreatedAt);

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
        /// 2 convert the PDF to XLSX https://www.adobe.com/acrobat/online/pdf-to-excel.html
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="serviceCollection"></param>
        /// <exception cref="NotSupportedException"></exception>
        private static void HandleFreedomFinanceBank(
            FreedomFinanceBankParameters parameters,
            Lazy<ServiceProvider> serviceProvider,
            IServiceCollection serviceCollection)
        {
            switch (parameters.ConvertTo)
            {
                case DataSourceType.ZenMoney:
                    var transactions = Worker.Extract(parameters.InputFilePath).ToArray();

                    foreach (var item in ZenMoneyConverter.ConvertToJsFetchRequest(transactions))
                    {
                        Console.WriteLine(item);
                    }
                    break;
                default:
                    throw new NotSupportedException($"{nameof(HandleFreedomFinanceBank)} {parameters.ConvertTo}");
            }
            
        }
    }
}