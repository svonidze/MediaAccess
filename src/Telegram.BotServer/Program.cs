namespace Telegram.FirstBot
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using Castle.Facilities.TypedFactory;
    using Castle.Windsor;

    using Common.Serialization.Yaml;

    using MediaServer.Contracts;

    internal class Program
    {
        private const string ConfigFilePath = "/config/config.yaml";
        
        // TODO utilize library to parse CLI args 
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;

            string configurationFilePath;
            if (args.Any())
            {
                configurationFilePath = args.First();
            }
            else
            {
                Console.WriteLine($"No configuration file path passed, trying to use the default one at {ConfigFilePath}");
                configurationFilePath = ConfigFilePath;
            }

            if (!File.Exists(configurationFilePath))
            {
                Console.WriteLine($"No configuration file found at {configurationFilePath}");
                Environment.Exit(1);
            }

            var configuration = File.ReadAllText(configurationFilePath).FromYamlTo<Configuration>();
            Console.WriteLine($"{nameof(configuration)} is loaded from {configurationFilePath}");

            using var container = InitializeContainer(configuration);
            var telegramListener = container.Resolve<ITelegramListener>();
            telegramListener.StartReceiving();
            Thread.Sleep(int.MaxValue);
        }

        private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject.ToString());
        }

        private static WindsorContainer InitializeContainer(Configuration configuration)
        {
            var container = new WindsorContainer();
            container.AddFacility<TypedFactoryFacility>();

            container.Install(new WindsorInstaller(configuration));
            return container;
        }
    }
}