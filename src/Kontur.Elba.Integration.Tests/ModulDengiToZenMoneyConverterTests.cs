namespace Kontur.Elba.Integration.Tests
{
    using System;

    using Common.Http;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    using ModulDengi.Contracts;
    using ModulDengi.Integration;
    using ModulDengi.Integration.Contracts;

    using NUnit.Framework;

    public class ModulDengiToZenMoneyConverterTests
    {
        private const string MyCompanyId = "";

        private IModulDengiApi modulDengiApi;

        [SetUp]
        public void SetUp()
        {
            var modulDengiAccessConfig = new ModulDengiAccessConfig
                {
                    SiteUrl = "https://cabinet.moduldengi.ru",
                    MyCompanyId = MyCompanyId,
                    Credential = new Credential
                        {
                            Login = "",
                            Password = ""
                        }
                };
            var services = new ServiceCollection()
                .AddOptions()
                .AddSingleton(Options.Create(modulDengiAccessConfig))
                .AddTransient<IModulDengiApi, ModulDengiApi>()
                .AddTransient(provider => new HttpRequestBuilder(enableLogging: false));
            
            var serviceProvider = services.BuildServiceProvider();
            
            this.modulDengiApi = serviceProvider.GetService<IModulDengiApi>();
        }

        [Test]
        public void ConvertToJsFetchRequestTest()
        {
            var accountStatements = this.modulDengiApi.GetAccountStatements(
                MyCompanyId,
                dateSince: DateTime.Parse("2020-12-29"));
            foreach (var item in ModulDengiToElbaConverter.ConvertToJsFetchRequest(accountStatements))
            {
                Console.WriteLine(item);
            }
        }
    }
}