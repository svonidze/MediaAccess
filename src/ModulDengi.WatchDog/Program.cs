namespace ModulDengi.WatchDog
{
    using System;
    using System.IO;
    using System.Linq;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using ModulDengi.Contracts;
    using ModulDengi.Core;
    using ModulDengi.Integration;
    using ModulDengi.Integration.Contracts;

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureAppConfiguration(
                (hostContext, config) =>
                    {
                        if (args.Any())
                        {
                            var configFilePath = args.First();
                            if (!File.Exists(configFilePath))
                                throw new FileNotFoundException(configFilePath);

                            config.AddJsonFile(configFilePath, optional: false, reloadOnChange: true);
                        }
                        else
                        {
                            static string GetAppSettingsPath(string environment = null) =>
                                $"appsettings{(environment != null ? $".{environment}" : null)}.json";

                            config.SetBasePath(Environment.CurrentDirectory)
                                .AddJsonFile(GetAppSettingsPath(), optional: true, reloadOnChange: true)
                                .AddJsonFile(
                                    GetAppSettingsPath(hostContext.HostingEnvironment.EnvironmentName),
                                    optional: true);                            
                        }

                        config.AddEnvironmentVariables();
                    }).ConfigureServices(
                (hostContext, services) =>
                    {
                        var modulDengiSection = hostContext.Configuration.GetSection("ModulDengi");
                        var confirmationCheckSection = modulDengiSection.GetSection("ConfirmationCheck");

                        services.Configure<ModulDengiAccessConfig>(modulDengiSection);
                        services.Configure<EmailSettings>(confirmationCheckSection.GetSection("Email"));
                        services.Configure<TimeSettings>(
                            "newProjectCheckTimeSettings",
                            modulDengiSection.GetSection("NewProjectCheck:TimeSettings"));
                        services.Configure<TimeSettings>(
                            "confirmationCheckTimeSettings",
                            confirmationCheckSection.GetSection("TimeSettings"));

                        services.AddTransient<IModulDengiClient, ModulDengiClient>()
                            .AddTransient<IModulDengiApi, ModulDengiApi>()
                            .AddTransient<IConfirmationManager, EmailConfirmationManager>()
                            .AddTransient<IAccountant, Accountant>();

                        services.AddHostedService<Worker>();
                    });
    }
}