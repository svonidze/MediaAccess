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
            using var container = InitializeContainer().Install(WindsorInstaller.Register());
            
            var serverLogger = container.Resolve<IServerLogger>();
            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
                UnhandledExceptionTrapper(eventArgs, serverLogger);

            string configurationFilePath;
            if (args.Any())
            {
                configurationFilePath = args.First();
            }
            else
            {
                serverLogger.Log($"No configuration file path passed, trying to use the default one at {ConfigFilePath}");
                configurationFilePath = ConfigFilePath;
            }

            if (!File.Exists(configurationFilePath))
            {
                serverLogger.Log($"No configuration file found at {configurationFilePath}");
                Environment.Exit(1);
            }

            var configuration = File.ReadAllText(configurationFilePath).FromYamlTo<Configuration>();

            serverLogger.Log($"{nameof(configuration)} is loaded from {configurationFilePath}");
            container.Install(WindsorInstaller.Register(configuration));

            var telegramListener = container.Resolve<ITelegramListener>();
            telegramListener.StartReceiving();
            Thread.Sleep(int.MaxValue);
        }

        private static void UnhandledExceptionTrapper(UnhandledExceptionEventArgs e, IServerLogger serverLogger)
        {
            serverLogger.Log(e.ExceptionObject.ToString());
        }

        private static WindsorContainer InitializeContainer()
        {
            var container = new WindsorContainer();
            container.AddFacility<TypedFactoryFacility>();
            
            return container;
        }
    }
}