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

    public class ModulDengiToElbaConverterTests
    {
        private const string MyCompanyId = "";

        private IModulDengiApi modulDengiApi;

        [SetUp]
        public void SetUp()
        {
            var modulDengiAccessConfig = new ModulDengiAccessConfig
                {
                    SiteUrl = "https://cabinet.mdfin.ru",
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
                .AddTransient(provider => new HttpRequestBuilder(enableLogging: true));
            
            var serviceProvider = services.BuildServiceProvider();
            
            this.modulDengiApi = serviceProvider.GetService<IModulDengiApi>();
        }

        [Test]
        public void ConvertToJsFetchRequestTest()
        {
            var accountStatements = this.modulDengiApi.GetAccountStatements(
                MyCompanyId,
                dateSince: DateTime.Parse("2021-12-30"));
            foreach (var item in ModulDengiToElbaConverter.ConvertToJsFetchRequest(accountStatements))
            {
                Console.WriteLine(item);
            }
        }
    }
}