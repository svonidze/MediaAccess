namespace ModulDengi.Cli
{
    using System;
    using System.Collections.Generic;
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

                        Run(parameters.ConversionDirectionType, parameters.SinceDate, modulDengiAccessConfig);
                    });
        }

        private static void Run(
            ConversionDirectionType conversionDirectionType,
            DateTime sinceDate,
            ModulDengiAccessConfig config)
        {
            var services = new ServiceCollection().AddOptions().AddSingleton(Options.Create(config))
                .AddTransient<IModulDengiApi, ModulDengiApi>()
                .AddTransient(provider => new HttpRequestBuilder(enableLogging: true));

            var serviceProvider = services.BuildServiceProvider();

            var modulDengiApi = serviceProvider.GetService<IModulDengiApi>();

            var accountStatements = modulDengiApi.GetAccountStatements(config.MyCompanyId, sinceDate: sinceDate);

            switch (conversionDirectionType)
            {
                case ConversionDirectionType.ToElba:
                    {
                        foreach (var item in ModulDengiToElbaConverter.ConvertToJsFetchRequest(accountStatements))
                        {
                            Console.WriteLine(item);
                        }

                        break;
                    }
                case ConversionDirectionType.ToZenMoney:
                    {
                        foreach (var item in ModulDengiToZenMoneyConverter.ConvertToJsFetchRequest(accountStatements))
                        {
                            Console.WriteLine(item);
                        }

                        break;
                    }
                default:
                    throw new NotSupportedException($"{conversionDirectionType}");
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