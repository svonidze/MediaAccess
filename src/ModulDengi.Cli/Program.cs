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

    using Kontur.Elba.Integration;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    using ModulDengi.Contracts;
    using ModulDengi.Integration;
    using ModulDengi.Integration.Contracts;

    using ZenMoney.Integration;

    internal class Program
    {
        private const string SiteUrl = "https://cabinet.mdfin.ru";

        private static void Main(string[] args)
        {
            ParseArgumentsAndRun<LauncherParameters>(
                args,
                parameters =>
                    {
                        var modulDengiAccessConfig = new ModulDengiAccessConfig
                            {
                                SiteUrl = parameters.Url ?? SiteUrl,
                                MyCompanyId = parameters.CompanyId,
                                Credential = new Credential
                                    {
                                        Login = parameters.Login,
                                        Password = parameters.Password
                                    }
                            };

                        Run(parameters.ConversionDirectionType, parameters.DateSince, modulDengiAccessConfig);
                    });
        }

        private static void Run(
            ConversionDirectionType conversionDirectionType,
            DateTime dateSince,
            ModulDengiAccessConfig config)
        {
            var services = new ServiceCollection()
                .AddOptions()
                .AddSingleton(Options.Create(config))
                .AddTransient<IModulDengiApi, ModulDengiApi>()
                .AddTransient(_ => new HttpRequestBuilder(enableLogging: true));

            var serviceProvider = services.BuildServiceProvider();
            var modulDengiApi = serviceProvider.GetService<IModulDengiApi>();

            var accountStatements = modulDengiApi.GetAccountStatements(config.MyCompanyId, dateSince: dateSince)
                .OrderBy(x => x.CreatedAt);
            var items = conversionDirectionType switch
                {
                    ConversionDirectionType.ToElba => ModulDengiToElbaConverter.ConvertToJsFetchRequest(
                        accountStatements),
                    ConversionDirectionType.ToZenMoney => ModulDengiToZenMoneyConverter.ConvertToJsFetchRequest(
                        accountStatements),
                    _ => throw new NotSupportedException($"{conversionDirectionType}")
                };
            
            //TextWriter oldOut = Console.Out;
            const string LogFilePath = "./run.log";

            Console.WriteLine($"Console log will be redirected to file {LogFilePath}");
                        
            using var fileStream = new FileStream(LogFilePath, FileMode.Create, FileAccess.Write);
            using var writer = new StreamWriter(fileStream);
            Console.SetOut(writer);
            foreach (var item in items)
            {
                Console.WriteLine(item);
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
    }
}